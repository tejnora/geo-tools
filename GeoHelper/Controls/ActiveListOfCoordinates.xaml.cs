using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GeoBase.Localization;
using GeoHelper.Tables;
using GeoHelper.Tabulky;

namespace GeoHelper.Controls
{
    public partial class ActiveListOfCoordinates
    {
        public ActiveListOfCoordinates()
        {
            InitializeComponent();
        }

        public ObservableCollection<ActiveListOfCoordinatesNode> Nodes { get; set; }

        public void Init(List<Tuple<string, TableBase>> nodes, TableBase selectedNode)
        {
            Nodes = new ObservableCollection<ActiveListOfCoordinatesNode>();
            var dictionary = LanguageConverter.ResolveDictionary();
            Nodes.Add(new ActiveListOfCoordinatesNode {Name = dictionary.Translate<string>("472", "Text"), Data = null});
            var selId = 0;
            foreach (var node in nodes)
            {
                if (node.Item2 == selectedNode)
                    selId = Nodes.Count;
                Nodes.Add(new ActiveListOfCoordinatesNode {Name = node.Item1, Data = node.Item2});
            }
            _dataGrid.SelectedIndex = selId;
            DataContext = this;
        }

        public TableBase GetSelectedTable()
        {
            var selNode = _dataGrid.SelectedItem as ActiveListOfCoordinatesNode;
            if (selNode != null) return (TableBase) selNode.Data;
            return null;
        }
    }
}