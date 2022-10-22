using System;
using System.IO;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoHelper.Utils;

namespace GeoHelper.Tables
{
    [Serializable]
    public class TableInfoContext : DataObjectBase<TableInfoContext>
    {
        public TableInfoContext()
            : base(null, new StreamingContext())
        {
        }

        public TableInfoContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public virtual void Serialize(Stream aWriter)
        {
            aWriter.WriteByte(0);
            aWriter.WriteString(FileDescription);
            aWriter.WriteString(Job);
            aWriter.WriteString(Locality);
            aWriter.WriteString(CoordinateSystem);
            aWriter.WriteString(LevelSystem);
            aWriter.WriteString(FileDescription);
            aWriter.WriteString(Notes);
        }

        public virtual void Deserialize(Stream aReader)
        {
            aReader.ReadByte();
            FileDescription = aReader.ReadString();
            Job = aReader.ReadString();
            Locality = aReader.ReadString();
            CoordinateSystem = aReader.ReadString();
            LevelSystem = aReader.ReadString();
            FileDescription = aReader.ReadString();
            Notes = aReader.ReadString();
        }

        readonly PropertyData _fileNameProperty = RegisterProperty("FileName", typeof(string), string.Empty);
        public string FileName
        {
            get { return GetValue<string>(_fileNameProperty); }
            set { SetValue(_fileNameProperty, value); }
        }

        readonly PropertyData _fileDescriptionProperty = RegisterProperty("FileDescription", typeof(string), string.Empty);
        public string FileDescription
        {
            get { return GetValue<string>(_fileDescriptionProperty); }
            set { SetValue(_fileDescriptionProperty, value); }
        }

        readonly PropertyData _jobProperty = RegisterProperty("Job", typeof(string), string.Empty);
        public string Job
        {
            get { return GetValue<string>(_jobProperty); }
            set { SetValue(_jobProperty, value); }
        }

        readonly PropertyData _localityProperty = RegisterProperty("Locality", typeof(string), string.Empty);
        public string Locality
        {
            get { return GetValue<string>(_localityProperty); }
            set { SetValue(_localityProperty, value); }
        }

        readonly PropertyData _coordinateSystemProperty = RegisterProperty("CoordinateSystem", typeof(string), string.Empty);
        public string CoordinateSystem
        {
            get { return GetValue<string>(_coordinateSystemProperty); }
            set { SetValue(_coordinateSystemProperty, value); }
        }

        readonly PropertyData _levelSystemProperty = RegisterProperty("LevelSystem", typeof(string), string.Empty);
        public string LevelSystem
        {
            get { return GetValue<string>(_levelSystemProperty); }
            set { SetValue(_levelSystemProperty, value); }
        }

        readonly PropertyData _notesProperty = RegisterProperty("Notes", typeof(string), string.Empty);
        public string Notes
        {
            get { return GetValue<string>(_notesProperty); }
            set { SetValue(_notesProperty, value); }
        }
    }
}