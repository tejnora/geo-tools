using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Utils;

namespace GeoHelper.Tables
{
    public class UndoRedo
    {
        enum UndoRedoType : byte
        {
            Delete,
            Insert,
            Edit
        }

        public UndoRedo(TableBase owner)
        {
            _owner = owner;
            ResetUndoRedo();
        }

        readonly TableBase _owner;
        bool _beginUndo;
        bool _corupted;
        bool _doRedo;
        bool _doUndo;
        int _modifiedNodeCount;
        Stream _redoStream;
        Stack<long> _redoStreamOffsets;
        Stream _undoStream;
        Stack<long> _undoStreamOffsets;

        public void BeginUndo()
        {
            if (_corupted || _beginUndo) return;
            _modifiedNodeCount = 0;
            UpdateUndoRedoSize();
            try
            {
                if (_doRedo)
                {
                    _redoStreamOffsets.Push(_redoStream.Position);
                    _redoStream.WriteInt32(0);
                }
                else
                {
                    _undoStreamOffsets.Push(_undoStream.Position);
                    _undoStream.WriteInt32(0);
                }
                _beginUndo = true;
            }
            catch (Exception)
            {
                ResetUndoRedo();
                _corupted = true;
            }
        }

        public void AddDeletedItem(TableNodesBase node)
        {
            if (_corupted) return;
            Debug.Assert(_beginUndo);
            try
            {
                int index = _owner.Nodes.IndexOf(node);
                if (_doRedo)
                {
                    _redoStream.WriteByte((byte) UndoRedoType.Insert);
                    _redoStream.WriteInt32(index);
                    node.Serialize(new BinaryWriter(_redoStream));
                }
                else
                {
                    _undoStream.WriteByte((byte) UndoRedoType.Insert);
                    _undoStream.WriteInt32(index);
                    node.Serialize(new BinaryWriter(_undoStream));
                }
                _modifiedNodeCount++;
            }
            catch (Exception)
            {
                ResetUndoRedo();
                _corupted = true;
            }
        }

        public void AddAddedItem(TableNodesBase node)
        {
            if (_corupted) return;
            Debug.Assert(_beginUndo);
            try
            {
                int index = _owner.Nodes.IndexOf(node);
                if (_doRedo)
                {
                    _redoStream.WriteByte((byte) UndoRedoType.Delete);
                    _redoStream.WriteInt32(index);
                }
                else
                {
                    _undoStream.WriteByte((byte) UndoRedoType.Delete);
                    _undoStream.WriteInt32(index);
                }
                _modifiedNodeCount++;
            }
            catch (Exception)
            {
                ResetUndoRedo();
                _corupted = true;
            }
        }

        public void AddEditedNode(TableNodesBase node)
        {
            if (_corupted) return;
            Debug.Assert(_beginUndo);
            try
            {
                int index = _owner.Nodes.IndexOf(node);
                if (_doRedo)
                {
                    _redoStream.WriteByte((byte) UndoRedoType.Edit);
                    _redoStream.WriteInt32(index);
                    node.Serialize(new BinaryWriter(_redoStream));
                }
                else
                {
                    _undoStream.WriteByte((byte) UndoRedoType.Edit);
                    _undoStream.WriteInt32(index);
                    node.Serialize(new BinaryWriter(_undoStream));
                }
                _modifiedNodeCount++;
            }
            catch (Exception)
            {
                ResetUndoRedo();
                _corupted = true;
            }
        }

        public void EndUndo()
        {
            if (_corupted) return;
            Debug.Assert(_beginUndo);
            try
            {
                _beginUndo = false;
                if (_modifiedNodeCount == 0)
                {
                    if (_doRedo)
                        _redoStream.Seek(_redoStreamOffsets.Pop(), SeekOrigin.Begin);
                    else
                        _undoStream.Seek(_undoStreamOffsets.Pop(), SeekOrigin.Begin);
                    return;
                }
                if (_doRedo)
                {
                    long backupPosition = _redoStream.Position;
                    _redoStream.Seek(_redoStreamOffsets.Peek(), SeekOrigin.Begin);
                    _redoStream.WriteInt32(_modifiedNodeCount);
                    _redoStream.Seek(backupPosition, SeekOrigin.Begin);
                }
                else
                {
                    long backupPosition = _undoStream.Position;
                    _undoStream.Seek(_undoStreamOffsets.Peek(), SeekOrigin.Begin);
                    _undoStream.WriteInt32(_modifiedNodeCount);
                    _undoStream.Seek(backupPosition, SeekOrigin.Begin);
                    if (!_doUndo)
                    {
                        _redoStream = new MemoryStream();
                        _redoStreamOffsets.Clear();
                    }
                }
            }
            catch (Exception)
            {
                ResetUndoRedo();
                _corupted = true;
            }
        }

        public void DoUndo()
        {
            if (_corupted) return;
            if (_undoStreamOffsets.Count == 0)
                return;
            _doRedo = true;
            try
            {
                long offset = _undoStreamOffsets.Pop();
                _undoStream.Seek(offset, SeekOrigin.Begin);
                int count = _undoStream.ReadInt32();
                using (var action = new Action(_owner))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var type = (UndoRedoType) _undoStream.ReadByte();
                        int index = _undoStream.ReadInt32();
                        switch (type)
                        {
                            case UndoRedoType.Insert:
                                {
                                    TableNodesBase node = _owner.GetNewNode();
                                    node.Deserialize(new BinaryReader(_undoStream));
                                    _owner.InsertItemRaw(node, index);
                                }
                                break;
                            case UndoRedoType.Delete:
                                _owner.DeleteItemRaw(index, true);
                                break;
                            case UndoRedoType.Edit:
                                {
                                    TableNodesBase node = _owner.GetNewNode();
                                    node.Deserialize(new BinaryReader(_undoStream));
                                    _owner.EditNode(node, index);
                                }
                                break;
                        }
                    }
                }
                _undoStream.Seek(offset, SeekOrigin.Begin);
            }
            catch (Exception)
            {
                Debug.Assert(false);
                ResetUndoRedo();
                _corupted = true;
            }
            finally
            {
                _doRedo = false;
            }
        }

        public void DoRedo()
        {
            if (_corupted) return;
            if (_redoStreamOffsets.Count == 0)
                return;
            try
            {
                _doUndo = true;
                long offset = _redoStreamOffsets.Pop();
                _redoStream.Seek(offset, SeekOrigin.Begin);
                int count = _redoStream.ReadInt32();
                using (var action = new Action(_owner))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var type = (UndoRedoType) _redoStream.ReadByte();
                        int index = _redoStream.ReadInt32();
                        switch (type)
                        {
                            case UndoRedoType.Insert:
                                {
                                    TableNodesBase node = _owner.GetNewNode();
                                    node.Deserialize(new BinaryReader(_redoStream));
                                    _owner.InsertItemRaw(node, index);
                                }
                                break;
                            case UndoRedoType.Delete:
                                _owner.DeleteItemRaw(index, true);
                                break;
                            case UndoRedoType.Edit:
                                {
                                    TableNodesBase node = _owner.GetNewNode();
                                    node.Deserialize(new BinaryReader(_redoStream));
                                    _owner.EditNode(node, index);
                                }
                                break;
                        }
                    }
                }
                _redoStream.Seek(offset, SeekOrigin.Begin);
            }
            catch (Exception)
            {
                Debug.Assert(false);
                ResetUndoRedo();
                _corupted = true;
            }
            finally
            {
                _doUndo = false;
            }
        }

        public bool CanUndo()
        {
            return _undoStreamOffsets.Count > 0;
        }

        public bool CanRedo()
        {
            return _redoStreamOffsets.Count > 0;
        }

        void ResetUndoRedo()
        {
            _undoStream = new MemoryStream();
            _undoStreamOffsets = new Stack<long>();
            _redoStream = new MemoryStream();
            _redoStreamOffsets = new Stack<long>();
        }

        void UpdateUndoRedoSize()
        {
            if (_corupted) return;
            Stream stream;
            Stack<long> offsets;
            if (_doRedo)
            {
                stream = _redoStream;
                offsets = _redoStreamOffsets;
            }
            else
            {
                stream = _undoStream;
                offsets = _undoStreamOffsets;
            }
            if (stream.Length > 10240000)
            {
                long[] array = offsets.ToArray();
                Array.Sort(array);
                int i = 0;
                while (i < array.Length)
                {
                    if (array[i] > 512000)
                        break;
                    i++;
                }
                try
                {
                    if (i > array.Length)
                    {
                        stream = new MemoryStream();
                        offsets.Clear();
                    }
                    else
                    {
                        Stream destStream = new MemoryStream();
                        stream.Seek(array[i], SeekOrigin.Begin);
                        stream.CopyTo(destStream);
                        stream = destStream;
                        offsets.Clear();
                        long offset = array[i];
                        while (i < array.Length)
                        {
                            offsets.Push(array[i] - offset);
                            i++;
                        }
                    }
                }
                catch (Exception)
                {
                    _corupted = true;
                    ResetUndoRedo();
                    return;
                }
                if (_doRedo)
                {
                    _redoStream = stream;
                    _redoStreamOffsets = offsets;
                }
                else
                {
                    _undoStream = stream;
                    _undoStreamOffsets = offsets;
                }
            }
        }
    }
}