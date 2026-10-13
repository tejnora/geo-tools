using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CAD.DTM.Validation;
using GeoBase.Gui;

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
        public List<DtmValidationTreeNode> Children { get; } = new List<DtmValidationTreeNode>();
    }

    public partial class DtmValidationDialog : DialogBase
    {
        public DtmValidationDialog(DtmValidationResult result)
            : base("DtmValidationDialog", false, true)
        {
            Result = result;
            Nodes = BuildNodes(result);
            var failed = result.Rules.Count(r => r.Status == DtmRuleStatus.Failed);
            var warned = result.Rules.Count(r => r.Status == DtmRuleStatus.Warning);
            Summary = failed == 0 && warned == 0 ? "Všechny provedené kontroly byly splněny."
                : $"Skupin s chybami: {failed}, skupin s varováními: {warned}";
            InitializeComponent();
            DataContext = this;
        }

        public DtmValidationResult Result { get; }
        public IList<DtmValidationTreeNode> Nodes { get; }
        public string Summary { get; }

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
                    Icon = issue.IsWarning ? "⚠" : "•",
                    Brush = issue.IsWarning ? Brushes.DarkOrange : Brushes.Red
                });
            return node;
        }

        void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
