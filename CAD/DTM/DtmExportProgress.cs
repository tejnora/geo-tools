using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using CAD.DTM.Validation;

namespace CAD.DTM
{
    internal enum DtmExportActionStatus
    {
        Waiting, Running, Passed, Warning, Failed, Skipped
    }

    internal sealed class DtmExportActionResult
    {
        internal DtmExportActionResult(DtmExportActionStatus status, string detail)
        {
            Status = status;
            Detail = detail;
        }

        internal DtmExportActionStatus Status { get; }
        internal string Detail { get; }
    }

    internal sealed class DtmExportAction : INotifyPropertyChanged
    {
        readonly Func<DtmExportActionResult> _execute;
        readonly string _initialDetail;

        internal DtmExportAction(string name, string detail, Func<DtmExportActionResult> execute, bool requiresPreviousSuccess = true)
        {
            Name = name;
            _initialDetail = Detail = detail;
            _execute = execute;
            RequiresPreviousSuccess = requiresPreviousSuccess;
        }

        public string Name { get; }
        internal bool RequiresPreviousSuccess { get; }
        public string Detail { get; private set; }
        public DtmExportActionStatus Status { get; private set; }
        public string StatusText => Status switch
        {
            DtmExportActionStatus.Running => "Probíhá",
            DtmExportActionStatus.Passed => "V pořádku",
            DtmExportActionStatus.Warning => "Varování",
            DtmExportActionStatus.Failed => "Selhalo",
            DtmExportActionStatus.Skipped => "Přeskočeno",
            _ => "Čeká"
        };
        public string Icon => Status switch
        {
            DtmExportActionStatus.Running => "…",
            DtmExportActionStatus.Passed => "✔",
            DtmExportActionStatus.Warning => "⚠",
            DtmExportActionStatus.Failed => "✖",
            _ => "–"
        };
        public Brush StatusBrush => Status switch
        {
            DtmExportActionStatus.Running => Brushes.SteelBlue,
            DtmExportActionStatus.Passed => Brushes.Green,
            DtmExportActionStatus.Warning => Brushes.DarkOrange,
            DtmExportActionStatus.Failed => Brushes.Firebrick,
            _ => Brushes.Gray
        };

        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetStatus(DtmExportActionStatus status, string detail)
        {
            Status = status;
            Detail = string.IsNullOrEmpty(_initialDetail) ? detail : _initialDetail + "\n\n" + detail;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        internal DtmExportActionResult Execute() => _execute();
    }

    internal sealed class DtmExportProgress
    {
        bool _started;

        internal DtmExportProgress(string outputPath, Func<DtmValidationResult> validate, Action export,
            Func<DtmExportActionResult> validateGadFiles = null)
            : this(CreateActions(outputPath, validate, export, validateGadFiles))
        {
        }

        static IEnumerable<DtmExportAction> CreateActions(string outputPath, Func<DtmValidationResult> validate,
            Action export, Func<DtmExportActionResult> validateGadFiles)
        {
            yield return new DtmExportAction("Validace DTM", "Kontrola stejnými pravidly jako na paletce validace DTM.",
                () => DescribeValidation(validate()));
            yield return new DtmExportAction("Zápis souboru: " + Path.GetFileName(outputPath), outputPath, () =>
            {
                export();
                return new DtmExportActionResult(DtmExportActionStatus.Passed, "Soubor byl úspěšně zapsán.");
            });
            if (validateGadFiles != null)
                yield return new DtmExportAction("Kontrola náležitostí GAD DTM",
                    "Existence a názvy příloh podle metodiky ČÚZK 2.1, kapitola 5.2.", validateGadFiles,
                    requiresPreviousSuccess: false);
        }

        internal DtmExportProgress(IEnumerable<DtmExportAction> actions)
        {
            Actions = Array.AsReadOnly(actions.ToArray());
        }

        public IReadOnlyList<DtmExportAction> Actions { get; }
        internal bool HasFailures => Actions.Any(a => a.Status == DtmExportActionStatus.Failed);

        // The caller yields to WPF before each action so its current state is rendered.
        // Actions stay on the UI thread because validation and export share the live document model.
        internal async Task RunAsync(Func<Task> render)
        {
            if (_started) return;
            _started = true;
            var failures = new List<string>();
            foreach (var action in Actions)
            {
                if (failures.Count > 0 && action.RequiresPreviousSuccess)
                {
                    action.SetStatus(DtmExportActionStatus.Skipped, "Akce nebyla provedena, protože selhaly předchozí kontroly: " + string.Join(", ", failures));
                    continue;
                }

                try
                {
                    action.SetStatus(DtmExportActionStatus.Running, "Akce právě probíhá.");
                    await render();
                    var result = action.Execute();
                    action.SetStatus(result.Status, result.Detail);
                    if (result.Status == DtmExportActionStatus.Failed) failures.Add(action.Name);
                }
                catch (Exception ex)
                {
                    action.SetStatus(DtmExportActionStatus.Failed, ex.Message);
                    failures.Add(action.Name);
                }
            }
        }

        static DtmExportActionResult DescribeValidation(DtmValidationResult validation)
        {
            var rules = Flatten(validation.Rules).ToArray();
            var errors = rules.Sum(r => r.Issues.Count(i => !i.IsWarning));
            var warnings = rules.Sum(r => r.Issues.Count(i => i.IsWarning));
            var details = new List<string> { $"Chyby: {errors}, varování: {warnings}." };
            foreach (var rule in rules)
            {
                if (!string.IsNullOrEmpty(rule.Note)) details.Add($"{rule.Code} {rule.Name}: {rule.Note}");
                foreach (var issue in rule.Issues)
                    details.Add($"{rule.Code} {rule.Name} — {(issue.IsWarning ? "Varování" : "Chyba")}: {issue}");
            }
            var status = validation.HasErrors ? DtmExportActionStatus.Failed
                : rules.Any(r => r.Status == DtmRuleStatus.Warning) ? DtmExportActionStatus.Warning
                : DtmExportActionStatus.Passed;
            return new DtmExportActionResult(status, string.Join("\n", details));
        }

        static IEnumerable<DtmRuleResult> Flatten(IEnumerable<DtmRuleResult> rules)
        {
            foreach (var rule in rules)
            {
                yield return rule;
                foreach (var child in Flatten(rule.SubResults)) yield return child;
            }
        }
    }
}
