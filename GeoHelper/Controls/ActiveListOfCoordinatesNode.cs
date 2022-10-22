using System.Runtime.Serialization;
using GeoBase.Utils;

namespace GeoHelper.Controls
{
    public class ActiveListOfCoordinatesNode : ActiveListOfCoordinatesNodeBase
    {
        public ActiveListOfCoordinatesNode()
            : base(null, new StreamingContext())
        {
        }

        public readonly PropertyData _NameProperty = RegisterProperty("Name", typeof(string), string.Empty);

        public string Name
        {
            get { return GetValue<string>(_NameProperty); }
            set { SetValue(_NameProperty, value); }
        }

        public object Data { get; set; }
    }
}
