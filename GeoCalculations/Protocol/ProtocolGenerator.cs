using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using GeoBase.Compiler.Parser;
using GeoBase.Localization;
using GeoCalculations.Deviations;

namespace GeoCalculations.Protocol
{
    public class ProtocolGenerator
    {
        private readonly IDictionary<ProtocolPluginTypes, IProtocolPlugin> _plugins = new Dictionary<ProtocolPluginTypes, IProtocolPlugin>();

        public void RegisterPlugin(ProtocolPluginTypes type, IProtocolPlugin plugin)
        {
            _plugins[type] = plugin;
        }

        public abstract class StackItem
        {
            public List<StackItem> Children { get; private set; }
            public StackItem Parent { get; private set; }
            internal Scope Scope { get; private set; }
            readonly Dictionary<string, string> _attributes = new Dictionary<string, string>();

            protected StackItem()
            {
                Children = new List<StackItem>();
            }

            public void AddAttribute(string name, string value)
            {
                _attributes[name] = value;
            }

            public bool TryGetAttribute(string key, out string name)
            {
                return _attributes.TryGetValue(key, out  name);
            }

            public bool ContainsAttribte(string key)
            {
                return _attributes.ContainsKey(key);
            }

            public string GetAttributeValue(string key)
            {
                return _attributes[key];
            }

            public void AddChild(StackItem child)
            {
                if (child == null)
                {
                    Children = null;
                    return;
                }
                child.Parent = this;
                child.Scope = Scope;
                Children.Add(child);
            }

            public void SetScope(Scope scope)
            {
                Scope = scope;
            }

            public class StringGeneratorContext
            {
                public bool ProcessReturn { get; set; }
            }

            public abstract string GetFinalString(StringGeneratorContext contex);
        }

        public class StackItemAny : StackItem
        {
            public override string GetFinalString(StringGeneratorContext contex)
            {
                return Children.Aggregate(string.Empty, (current, child) =>
                    {
                        if (contex.ProcessReturn) return current;
                        return current + child.GetFinalString(contex);
                    });
            }
        }

        public class IfStatement : StackItemAny
        {

        }

        public class IfElseStatement : StackItemAny
        {
            public IfElseStatement()
            {
                IsTrue = true;
            }
            public bool IsTrue;
        }

        public class DeviationsStatement : StackItemAny
        {
            public override string GetFinalString(StringGeneratorContext contex)
            {
                string variable;
                TryGetAttribute("Variable", out variable);
                var result = Scope.Evaluate(variable).Data as CalculationDeviation;
                if (result == null)
                    throw new ArgumentException("Deviation variable has invalid value. Must be CalculationDeviation variable");
                string name;
                TryGetAttribute("Name", out name);
                name += "Deviation";
                var resultString = new StringBuilder();
                string header;
                if (TryGetAttribute("Header", out header))
                {
                    resultString.Append(header + Environment.NewLine);
                    resultString.Append("".PadLeft(header.Length, '-'));
                    resultString.Append(Environment.NewLine);
                }
                foreach (var devation in result.Deviations.Where(devation => devation.Exceeded))
                {
                    if (devation.GetType().Name == name) continue;
                    resultString.Append(devation + "\n");
                }
                if (result.Deviations.Count != 0)
                {
                    var testDeviations = result.Deviations.Where(n => n.GetType().Name == name);
                    foreach (var deviation in testDeviations)
                    {
                        resultString.Append(deviation.ToString() + '\n');
                    }
                    resultString.Append('\n');
                    if (result.HasError)
                        resultString.Append(LanguageDictionary.Current.Translate<string>("Deviations.DeviationFailed", "Text"));
                    else
                        resultString.Append(LanguageDictionary.Current.Translate<string>("Deviations.DeviationSuccess", "Text"));
                }
                return resultString.ToString();
            }
        }

        public class TableFormatContext
        {
            public enum Stages
            {
                Preprocess,
                Result
            }
            public List<int> MaxColumnWidths { get; private set; }
            public int CurrentColumn { get; set; }
            public Stages Stage { get; set; }
            readonly List<string> _lines = new List<string>();
            string _currentLine;
            public int TableCellSpace { get; set; }
            public bool LastCell { get; set; }
            public TableFormatContext()
            {
                MaxColumnWidths = new List<int>();
            }

            public void AppendToLine(string text)
            {
                if (LastCell)
                    _currentLine += text;
                else
                    _currentLine += text.PadRight(text.Length + TableCellSpace);
            }

            public void FlushLine()
            {
                _lines.Add(_currentLine);
                _currentLine = string.Empty;
            }

            public void Underline(char value)
            {
                if (_lines.Count == 0) return;
                var lastLineLength = _lines.Last().Length;
                _lines.Add("".PadLeft(lastLineLength, value));
            }

            public string GetTableText()
            {
                var result = new StringBuilder();
                foreach (var line in _lines)
                {
                    result.Append(line + '\n');
                }
                return result.ToString();
            }
        }

        public abstract class TableDataBase : StackItemAny
        {
            public virtual void FormatTable(TableFormatContext context)
            {
                foreach (var stackItem in Children)
                {
                    ((TableDataBase)stackItem).FormatTable(context);
                }
                if (context.Stage != TableFormatContext.Stages.Result) return;
                string attrValue;
                if (TryGetAttribute("LastLine", out attrValue))
                {
                    context.Underline(attrValue[0]);
                }
            }
        }

        class TableDataItem : TableDataBase
        {
            TableFormatContext _formatContext;
            public void PreprocessFormatTable()
            {
                _formatContext = new TableFormatContext();
                var cellSpace = string.Empty;
                if (TryGetAttribute("CellSpace", out cellSpace))
                {
                    _formatContext.TableCellSpace = int.Parse(cellSpace);
                }
                _formatContext.Stage = TableFormatContext.Stages.Preprocess;
                base.FormatTable(_formatContext);
            }

            public override string GetFinalString(StringGeneratorContext contex)
            {
                _formatContext.Stage = TableFormatContext.Stages.Result;
                base.FormatTable(_formatContext);
                return _formatContext.GetTableText();
            }
        }

        class TableRowItem : TableDataBase
        {
            public bool Repeted { get; set; }

            public override void FormatTable(TableFormatContext context)
            {
                var backupLastCell = context.LastCell;
                for (var i = 0; i < Children.Count; i++)
                {
                    context.LastCell = i == Children.Count - 1;
                    ((TableDataBase)Children[i]).FormatTable(context);
                }
                context.LastCell = backupLastCell;
                string attrValue;
                if (context.Stage == TableFormatContext.Stages.Result && !Repeted)
                {
                    context.FlushLine();
                    if (TryGetAttribute("Line", out attrValue))
                    {
                        context.Underline(attrValue[0]);
                    }
                }
                context.CurrentColumn = 0;
            }

            public override string GetFinalString(StringGeneratorContext contex)
            {
                if (Repeted)
                {
                    var result = string.Empty;
                    foreach (var child in Children)
                    {
                        result += child.GetFinalString(contex);
                    }
                    return result;
                }
                return base.GetFinalString(contex) + '\n';
            }
        }


        public class TableAdditionalMessageAfterRow : TableDataBase
        {
            readonly string _additionalFunctionGetter;
            string result;

            public TableAdditionalMessageAfterRow(string additionalFunctionGetter)
            {
                _additionalFunctionGetter = additionalFunctionGetter;
            }

            public override void FormatTable(TableFormatContext context)
            {
                switch (context.Stage)
                {
                    case TableFormatContext.Stages.Preprocess:
                        result = Scope.EvaluateFunction<string>(_additionalFunctionGetter);
                        break;
                    case TableFormatContext.Stages.Result:
                        if (string.IsNullOrEmpty(result)) return;
                        var lines = result.Split('\n');
                        var length = lines.Length;
                        while (length >= 0)
                        {
                            if (!string.IsNullOrEmpty(lines[length - 1]))
                                break;
                            --length;
                        }
                        for (var i = 0; i < length; i++)
                        {
                            context.AppendToLine(lines[i]);
                            context.FlushLine();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public override string GetFinalString(StringGeneratorContext contex)
            {
                return result;
            }
        }

        public class TableDataColumn : TableDataBase
        {
            public override void FormatTable(TableFormatContext context)
            {
                foreach (var stackItem in Children)
                {
                    var cellContent = stackItem.GetFinalString(null);
                    switch (context.Stage)
                    {
                        case TableFormatContext.Stages.Preprocess:
                            if (context.CurrentColumn < context.MaxColumnWidths.Count)
                            {
                                var max = Math.Max(context.MaxColumnWidths[context.CurrentColumn], cellContent.Length);
                                context.MaxColumnWidths[context.CurrentColumn] = max;
                            }
                            else
                                context.MaxColumnWidths.Add(cellContent.Length);
                            break;
                        case TableFormatContext.Stages.Result:
                            var length = context.MaxColumnWidths[context.CurrentColumn];
                            context.AppendToLine(cellContent.PadLeft(length, ' '));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    context.CurrentColumn++;
                }
            }
        }

        public class TableHeaderItem : TableDataColumn
        {

        }

        class FieldNameItem : StackItem
        {
            readonly Scope.EvaluatedResult _result;
            readonly IDictionary<ProtocolPluginTypes, IProtocolPlugin> _plugins;

            public FieldNameItem(Scope.EvaluatedResult result, IDictionary<ProtocolPluginTypes, IProtocolPlugin> plugins)
            {
                _result = result;
                _plugins = plugins;
            }

            public override string GetFinalString(StringGeneratorContext contex)
            {
                return _result.GetString(_plugins);
            }
        }

        class EndOfLineItem : StackItem
        {
            public override string GetFinalString(StringGeneratorContext contex)
            {
                return HasTableParent() ? "" : "\n";
            }

            bool HasTableParent()
            {
                var parent = Parent;
                do
                {
                    if (parent is TableDataItem)
                        return true;
                    parent = parent.Parent;
                } while (parent != null);
                return false;
            }
        }

        class TextItem : StackItem
        {
            readonly string _text;

            public TextItem(string text)
            {
                _text = text;
            }

            public override string GetFinalString(StringGeneratorContext contex)
            {
                return _text;
            }
        }

        class LineItem : StackItem
        {
            public override string GetFinalString(StringGeneratorContext contex)
            {
                var result = string.Empty;
                if (Children.Count == 0) return result;
                result += Children[0].GetFinalString(contex);
                if (Children[0] is SpaceItem)
                {
                    if (Children.Count == 1)
                        return result;
                }
                else
                {
                    if (Children.Count == 1)
                        return result.Trim(' ');
                    result = result.TrimStart(' ');
                }
                for (var i = 1; i < Children.Count; i++)
                {
                    result += Children[i].GetFinalString(contex);
                }
                if (Children.Last() is SpaceItem)
                {
                    return result;
                }
                return result.TrimEnd(' ');
            }
        }

        class SpaceItem : StackItem
        {
            public override string GetFinalString(StringGeneratorContext contex)
            {
                return " ";
            }
        }

        class ReturnItem : StackItem
        {
            public override string GetFinalString(StringGeneratorContext contex)
            {
                contex.ProcessReturn = true;
                return "";
            }
        }

        public abstract class Scope
        {
            protected readonly Scope _parentRootScope;
            protected readonly ScanProtocolResultData.PropertyContext _properties;
            protected readonly object _data;

            public class EvaluatedResult
            {
                public object Data;
                public ScanProtocolResultData.PropertyContext Properties;
                public string GetString(IDictionary<ProtocolPluginTypes, IProtocolPlugin> plugins)
                {
                    var type = Data.GetType();
                    var properties = Properties;
                    if (type.Name == "Double")
                    {
                        var dValue = (double)Data;
                        if (double.IsNaN(dValue)) return "";
                        IProtocolPlugin plugin;
                        if (!plugins.TryGetValue(ProtocolPluginTypes.Units, out plugin))
                        {
                            if (double.IsNaN(dValue) || dValue < 0.00000000001)
                                return "0";
                            return Data.ToString();
                        }
                        var unitPlugin = (ProtocolUnitsPlugin)plugin;
                        switch (properties.ValueType)
                        {
                            case ProtocolPropertyValueTypeAttribute.Types.Heigth:
                                return unitPlugin.HightToString(dValue);
                            case ProtocolPropertyValueTypeAttribute.Types.Cordinate:
                                return unitPlugin.CoordinateToString(dValue);
                            case ProtocolPropertyValueTypeAttribute.Types.Distance:
                                return unitPlugin.DistanceToString(dValue);
                            case ProtocolPropertyValueTypeAttribute.Types.Scale:
                                return unitPlugin.ScaleToString(dValue);
                            case ProtocolPropertyValueTypeAttribute.Types.Angle:
                                return unitPlugin.AngleToString(dValue);
                            case ProtocolPropertyValueTypeAttribute.Types.Area:
                                return unitPlugin.AreaToString(dValue);
                            default:
                                return string.Format(CultureInfo.InvariantCulture, "{0:0.000}", Data);
                        }
                    }
                    return Data.ToString();
                }
            }

            protected Scope(Scope parentRootScope, ScanProtocolResultData.PropertyContext properties, object data)
            {
                _parentRootScope = parentRootScope;
                _properties = properties;
                _data = data;
            }

            protected virtual EvaluatedResult EvaluateField(string[] fieldName, int fieldNameIdx, ScanProtocolResultData.PropertyContext properties, object data)
            {
                if (fieldNameIdx < fieldName.Length)
                {
                    foreach (var property in properties.Children)
                    {
                        if (property.Name != fieldName[fieldNameIdx]) continue;
                        if (property.PropertyType == ScanProtocolResultData.PropertyTypes.ArrayNode || property.PropertyType == ScanProtocolResultData.PropertyTypes.Method) continue;
                        data = property.PropertyInfo.GetValue(data, null);
                        return EvaluateField(fieldName, fieldNameIdx + 1, property, data);
                    }
                    return null;
                }
                return new EvaluatedResult { Data = data, Properties = properties };
            }

            public EvaluatedResult Evaluate(string fieldName)
            {
                fieldName = fieldName.Trim(' ');
                if (fieldName.Length == 0)
                    throw new ArgumentOutOfRangeException(string.Format("Invalid field name.{0}", fieldName));
                if (fieldName[0] == '{' && fieldName[1] == '{')
                {
                    if (fieldName[fieldName.Length - 1] != '}' || fieldName[fieldName.Length - 2] != '}' || fieldName.Length <= 4)
                        throw new ArgumentOutOfRangeException(string.Format("Invalid field name.{0}", fieldName));
                    var conditionValue = fieldName.Substring(2, fieldName.Length - 4);
                    var eval = new ProtocolExpressionEvaluator(conditionValue, this);
                    var result = eval.Evaluate();
                    switch (result.VariantType)
                    {
                        case Variant.VariantTypes.Double:
                            return new EvaluatedResult { Data = eval.Evaluate().DoubleValue, Properties = new ScanProtocolResultData.PropertyContext(typeof(double), string.Empty, ScanProtocolResultData.PropertyTypes.RuntimeEvaluated) }; return new EvaluatedResult { Data = eval.Evaluate().BoolValue, Properties = new ScanProtocolResultData.PropertyContext(typeof(int), string.Empty, ScanProtocolResultData.PropertyTypes.RuntimeEvaluated) };
                        case Variant.VariantTypes.Int:
                            return new EvaluatedResult { Data = eval.Evaluate().IntValue, Properties = new ScanProtocolResultData.PropertyContext(typeof(int), string.Empty, ScanProtocolResultData.PropertyTypes.RuntimeEvaluated) };
                        case Variant.VariantTypes.Bool:
                            return new EvaluatedResult { Data = eval.Evaluate().BoolValue, Properties = new ScanProtocolResultData.PropertyContext(typeof(bool), string.Empty, ScanProtocolResultData.PropertyTypes.RuntimeEvaluated) };
                        case Variant.VariantTypes.String:
                            return new EvaluatedResult { Data = eval.Evaluate().StringValue, Properties = new ScanProtocolResultData.PropertyContext(typeof(string), string.Empty, ScanProtocolResultData.PropertyTypes.RuntimeEvaluated) };
                        default:
                            throw new ArgumentOutOfRangeException(string.Format("Invalid condition value.{0}", fieldName));
                    }
                }
                var splittedVariable = fieldName.Split('.');
                var value = EvaluateField(splittedVariable, 0, _properties, _data);
                if (value != null) return value;
                if (_parentRootScope == null)
                    throw new ArgumentOutOfRangeException(string.Format("Invalid field name.{0}", fieldName));
                return _parentRootScope.Evaluate(fieldName);
            }

            public TRes EvaluateFunction<TRes>(string functionGetter)
            {
                var scope = this;
                do
                {
                    foreach (var property in scope._properties.Children)
                    {
                        if (property.Name != functionGetter) continue;
                        if (property.PropertyType != ScanProtocolResultData.PropertyTypes.Method) continue;
                        return CallMethod<TRes>(property.MethodInfo, scope._data);
                    }
                    scope = scope._parentRootScope;
                } while (scope != null);
                throw new ArgumentException(string.Format("Method {0} does not exist.", functionGetter));
            }

            protected virtual TRes CallMethod<TRes>(MethodInfo methodInfo, object data)
            {
                throw new NotImplementedException("CallMethod");
            }
        }

        class RootScope : Scope
        {
            public RootScope(ScanProtocolResultData.PropertyContext properties, object data)
                : base(null, properties, data)
            {
            }
        }

        class SingleRowScope : Scope
        {
            public SingleRowScope(Scope parentRootScope)
                : base(parentRootScope, new ScanProtocolResultData.PropertyContext(), null)
            {
            }
            protected override TRes CallMethod<TRes>(MethodInfo methodInfo, object data)
            {
                return (TRes)methodInfo.Invoke(data, new object[] { string.Empty, -1 });
            }
        }

        class RowScope : Scope
        {
            readonly int _rowId;
            readonly string _arrayName;
            public RowScope(Scope parentScope, ScanProtocolResultData.PropertyContext properties, int rowId, string arrayName, object data)
                : base(parentScope, properties, data)
            {
                _rowId = rowId;
                _arrayName = arrayName;
            }

            protected override EvaluatedResult EvaluateField(string[] fieldName, int fieldNameIdx, ScanProtocolResultData.PropertyContext properties, object data)
            {
                if (fieldNameIdx == 0)
                {
                    var array = _arrayName.Split('.');
                    fieldNameIdx = array.Length;
                    foreach (var property in properties.Children)
                    {
                        if (property.Name == fieldName[fieldNameIdx])
                        {
                            if (fieldNameIdx + 1 == fieldName.Length && ScanProtocolResultData.IsSimpleEnumerableType(data.GetType()))
                            {
                                switch (property.Name)
                                {
                                    case ScanProtocolResultData.RepeatedCountVariableName:
                                        return new EvaluatedResult { Data = _rowId, Properties = property };
                                    case ScanProtocolResultData.RepeatedItemVariableName:
                                        return new EvaluatedResult { Data = ((IList)data)[_rowId], Properties = property };
                                    default:
                                        throw new ArgumentException("Variable '' does not exist.", string.Join(".", fieldName));
                                }
                            }
                            data = property.PropertyInfo.GetValue(((IList)data)[_rowId], null);
                            return base.EvaluateField(fieldName, fieldNameIdx + 1, property, data);
                        }
                    }
                    return null;
                }
                return base.EvaluateField(fieldName, fieldNameIdx, properties, data);
            }

            protected override TRes CallMethod<TRes>(MethodInfo methodInfo, object data)
            {
                return (TRes)methodInfo.Invoke(data, new object[] { _arrayName, _rowId });
            }

        }

        public string Eval(Node node, ScanProtocolResultData.PropertyContext properties, object data)
        {
            var main = new StackItemAny();
            main.SetScope(new RootScope(properties, data));
            EvalInternal(node, main);
            return main.GetFinalString(new StackItem.StringGeneratorContext());
        }

        void EvalInternal(Node node, StackItem stackItem)
        {
            switch (node.Label)
            {
                case "Table":
                    {
                        var table = new TableDataItem();
                        stackItem.AddChild(table);
                        EvalInternal(node.Nodes[0], table);
                        EvalInternal(node.Nodes[1], table);
                        table.PreprocessFormatTable();
                    } break;
                case "Deviations":
                    {
                        var deviations = new DeviationsStatement();
                        EvalInternal(node.Nodes[0], deviations);
                        if (!deviations.ContainsAttribte("Variable"))
                            throw new ArgumentException("Deviation has not set attribute 'Variable'.");
                        if (!deviations.ContainsAttribte("Name"))
                            throw new ArgumentException("Deviation has not set attribute 'Name'.");
                        stackItem.AddChild(deviations);
                    } break;
                case "If":
                    {
                        var ifStatement = new IfStatement();
                        EvalInternal(node.Nodes[0], ifStatement);
                        string condition;
                        if (ifStatement.TryGetAttribute("Variable", out condition))
                        {
                            var result = stackItem.Scope.Evaluate(condition);
                            if (result.Data.GetType() != typeof(bool))
                            {
                                throw new ArgumentException(string.Format("Condition type '{0}'is not supported. Only bool variables.", result.Data.GetType()));
                            }
                            if (!(bool)result.Data)
                            {
                                ((IfElseStatement)stackItem).IsTrue = false;
                                return;
                            }
                        }
                        stackItem.AddChild(ifStatement);
                        for (var i = 1; i < node.Nodes.Count; i++)
                        {
                            EvalInternal(node.Nodes[i], ifStatement);
                        }
                    } break;
                case "IfElse":
                    {
                        var ifelseStatement = new IfElseStatement();
                        stackItem.AddChild(ifelseStatement);
                        EvalInternal(node.Nodes[0], ifelseStatement);
                        if (ifelseStatement.IsTrue || node.Nodes.Count == 1) return;
                        EvalInternal(node.Nodes[1], stackItem);
                    } break;
                case "TableRow":
                    {
                        var item = new TableRowItem();
                        stackItem.AddChild(item);
                        EvalInternal(node.Nodes[0], item);
                        string attr;
                        var callAdditionalMessageAfterRow = item.ContainsAttribte("AdditionalMessageAfterRow");
                        if (item.TryGetAttribute("Variable", out attr))
                        {
                            item.Repeted = true;
                            var result = stackItem.Scope.Evaluate(attr);
                            if (!ScanProtocolResultData.GetIsListType(result.Data.GetType()))
                                throw new ArgumentException(string.Format("Type '{0}'is not supported.", result.Data.GetType()));
                            var dynamicRows = result.Data as IList;
                            for (var i = 0; i < dynamicRows.Count; i++)
                            {
                                var repeatedRow = new TableRowItem();
                                item.AddChild(repeatedRow);
                                var rowScope = new RowScope(stackItem.Scope, result.Properties, i, attr, dynamicRows);
                                repeatedRow.SetScope(rowScope);
                                EvalInternal(node.Nodes[1], repeatedRow);
                                if (callAdditionalMessageAfterRow)
                                {
                                    var additionalData = new TableAdditionalMessageAfterRow(item.GetAttributeValue("AdditionalMessageAfterRow"));
                                    item.AddChild(additionalData);
                                    additionalData.SetScope(rowScope);
                                }
                            }
                        }
                        else
                        {
                            EvalInternal(node.Nodes[1], item);
                            if (callAdditionalMessageAfterRow)
                            {
                                var additionalData = new TableAdditionalMessageAfterRow(item.GetAttributeValue("AdditionalMessageAfterRow"));
                                item.AddChild(additionalData);
                                additionalData.SetScope(new SingleRowScope(stackItem.Scope));
                            }
                        }
                    } break;
                case "TableHeaderColumn":
                    {
                        var item = new TableHeaderItem();
                        stackItem.AddChild(item);
                        EvalInternal(node.Nodes[0], item);
                        EvalInternal(node.Nodes[1], item);
                    } break;
                case "TableDataColumn":
                    {
                        var item = new TableDataColumn();
                        stackItem.AddChild(item);
                        EvalInternal(node.Nodes[0], item);
                        EvalInternal(node.Nodes[1], item);
                    } break;
                case "Terms":
                case "CellContent":
                case "ParseAttributes":
                case "TableColumns":
                case "TableRows":
                case "Else":
                    foreach (var curNode in node.Nodes)
                    {
                        EvalInternal(curNode, stackItem);
                    }
                    break;
                case "ParseAttribute":
                    stackItem.AddAttribute(node.Nodes[0].Text, node.Nodes[1].Text);
                    break;
                case "ExpressionValue":
                    stackItem.AddChild(new FieldNameItem(stackItem.Scope.Evaluate("{{" + node.Text + "}}"), _plugins));
                    break;
                case "Text":
                    stackItem.AddChild(new TextItem(node.Text));
                    break;
                case "EndOfLine":
                    stackItem.AddChild(new EndOfLineItem());
                    break;
                case "Line":
                    {
                        var item = new LineItem();
                        stackItem.AddChild(item);
                        foreach (var curNode in node.Nodes)
                        {
                            EvalInternal(curNode, item);
                        }
                    } break;
                case "Space":
                    {
                        stackItem.AddChild(new SpaceItem());
                    } break;
                case "Return":
                    stackItem.AddChild(new ReturnItem());
                    break;
                default:
                    throw new NotImplementedException("Internal error");
            }
        }
    }
}
