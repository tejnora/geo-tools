using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GeoHelper.Converters;
using GeoHelper.Printing;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tabulky;

namespace GeoHelper.Tables.TablePrintingElements
{
    internal class TableCoordinatePrintSetting
        : TablePrintSettingBase
    {
        public TableCoordinatePrintSetting(PrintSetting printSetting, ColumnVisibility columnVisibility)
            : base(printSetting, columnVisibility)
        {
            Columns = new[]
                          {
                              new ColumnIfno("Číslo bodu", 20),
                              new ColumnIfno("Y", 15),
                              new ColumnIfno("X", 15),
                              new ColumnIfno("Z", 15),
                              new ColumnIfno("Kód kvality", 10),
                              new ColumnIfno("Description", 25),
                          };
            IsVisibleCislo = (columnVisibility & ColumnVisibility.Number) == ColumnVisibility.Number;
            IsVisiblePredcisli = (columnVisibility & ColumnVisibility.Prefix) == ColumnVisibility.Prefix;
            Columns[0].Visibility = (IsVisibleCislo || IsVisiblePredcisli);
            Columns[1].Visibility = ((columnVisibility & ColumnVisibility.CoordinateX) == ColumnVisibility.CoordinateX);
            Columns[2].Visibility = ((columnVisibility & ColumnVisibility.CoordinateY) == ColumnVisibility.CoordinateY);
            Columns[3].Visibility = ((columnVisibility & ColumnVisibility.CoordinateZ) == ColumnVisibility.CoordinateZ);
            Columns[4].Visibility = ((columnVisibility & ColumnVisibility.Quality) == ColumnVisibility.Quality);
            Columns[5].Visibility = ((columnVisibility & ColumnVisibility.Description) == ColumnVisibility.Description);
            RecalcTableColumnWidth();
        }
    }

    internal class CoordinateHeaderPageElemnt : PageElemnt
    {
        public CoordinateHeaderPageElemnt(CoordinateListInfoDialogContext coordinateListInfo, TableCoordinatePrintSetting ps,
                                  HeaderType headerType)
        {
            _coordinateListInfo = coordinateListInfo;
            _ps = ps;
            _headerType = headerType;
        }

        readonly HeaderType _headerType;
        readonly TableCoordinatePrintSetting _ps;
        readonly CoordinateListInfoDialogContext _coordinateListInfo;

        public override void OnRender(DrawingContext drawingContext, ref Point point, double width)
        {
            //left and right line
            if (_headerType == HeaderType.FirstHeader)
            {
                drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X, point.Y + GetSize())));
                drawingContext.DrawLine(Pen, toPixel(new Point(point.X + width, point.Y)),
                                        toPixel(new Point(point.X + width, point.Y + GetSize())));
                drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
                var text = MakeText("SEZNAM SOUŘADNIC", _ps.VyskaFontu + 2);
                text.SetFontWeight(FontWeights.Bold);
                var xFontIndent = (width - toMM(text.Width)) / 2;
                var yFontIndet = (_ps.VyskaHlavnihoNadpisu - toMM(text.Height)) / 2;
                drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent, point.Y + yFontIndet)));
                point.Y += _ps.VyskaHlavnihoNadpisu;
                drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
                //zacatek hlavicka
                if (_ps.PrintSetting.PrintInformaceOSoubor)
                {
                    text = MakeText(string.Format("Souřadnicový systém: {0}", _coordinateListInfo.CoordinateSystem),
                                    _ps.VyskaFontu);
                    yFontIndet = (_ps.VyskaHlavniHlavicky / 2 - toMM(text.Height)) / 2;
                    xFontIndent = 10;
                    drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent, point.Y + yFontIndet)));
                    text = MakeText(string.Format("Zakázka: {0}", _coordinateListInfo.Job), _ps.VyskaFontu);
                    drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent + 90, point.Y + yFontIndet)));
                    yFontIndet = (_ps.VyskaHlavniHlavicky / 2) +
                                 ((_ps.VyskaHlavniHlavicky / 2 - toMM(text.Height)) / 2);
                    text = MakeText(string.Format("Výškový systému: {0}", _coordinateListInfo.LevelSystem),
                                    _ps.VyskaFontu);
                    drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent, point.Y + yFontIndet)));
                    text = MakeText(string.Format("Locality: {0}", _coordinateListInfo.Locality),
                                    _ps.VyskaFontu);
                    drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent + 90, point.Y + yFontIndet)));
                    point.Y += _ps.VyskaHlavniHlavicky;
                    drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
                }
            }
            //konec hlavicka
            //zacatek hlavicka sloupce
            if (_headerType == HeaderType.FirstHeader || _headerType == HeaderType.Header)
            {
                DrawTableHeaderWithOneRow(drawingContext, _ps, _headerType, Pen, ref point, width);
            }
        }

        public override double GetSize()
        {
            if (_headerType == HeaderType.FirstHeader)
            {
                double vyska = _ps.VyskaHlavnihoNadpisu + _ps.VyskaDruhehoRadkuHlavickyTabulky;
                if (_ps.PrintSetting.PrintInformaceOSoubor)
                    vyska += _ps.VyskaHlavniHlavicky;
                return vyska;
            }
            if (_headerType == HeaderType.Header)
            {
                return _ps.VyskaDruhehoRadkuHlavickyTabulky;
            }
            return 0;
        }
    }

    internal class CoordinateRowPageElemnt : RowPageElement
    {
        public CoordinateRowPageElemnt(TableCoordinateListNode tableCoordinateListNode, TableCoordinatePrintSetting ps)
        {
            _tableCoordinateListNode = tableCoordinateListNode;
            _ps = ps;
        }

        readonly TableCoordinateListNode _tableCoordinateListNode;

        public override double GetSize()
        {
            return _ps.VyskaOpakovanehoRadku;
        }

        public override string GetColumnString(int columnIndex)
        {
            string value = string.Empty;
            var souradniceConverter = new CoordinateConverter();
            var dcStringToString = new StringToIntWithInvisibleZero();
            switch (columnIndex)
            {
                case 0: //"NumberWithPrefix"
                    if (_ps.IsVisiblePredcisli && _ps.IsVisibleCislo)
                        value = _tableCoordinateListNode.NumberWithPrefix;
                    else if (_ps.IsVisiblePredcisli)
                        value = _tableCoordinateListNode.Prefix;
                    else if (_ps.IsVisibleCislo)
                        value = _tableCoordinateListNode.Number;
                    else
                        Debug.Assert(false);
                    break;
                case 1: //"Y"
                    value = (string)souradniceConverter.Convert(_tableCoordinateListNode.Y, null, null, CultureInfo.InvariantCulture);
                    break;
                case 2: //"X"
                    value = (string)souradniceConverter.Convert(_tableCoordinateListNode.Y, null, null, CultureInfo.InvariantCulture);
                    break;
                case 3: //"Z"
                    value = (string)souradniceConverter.Convert(_tableCoordinateListNode.Z, null, null, CultureInfo.InvariantCulture);
                    break;
                case 4: //"Code Quality"
                    value = (string)dcStringToString.Convert(_tableCoordinateListNode.Quality, null, null, CultureInfo.InvariantCulture);
                    break;
                case 5: //"Description":
                    value = _tableCoordinateListNode.Description;
                    break;
            }
            return value;
        }
    }
}
