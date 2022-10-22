using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using GeoCalculations.MethodPoints;
using GeoHelper.Protocols;
using GeoHelper.Tables.TableNodes;
using WPF.MDI;

namespace GeoHelper
{
    public enum TableType
    {
        Souradnice,
    }

    public interface IMainWindow
    {
        //Vypocty
        bool AddNewNodeIntoTable(Type tableType, CalculatedPointBase newPoint, IProtocolContext protocolContex);
        void ReleaseCalculationMethod(string name);
        TableNodesBase FindNodeFromTables(string uplneCislo, TableType tableType);

        //Tabulky
        void CreateWindowWithDoubleCoordinates(List<TableNodesBase> aFromNode);
        void SetModifiedFlag(object table);
        MdiChild GetMdiWindow(object tableContent);
        void AddTable(UIElement table);

        void AppendTextIntoProtocol(FlowDocument document);
    }
}