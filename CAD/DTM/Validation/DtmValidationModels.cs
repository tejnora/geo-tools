using System;
using System.Collections.Generic;
using System.Linq;

namespace CAD.DTM.Validation
{
    public class DtmValidationSettings
    {
        public static DtmValidationSettings Default { get; } = new DtmValidationSettings();

        // Hodnoty jsou v souradnicich JVF (zaporne, 3. kvadrant S-JTSK).
        public double MinX { get; set; } = -905000;
        public double MaxX { get; set; } = -430000;
        public double MinY { get; set; } = -1230000;
        public double MaxY { get; set; } = -930000;
        public double MinZ { get; set; } = 1;
        public double MaxZ { get; set; } = 1620;
        public double MinZDefinicniBod { get; set; } = 0;
        public double PrecisionTolerance { get; set; } = 1e-4;
        public double OnBoundaryTolerance { get; set; } = 0.005;
        public string ZpsContentPart { get; set; } = "ZPS";

        public double MinPointDistance { get; set; } = 0.05;
        public double MinSegmentLength { get; set; } = 0.05;
        public double MinDuplicateZDifference { get; set; } = 0.12;
        public double MinPointToLineDistance { get; set; } = 0.05;
        public double MinAreaSize { get; set; } = 0.05;
        public double GeometryTolerance { get; set; } = 0.001;
        public int MaxAccuracyClassForInsert { get; set; } = 3;
        public DateTime MinDate { get; set; } = new DateTime(1900, 1, 1);

        public ISet<string> PodrobnyBodGroups { get; } = new HashSet<string> { "PodrobnyBodZPS" };
        public ISet<string> PointsExcludedFromTopology { get; } = new HashSet<string> { "VyskovyBodNaTerenu", "IdentickyBod" };
        // Konstrukcni linie vstupujici do tvorby odvozenych objektu ZPS.
        public ISet<string> ConstructionLineGroups { get; } = new HashSet<string>
        {
            "HraniceBudovy", "HraniceZdi", "Plot", "StavebneUpravenyVjezdNaPozemek", "HraniceDopravniStavbyPlochy",
            "HranicePrirodnihoPoloprirodnihoObjektu", "HraniceStavby", "HraniceSchodiste", "NeidentifikovanyObjekt", "ZedLinie"
        };
    }

    public enum DtmRuleStatus
    {
        Passed,
        Warning,
        Failed,
        Skipped
    }

    public class DtmValidationIssue
    {
        public DtmValidationIssue(string objectName, string message, bool isWarning = false)
        {
            IsWarning = isWarning;
            ObjectName = objectName;
            Message = message;
        }
        public bool IsWarning { get; }
        public string ObjectName { get; }
        public string Message { get; }
        public override string ToString() => string.IsNullOrEmpty(ObjectName) ? Message : $"{ObjectName}: {Message}";
    }

    public class DtmRuleResult
    {
        readonly List<DtmValidationIssue> _issues = new List<DtmValidationIssue>();

        public DtmRuleResult(string code, string name)
        {
            Code = code;
            Name = name;
        }
        public string Code { get; }
        public string Name { get; }
        public string Note { get; set; }
        public string Description { get; set; }
        public bool IsSkipped { get; set; }
        public IReadOnlyList<DtmValidationIssue> Issues => _issues;
        public List<DtmRuleResult> SubResults { get; } = new List<DtmRuleResult>();

        public void AddError(string objectName, string message) => _issues.Add(new DtmValidationIssue(objectName, message));
        public void AddWarning(string objectName, string message) => _issues.Add(new DtmValidationIssue(objectName, message, true));

        public DtmRuleStatus Status
        {
            get
            {
                if (IsSkipped) return DtmRuleStatus.Skipped;
                if (_issues.Any(i => !i.IsWarning) || SubResults.Any(s => s.Status == DtmRuleStatus.Failed)) return DtmRuleStatus.Failed;
                if (_issues.Count > 0 || SubResults.Any(s => s.Status == DtmRuleStatus.Warning)) return DtmRuleStatus.Warning;
                if (SubResults.Count > 0 && SubResults.All(s => s.Status == DtmRuleStatus.Skipped)) return DtmRuleStatus.Skipped;
                return DtmRuleStatus.Passed;
            }
        }
    }

    public class DtmValidationResult
    {
        public DtmValidationResult(IEnumerable<DtmRuleResult> rules)
        {
            Rules = rules.ToList();
        }
        public IReadOnlyList<DtmRuleResult> Rules { get; }
        public bool HasErrors => Rules.Any(r => r.Status == DtmRuleStatus.Failed);
    }
}
