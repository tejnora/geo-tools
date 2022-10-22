using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WSGPApp
{
    public class Node
    {
        IList<Node> _children;
        public string Header { get; private set; }
        public string Header1 { get; private set; }
        public IList<Node> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = Enumerable.Range(1, 10).Select(i => new Node() { Header = $"Item {i}",Header1 = $"Item1 {i}" })
                        .ToArray();
                }
                return _children;
            }
        }
    }
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Node().Children;
#if DEBUG
            this.AttachDevTools();
#endif
        }

        void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
