using GeoCalculations.MethodPoints;
using GeoHelper.Controls;
using GeoHelper.Protocols;
using GeoHelper.Protocols.Gui;

namespace GeoHelper.Tabulky
{
    internal interface IManipulateItems
    {
        bool CanEdit();
        bool CanDelete();
        bool CanInsert();
        void OnInsert();
        void Delete(bool skipWarnning);
        void OnEdit();
        void OnShowInfoDialog();
        bool GetHasSouborInfo();
        bool AddNewNode(CalculatedPointBase newPoint, IProtocolContext protocolContext);
        bool CanCopy();
        void Copy();
        bool CanPaste();
        void Paste();
        bool CanCut();
        void Cut();
        bool CanUndo();
        void Undo();
        bool CanRedo();
        void Redo();
    }
}