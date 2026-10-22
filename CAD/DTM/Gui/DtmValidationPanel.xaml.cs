using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CAD.DTM.Validation;

namespace CAD.DTM.Gui
{
    public class DtmValidationTreeNode
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public Brush Brush { get; set; }
        public string ToolTip { get; set; }
        public bool HasToolTip => !string.IsNullOrEmpty(ToolTip);
        public bool IsExpanded { get; set; }
        public IDtmElement Element { get; set; }
        public List<DtmValidationTreeNode> Children { get; } = new List<DtmValidationTreeNode>();
    }

    public partial class DtmValidationPanel : UserControl
    {
        internal const string VisibilitySettingKey = "Dtm/ValidationPanelVisible";
        Document _document;
        public DtmValidationPanel()
        {
            InitializeComponent();
            SetDocument(null);
        }

        public void SetDocument(Document document)
        {
            if (_document != null) _document.DtmValidationStateChanged -= OnValidationStateChanged;
            _document = document;
            if (_document != null) _document.DtmValidationStateChanged += OnValidationStateChanged;
            _runValidation.IsEnabled = document?.DataModel.DtmMain != null;
            SetResult(document?.DtmValidationResult);
        }

        public void SetResult(DtmValidationResult result)
        {
            Result = result;
            Nodes = result == null ? new List<DtmValidationTreeNode>() : BuildNodes(result);
            var failed = result?.Rules.Count(r => r.Status == DtmRuleStatus.Failed) ?? 0;
            var warned = result?.Rules.Count(r => r.Status == DtmRuleStatus.Warning) ?? 0;
            Summary = result == null ? "Kontrola dosud nebyla spuštěna."
                : failed == 0 && warned == 0 ? "Všechny provedené kontroly byly splněny."
                : $"Skupin s chybami: {failed}, skupin s varováními: {warned}";
            DataContext = null;
            DataContext = this;
            UpdateStaleWarning();
        }

        void OnValidationStateChanged(object sender, System.EventArgs e) => UpdateStaleWarning();
        void UpdateStaleWarning() => _staleWarning.Visibility = _document?.IsDtmValidationStale == true
            ? Visibility.Visible : Visibility.Collapsed;

        public DtmValidationResult Result { get; private set; }
        public IList<DtmValidationTreeNode> Nodes { get; private set; }
        public string Summary { get; private set; }

        static List<DtmValidationTreeNode> BuildNodes(DtmValidationResult result)
        {
            return result.Rules.Select(BuildNode).ToList();
        }

        static DtmValidationTreeNode BuildNode(DtmRuleResult rule)
        {
            var node = new DtmValidationTreeNode { Text = $"{rule.Code} {rule.Name}", ToolTip = rule.Description ?? rule.Name };
            switch (rule.Status)
            {
                case DtmRuleStatus.Passed:
                    node.Icon = "✔";
                    node.Brush = Brushes.Green;
                    break;
                case DtmRuleStatus.Failed:
                    node.Icon = "✖";
                    node.Brush = Brushes.Red;
                    node.IsExpanded = true;
                    break;
                case DtmRuleStatus.Warning:
                    node.Icon = "⚠";
                    node.Brush = Brushes.DarkOrange;
                    node.IsExpanded = true;
                    break;
                default:
                    node.Icon = "–";
                    node.Brush = Brushes.Gray;
                    break;
            }
            if (!string.IsNullOrEmpty(rule.Note))
                node.Children.Add(new DtmValidationTreeNode { Text = rule.Note, Icon = "i", Brush = Brushes.Gray });
            foreach (var sub in rule.SubResults)
                node.Children.Add(BuildNode(sub));
            foreach (var issue in rule.Issues)
                node.Children.Add(new DtmValidationTreeNode
                {
                    Text = issue.ToString(),
                    Element = issue.Element,
                    Icon = issue.IsWarning ? "⚠" : "•",
                    Brush = issue.IsWarning ? Brushes.DarkOrange : Brushes.Red
                });
            return node;
        }

        void OnValidate(object sender, RoutedEventArgs e)
        {
            _runValidation.IsEnabled = false;
            try { _document?.ValidateDtm(); }
            finally { _runValidation.IsEnabled = _document?.DataModel.DtmMain != null; }
        }

        void OnSelectedIssueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectIssue(e.NewValue as DtmValidationTreeNode);
        }

        void OnIssueClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectIssue((sender as FrameworkElement)?.DataContext as DtmValidationTreeNode);
        }

        void SelectIssue(DtmValidationTreeNode node)
        {
            if (node?.Element != null) _document?.SelectDtmValidationObject(node.Element);
        }
    }
}
