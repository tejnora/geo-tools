using System;
using System.IO;
using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoHelper.Utils;

namespace GeoHelper.Tables
{
    public partial class MeasureListInfoDialog
    {
        public MeasureListInfoDialog(SeznamMereniInfoDialogContext context)
            : base("MeasureListInfoDialog")
        {
            InitializeComponent();
            Context = context;
            DataContext = Context;
        }

        public SeznamMereniInfoDialogContext Context { get; private set; }
    }

    [Serializable]
    public class SeznamMereniInfoDialogContext : TableInfoContext
    {
        public SeznamMereniInfoDialogContext()
        {
        }

        public SeznamMereniInfoDialogContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void Serialize(Stream aWriter)
        {
            base.Serialize(aWriter);
            aWriter.WriteString(MeasureMan);
            aWriter.WriteString(TotalStation);
            aWriter.WriteString(Date);
        }

        public override void Deserialize(Stream aReader)
        {
            base.Deserialize(aReader);
            MeasureMan = aReader.ReadString();
            TotalStation = aReader.ReadString();
            Date = aReader.ReadString();
        }

        readonly PropertyData _measureManProperty = RegisterProperty("MeasureMan", typeof(string), string.Empty);
        public string MeasureMan
        {
            get { return GetValue<string>(_measureManProperty); }
            set { SetValue(_measureManProperty, value); }
        }

        readonly PropertyData _totalstationProperty = RegisterProperty("TotalStation", typeof(string), string.Empty);
        public string TotalStation
        {
            get { return GetValue<string>(_totalstationProperty); }
            set { SetValue(_totalstationProperty, value); }
        }

        public readonly PropertyData _dateProperty = RegisterProperty("Date", typeof(string), string.Empty);
        public string Date
        {
            get { return GetValue<string>(_dateProperty); }
            set { SetValue(_dateProperty, value); }
        }
    }
}