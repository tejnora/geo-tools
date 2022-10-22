using System.Runtime.Serialization;
using GeoBase.Utils;

namespace GeoHelper.Controls
{
    public class ActiveListOfCoordinatesNodeBase : DataObjectBase<ActiveListOfCoordinates>
    {
        public ActiveListOfCoordinatesNodeBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}