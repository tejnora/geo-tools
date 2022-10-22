using System;
using System.Windows;
using System.Windows.Documents;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoCalculations.Protocol;

namespace GeoHelper.Protocols.Gui
{

    public partial class ProtocolDialog
    {
        readonly IProtocolContext _protocolContex;
        readonly IMainWindow _owner;

        public ProtocolDialog(IProtocolContext protocolContex, IMainWindow owner)
            : base("ProtokolDialog")
        {
            InitializeComponent();
            _protocolContex = protocolContex;
            _owner = owner;
        }

        public string Text
        {
            get
            {
                var textRange = new TextRange(_textEditor.Document.ContentStart, _textEditor.Document.ContentEnd);
                return textRange.Text;
            }
        }

        public bool updateContent()
        {
            try
            {
                var type = _protocolContex.GetType();
                var attributes = type.GetCustomAttributes(false);
                string templateName = null;
                for (var i = 0; i < attributes.Length; i++)
                {
                    var data = attributes[i] as ProtocolRootDataAttribute;
                    if (data == null) continue;
                    templateName = data.TemplateName;
                }
                if (string.IsNullOrEmpty(templateName))
                {
                    throw new ArgumentException("Invalid template name.");
                }
                _textEditor.Document.Blocks.Clear();
                var dataProperties = ScanProtocolResultData.GetProperties(type);
                var template = LanguageDictionary.Current.Translate<string>("Protocols.Protocol." + templateName, "Text");
                if (string.IsNullOrEmpty(template))
                    throw new ArgumentException("Template can not be empty:.");
                var parsedTemplate = ProtocolGrammar.Terms.Parse(template.Replace("\r\n", ""));
                var generator = new ProtocolGenerator();
                generator.RegisterPlugin(ProtocolPluginTypes.Units, new ProtocolUnitsPlugin(Singletons.MyRegistry));
                var evaluatedString = generator.Eval(parsedTemplate[0], dataProperties, _protocolContex);
                var first = true;
                foreach (var line in evaluatedString.Split('\n'))
                {
                    if (first)
                    {
                        first = false;
                        if (line == "\r" || line.Length == 0)
                            continue;
                    }
                    var par = new Paragraph();
                    par.Inlines.Add(line.EndsWith("\r") ? line.Substring(0, line.Length - 1) : line);
                    _textEditor.Document.Blocks.Add(par);
                }
                _textEditor.ScrollToEnd();
                return true;
            }
            catch (Exception ex)
            {
                var par = new ResourceParams();
                par.Add("error", ex.ToString());
                LanguageDictionary.Current.ShowMessageBox("ProtocolDialog.1", par, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        public override void EndInit()
        {
            base.EndInit();
            AddAnyButton("Erease").Click += OnClear;
            AddAnyButton("Save").Click += OnSave;
        }

        void OnSave(object sender, EventArgs args)
        {

            _owner.AppendTextIntoProtocol(_textEditor.Document);
        }

        void OnClear(object sender, EventArgs args)
        {
            _textEditor.Document.Blocks.Clear();
            _protocolContex.Clear();
        }
    }
}