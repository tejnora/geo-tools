using System;
using System.Runtime.Serialization;

namespace GeoHelper.Tables
{
    public partial class CoordinateListInfoDialog
    {
        public CoordinateListInfoDialog(CoordinateListInfoDialogContext context)
            : base("CoordinateListInfoDialog")
        {
            InitializeComponent();
            Context = context;
            DataContext = Context;
        }

        public CoordinateListInfoDialogContext Context { get; private set; }
    }

    [Serializable]
    public class CoordinateListInfoDialogContext : TableInfoContext
    {
        public CoordinateListInfoDialogContext()
        {
        }

        public CoordinateListInfoDialogContext(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}