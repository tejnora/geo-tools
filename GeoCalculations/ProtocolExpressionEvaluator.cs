using System;
using System.Globalization;
using GeoCalculations.Protocol;

namespace GeoCalculations
{
    public struct Variant
    {
        public enum VariantTypes
        {
            None,
            Double,
            Int,
            Bool,
            String
        }

        readonly VariantTypes _type;
        readonly double _doubleValue;
        readonly int _intValue;
        readonly string _stringValue;

        public VariantTypes VariantType
        {
            get { return _type; }
        }

        public int IntValue
        {
            get { return _intValue; }
        }

        public double DoubleValue
        {
            get { return _doubleValue; }
        }

        public bool BoolValue
        {
            get { return _intValue != 0; }
        }

        public string StringValue
        {
            get { return _stringValue; }
        }

        public Variant(double doubleValue)
        {
            _type = VariantTypes.Double;
            _doubleValue = doubleValue;
            _intValue = 0;
            _stringValue = string.Empty;
        }

        public Variant(bool boolValue)
        {
            _type = VariantTypes.Bool;
            _doubleValue = 0;
            _intValue = boolValue ? 1 : 0;
            _stringValue = string.Empty;
        }

        public Variant(int intValue)
        {
            _type = VariantTypes.Int;
            _doubleValue = 0;
            _intValue = intValue;
            _stringValue = string.Empty;
        }

        public Variant(string stringValue)
        {
            _type = VariantTypes.String;
            _doubleValue = 0;
            _intValue = 0;
            _stringValue = stringValue;
        }

        static VariantTypes ResolveType(Variant a, Variant b)
        {
            if (a._type == VariantTypes.Bool || b._type == VariantTypes.Bool)
                throw new ArgumentException();
            switch (a._type)
            {
                case VariantTypes.Double:
                    return VariantTypes.Double;
                case VariantTypes.Int:
                    {
                        switch (b._type)
                        {
                            case VariantTypes.Double:
                                return VariantTypes.Double;
                            case VariantTypes.Int:
                                return VariantTypes.Int;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        double ConvertToDouble()
        {
            switch (_type)
            {
                case VariantTypes.Double:
                    return _doubleValue;
                case VariantTypes.Int:
                    return _intValue;
                default:
                    throw new ArgumentOutOfRangeException(_type.ToString());
            }
        }

        int ConvertToInt()
        {
            switch (_type)
            {
                case VariantTypes.Double:
                    return (int)_doubleValue;
                case VariantTypes.Int:
                    return _intValue;
                default:
                    throw new ArgumentOutOfRangeException(_type.ToString());
            }

        }

        public static Variant operator +(Variant a, Variant b)
        {
            var type = ResolveType(a, b);
            switch (type)
            {
                case VariantTypes.Double:
                    return new Variant(a.ConvertToDouble() + b.ConvertToDouble());
                case VariantTypes.Int:
                    return new Variant(a.ConvertToInt() + b.ConvertToInt());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Variant operator -(Variant a, Variant b)
        {
            var type = ResolveType(a, b);
            switch (type)
            {
                case VariantTypes.Double:
                    return new Variant(a.ConvertToDouble() - b.ConvertToDouble());
                case VariantTypes.Int:
                    return new Variant(a.ConvertToInt() - b.ConvertToInt());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Variant operator *(Variant a, Variant b)
        {
            var type = ResolveType(a, b);
            switch (type)
            {
                case VariantTypes.Double:
                    return new Variant(a.ConvertToDouble() * b.ConvertToDouble());
                case VariantTypes.Int:
                    return new Variant(a.ConvertToInt() * b.ConvertToInt());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public static Variant operator /(Variant a, Variant b)
        {
            var type = ResolveType(a, b);
            switch (type)
            {
                case VariantTypes.Double:
                    return new Variant(a.ConvertToDouble() / b.ConvertToDouble());
                case VariantTypes.Int:
                    if (b.ConvertToInt() == 0)
                        throw new DivideByZeroException();
                    return new Variant(a.ConvertToInt() / b.ConvertToInt());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Variant operator <(Variant a, Variant b)
        {
            var type = ResolveType(a, b);
            switch (type)
            {
                case VariantTypes.Double:
                    return new Variant(a.ConvertToDouble() < b.ConvertToDouble());
                case VariantTypes.Int:
                    if (b.ConvertToInt() == 0)
                        throw new DivideByZeroException();
                    return new Variant(a.ConvertToInt() < b.ConvertToInt());
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public static Variant operator >(Variant a, Variant b)
        {
            var less = a <= b;
            return new Variant(!less.BoolValue);
        }

        public static Variant operator <=(Variant a, Variant b)
        {
            var less = a < b;
            if (less.BoolValue) return less;
            return a == b;
        }

        public static Variant operator >=(Variant a, Variant b)
        {
            return a <= b;
        }

        public static Variant operator ==(Variant a, Variant b)
        {
            var type = ResolveType(a, b);
            switch (type)
            {
                case VariantTypes.Double:
                    return new Variant(Math.Abs(a.ConvertToDouble() - b.ConvertToDouble()) < 0.0000000000000001);
                case VariantTypes.Int:
                    return new Variant(a.ConvertToInt() == b.ConvertToInt());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Variant operator !=(Variant a, Variant b)
        {
            return new Variant(!(a == b).BoolValue);
        }

        public Variant Negate()
        {
            switch (_type)
            {
                case VariantTypes.Double:
                    return new Variant(-_doubleValue);
                case VariantTypes.Int:
                    return new Variant(-_intValue);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool Equals(Variant other)
        {
            return Equals(other._type, _type) && other._doubleValue.Equals(_doubleValue) && other._intValue == _intValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Variant)) return false;
            return Equals((Variant)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _type.GetHashCode();
                result = (result * 397) ^ _doubleValue.GetHashCode();
                result = (result * 397) ^ _intValue;
                return result;
            }
        }
    }

    public class ProtocolExpressionEvaluator
    {
        public int Position { get { return _pos; } }

        int _pos;
        readonly string _expr;
        readonly ProtocolGenerator.Scope _scope;
        Variant _current;

        public ProtocolExpressionEvaluator(string expr, ProtocolGenerator.Scope scope)
        {
            _expr = expr;
            _scope = scope;
        }

        public Variant Evaluate()
        {
            _pos = 0;
            SkipSpaces();
            if (_pos == _expr.Length)
            {
                throw new Exception("missing anything");
            }
            ParseCondition();
            if (_pos != _expr.Length)
            {
                throw new Exception("unknown error");
            }
            return _current;
        }

        void SkipSpaces()
        {
            while (_pos < _expr.Length && _expr[_pos] == ' ') _pos++;
        }


        void ParseCondition()
        {
            PlusMinus();
            SkipSpaces();
            if (_expr.Length == _pos) return;
            if (_expr[_pos] == '<' || _expr[_pos] == '>' || _expr[_pos] == '=')
            {
                if (_current.VariantType == Variant.VariantTypes.None)
                    throw new Exception("Expected number.");
                var leftNode = _current;
                _current = new Variant();
                if (_expr[_pos] == '<')
                {
                    _pos++;
                    if (_expr[_pos] == '=')
                    {
                        _pos++;
                        PlusMinus();
                        _current = leftNode <= _current;
                        return;
                    }
                    PlusMinus();
                    _current = leftNode < _current;
                    return;
                }
                if (_expr[_pos] == '>')
                {
                    _pos++;
                    if (_expr[_pos] == '=')
                    {
                        _pos++;
                        PlusMinus();
                        _current = leftNode >= _current;
                        return;
                    }
                    PlusMinus();
                    _current = leftNode > _current;
                    return;
                }
                _pos++;
                if (_expr[_pos] == '=')
                {
                    _pos++;
                    PlusMinus();
                    _current = leftNode == _current;
                    return;
                }
                throw new Exception("Expected condition.");
            }
        }

        void PlusMinus()
        {
            MultiplyDivide();
            while (_pos != _expr.Length)
            {
                switch (_expr[_pos])
                {
                    case '+':
                        {
                            var backup = _current;
                            NextWithSkipSpaces();
                            MultiplyDivide();
                            _current = backup + _current;
                            break;
                        }
                    case '-':
                        {
                            var backup = _current;
                            NextWithSkipSpaces();
                            MultiplyDivide();
                            _current = backup - _current;
                            break;
                        }
                    default:
                        return;
                }
            }
        }

        void MultiplyDivide()
        {
            Unary();
            while (_pos != _expr.Length)
            {
                switch (_expr[_pos])
                {
                    case '*':
                        {
                            var backup = _current;
                            NextWithSkipSpaces();
                            Unary();
                            _current = backup * _current;
                            break;
                        }
                    case '/':
                        {
                            var backup = _current;
                            NextWithSkipSpaces();
                            Unary();
                            _current = backup / _current;
                            break;
                        }
                    default:
                        return;
                }
            }
        }

        void NextWithSkipSpaces()
        {
            _pos++;
            SkipSpaces();
        }

        void Unary()
        {
            switch (_expr[_pos])
            {
                case '+':
                    NextWithSkipSpaces();
                    if (_pos == _expr.Length) throw new Exception("number or '(' expected");
                    Parenthesis();
                    break;
                case '-':
                    NextWithSkipSpaces();
                    if (_pos == _expr.Length) throw new Exception("number or '(' expected");
                    Parenthesis();
                    _current = _current.Negate();
                    break;
                default:
                    Parenthesis();
                    break;
            }
        }

        void Parenthesis()
        {
            if (_expr[_pos] == '(')
            {
                _pos++;
                SkipSpaces();
                PlusMinus();
                if (_pos == _expr.Length || _expr[_pos] != ')')
                {
                    throw new Exception("')' expected");
                }
                _pos++;
                SkipSpaces();
            }
            else
            {
                _current = ParseVariant();
                SkipSpaces();
            }
        }

        Variant ParseVariant()
        {
            var p = _pos;
            var variant = ParseDouble();
            if (variant.VariantType != Variant.VariantTypes.None) return variant;
            _pos = p;
            variant = ParseInt();
            if (variant.VariantType != Variant.VariantTypes.None) return variant;
            variant = ParseBool();
            if (variant.VariantType != Variant.VariantTypes.None) return variant;
            variant = ParserVariable();
            if (variant.VariantType != Variant.VariantTypes.None) return variant;
            throw new Exception("expecte value");
        }

        Variant ParseInt()
        {
            var p = _pos;
            while (p < _expr.Length && _expr[p] >= '0' && _expr[p] <= '9') p++;
            if (p == _pos) return new Variant();
            var res = new Variant(Convert.ToInt32(_expr.Substring(_pos, p - _pos)));
            _pos = p;
            return res;
        }

        Variant ParseBool()
        {
            if (_pos + 4 > _expr.Length)
                return new Variant();
            if (_expr[_pos] == 't' && _expr[_pos + 1] == 'r' && _expr[_pos + 2] == 'u' && _expr[_pos + 3] == 'e')
            {
                _pos += 4;
                return new Variant(true);
            }
            if (_pos + 5 > _expr.Length)
                return new Variant();
            if (_expr[_pos] == 'f' && _expr[_pos + 1] == 'a' && _expr[_pos + 2] == 'l' && _expr[_pos + 3] == 's' && _expr[_pos + 4] == 'e')
            {
                _pos += 5;
                return new Variant(false);
            }
            return new Variant();
        }

        Variant ParseDouble()
        {
            var p = _pos;
            while (p < _expr.Length && _expr[p] >= '0' && _expr[p] <= '9') p++;
            if (_expr.Length == p || _expr[p] != '.')
                return new Variant();
            p++;
            while (p < _expr.Length && _expr[p] >= '0' && _expr[p] <= '9') p++;
            var doubleValue = _expr.Substring(_pos, p - _pos);
            var res = new Variant(Double.Parse(doubleValue, CultureInfo.InvariantCulture));
            _pos = p;
            return res;
        }

        Variant ParserVariable()
        {
            var p = _pos;
            while (p < _expr.Length && (char.IsLetterOrDigit(_expr[p]) || _expr[p] == '.')) p++;
            if (p == _pos) return new Variant();
            var varialble = _expr.Substring(_pos, p - _pos);
            var result = _scope.Evaluate(varialble);
            if (result == null) return new Variant();
            var type = result.Data.GetType();
            _pos = p;
            switch (type.Name)
            {
                case "Double":
                    return new Variant((double)result.Data);
                case "Int32":
                case "Int64":
                    return new Variant((int)result.Data);
                case "Boolean":
                    return new Variant((bool)result.Data);
                case "String":
                    return new Variant((string)result.Data);
                default:
                    return new Variant();
            }
        }
    }
}
