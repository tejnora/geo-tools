using System;
using GeoBase.Localization;

namespace GeoHelper.FileParses
{
    internal class ParseException : Exception
    {
        readonly string _message = string.Empty;

        public ParseException()
        {
        }

        public ParseException(string ex)
        {
            _message = ex;
        }

        public override string Message
        {
            get { return _message; }
        }
    }

    internal class ParserMoreColumnItems : ParseException
    {
        readonly uint _column;
        readonly uint _row;

        public ParserMoreColumnItems(uint row, uint column)
        {
            _row = row;
            _column = column;
        }

        public override string ToString()
        {
            LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
            var msg = (string) dictionary.Translate("256", "Message", "Parser Error", typeof (string));
            msg = msg + string.Format("({0},{1})", _row, _column);
            return msg;
        }
    }

    internal class ParserInvalidRow : ParseException
    {
        readonly uint _row;

        public ParserInvalidRow(uint row)
        {
            _row = row;
        }

        public override string ToString()
        {
            LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
            return string.Format("({0})", _row) +
                   (string) dictionary.Translate("257", "Message", "Parser Error", typeof (string));
        }
    }

    internal class ParserPatternUnexpectItem : ParseException
    {
        readonly int _column;
        readonly int _row;

        public ParserPatternUnexpectItem(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public override string ToString()
        {
            LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
            var msg = (string) dictionary.Translate("248", "Message", "Parser Error", typeof (string));
            return msg + string.Format("({0},{1})", _row, _column);
        }
    }

    internal class ParserUnexpectLine : ParseException
    {
        readonly int _column;
        readonly int _row;

        public ParserUnexpectLine(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public override string ToString()
        {
            LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
            var msg = (string) dictionary.Translate("249", "Message", "Parser Error", typeof (string));
            return msg + string.Format("({0},{1})", _row, _column);
        }
    }
}