using System;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables
{
    public interface IUndoRedo
    {
        void BeginUndo();
        void AddDeletedItem(TableNodesBase node);
        void AddAddedItem(TableNodesBase node);
        void AddEditedNode(TableNodesBase node);
        void EndUndo();
    }

    internal class Action : IDisposable
    {
        public Action(TableBase owner)
        {
            _owner = owner;
            _owner.BeginUndo();
        }

        public void AddDeletedItem(TableNodesBase node)
        {
            _owner.AddDeletedItem(node);
        }

        public void AddAddedItem(TableNodesBase node)
        {
            _owner.AddAddedItem(node);
        }

        public void AddEditedNode(TableNodesBase node)
        {
            _owner.AddEditedNode(node);
        }

        readonly TableBase _owner;

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            _owner.EndUndo();
        }

        ~Action()
        {
            Dispose(false);
        }
    }
}