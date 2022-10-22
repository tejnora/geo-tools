using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GeoBase.Utils;
using GeoHelper.Utils;

namespace GeoHelper.Options
{
    public partial class OptionsDialog
    {
        public OptionsDialog(OptionsDialogContext ctx)
            : base("OptionsDialog")
        {
            InitializeComponent();
            OptionsDialogContext = ctx;
            var po = Singletons.MyRegistry.getEntry("OptionsDialog/LastSelectedItem");
            if (po.isString() && po.getString() != "empty")
            {
                ChangeContent(po.getString());
            }
        }

        public OptionsDialogContext OptionsDialogContext { get; private set; }

        void OnSelectedItemChanged(object sender, EventArgs args)
        {
            var tvi = (TreeViewItem)_treeView.SelectedItem;
            ChangeContent((string)tvi.Tag);
        }

        void ChangeContent(string name)
        {
            while (_content.Children.Count != 0)
                _content.Children.RemoveAt(0);
            var className = "GeoHelper.Options." + name;
            var type = Type.GetType(className);
            if (type == null)
                return;
            var content = (UIElement)Activator.CreateInstance(type);
            ((IOptionItem)content).Context = OptionsDialogContext.GetContext(name);
            _content.Children.Add(content);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            var tvi = (TreeViewItem)_treeView.SelectedItem;
            Singletons.MyRegistry.setEntry("OptionsDialog/LastSelectedItem",
                                           tvi == null
                                               ? new ProgramOption("empty")
                                               : new ProgramOption((string)tvi.Tag));
        }
    }

    public class OptionsDialogContext
    {
        public OptionsDialogContext()
        {
            _contexts = new Dictionary<string, IOptionContextItem>();
        }

        readonly Dictionary<string, IOptionContextItem> _contexts;

        public IOptionContextItem GetContext(string name)
        {
            if (_contexts.ContainsKey(name))
                return _contexts[name];
            var className = "GeoHelper.Options." + name + "Context";
            var type = Type.GetType(className);
            if (type == null)
                return null;
            var context = (IOptionContextItem)Activator.CreateInstance(type);
            context.LoadFromRegistry();
            _contexts[name] = context;
            return context;
        }

        public void SaveToRegistry()
        {
            foreach (var item in _contexts)
            {
                item.Value.SaveToRegistry();
            }
        }
    }
}