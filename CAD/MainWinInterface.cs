using CAD.UITools;
using CAD.Canvas;
using GeoBase.Utils;

namespace CAD
{
    public interface IMainWinInterface
    {
        void SetPositionInfo(UnitPoint aUnitpos);
        void SetSnapInfo(ISnapPoint aSnap);
        void UpdateToolBars(GeoCadRoutedCommand aDrawObjectId);

        void DocumentNew(string aName);
        void CloseDocument();
    }
}
