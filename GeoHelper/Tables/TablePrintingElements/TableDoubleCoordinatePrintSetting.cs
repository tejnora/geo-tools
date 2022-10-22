using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GeoHelper.Converters;
using GeoHelper.Printing;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables.TablePrintingElements
{
    internal class TableDoubleCoordinatePrintSetting
        : TablePrintSettingBase
    {
        public TableDoubleCoordinatePrintSetting(PrintSetting printSetting, ColumnVisibility columnVisibility)
            : base(printSetting, columnVisibility)
        {
            Columns = new[]
                          {
                              new ColumnIfno("Číslo bodu", 20),
                              new ColumnIfno("Y", 15),
                              new ColumnIfno("X", 15),
                              new ColumnIfno("Kód kvality", 10),
                              new ColumnIfno("Y", 15),
                              new ColumnIfno("X", 15),
                              new ColumnIfno("Kód kvality", 10),
                          };
            IsVisibleCislo = (columnVisibility & ColumnVisibility.Number) == ColumnVisibility.Number;
            IsVisiblePredcisli = (columnVisibility & ColumnVisibility.Prefix) == ColumnVisibility.Prefix;
            Columns[0].Visibility = (IsVisibleCislo || IsVisiblePredcisli);
            Columns[1].Visibility = ((columnVisibility & ColumnVisibility.CoordinateX) == ColumnVisibility.CoordinateX);
            Columns[2].Visibility = ((columnVisibility & ColumnVisibility.CoordinateY) == ColumnVisibility.CoordinateY);
            Columns[3].Visibility = ((columnVisibility & ColumnVisibility.Quality) == ColumnVisibility.Quality);
            Columns[4].Visibility = ((columnVisibility & ColumnVisibility.SpolY) == ColumnVisibility.SpolY);
            Columns[5].Visibility = ((columnVisibility & ColumnVisibility.SpolX) == ColumnVisibility.SpolX);
            Columns[6].Visibility = ((columnVisibility & ColumnVisibility.SpolQuality) == ColumnVisibility.SpolQuality);
            RecalcTableColumnWidth();
        }
    }

    internal class DoubleCoordinateHeaderPageElemnt : PageElemnt
    {
        public DoubleCoordinateHeaderPageElemnt(CoordinateListInfoDialogContext coordinateListInfo, TableDoubleCoordinatePrintSetting ps,
                                   HeaderType headerType)
        {
            _coordinateListInfo = coordinateListInfo;
            _ps = ps;
            _headerType = headerType;
        }

        readonly HeaderType _headerType;
        readonly TableDoubleCoordinatePrintSetting _ps;
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
                FormattedText text = MakeText("SEZNAM SOUŘADNIC", _ps.VyskaFontu + 2);
                text.SetFontWeight(FontWeights.Bold);
                double xFontIndent = (width - toMM(text.Width)) / 2;
                double yFontIndet = (_ps.VyskaHlavnihoNadpisu - toMM(text.Height)) / 2;
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
                if (_headerType == HeaderType.Header)
                {
                    drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
                    drawingContext.DrawLine(Pen, toPixel(point),
                                            toPixel(new Point(point.X, point.Y + _ps.VyskaPrvnihoRadkuHlavickyTabulky +
                                                                       _ps.VyskaDruhehoRadkuHlavickyTabulky)));
                    drawingContext.DrawLine(Pen, toPixel(new Point(point.X + width, point.Y)),
                                            toPixel(new Point(point.X + width, point.Y +
                                                                               _ps.VyskaPrvnihoRadkuHlavickyTabulky +
                                                                               _ps.VyskaDruhehoRadkuHlavickyTabulky)));
                }
                double firstColumnOffset = GetRealWidth(width, _ps.Columns[0].Width);
                drawingContext.DrawLine(Pen, toPixel(new Point(point.X + firstColumnOffset, point.Y)),
                                        toPixel(new Point(point.X + firstColumnOffset,
                                                          point.Y + _ps.VyskaPrvnihoRadkuHlavickyTabulky)));
                double leftColumnSize = GetRealWidth(width,
                                                     _ps.Columns[1].Width +
                                                     _ps.Columns[2].Width +
                                                     _ps.Columns[3].Width);
                double rightColumnSize = GetRealWidth(width,
                                                      _ps.Columns[4].Width + _ps.Columns[5].Width +
                                                      _ps.Columns[6].Width);
                DrawTextToCenter(drawingContext, new Point(point.X + firstColumnOffset, point.Y),
                                 _ps.VyskaPrvnihoRadkuHlavickyTabulky, leftColumnSize,
                                 "Souřadnice obrazu", _ps.VyskaFontu, FontWeights.Normal, Brushes.Black);
                DrawTextToCenter(drawingContext, new Point(point.X + leftColumnSize + firstColumnOffset, point.Y),
                                 _ps.VyskaDruhehoRadkuHlavickyTabulky, rightColumnSize, "Souřadnice polohy",
                                 _ps.VyskaFontu, FontWeights.Normal, Brushes.Black);
                drawingContext.DrawLine(Pen, toPixel(new Point(point.X + leftColumnSize + firstColumnOffset, point.Y)),
                                        toPixel(new Point(point.X + leftColumnSize + firstColumnOffset,
                                                          point.Y + _ps.VyskaPrvnihoRadkuHlavickyTabulky)));
                point.Y += _ps.VyskaPrvnihoRadkuHlavickyTabulky;
                drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));

                double deltaPosX = 0;
                for (Int32 i = 0; i < _ps.Columns.Count(); i++)
                {
                    if (!_ps.Columns[i].Visibility) continue;
                    double columnSize = GetRealWidth(width, _ps.Columns[i].Width);
                    DrawTextToCenter(drawingContext, new Point(point.X + deltaPosX, point.Y),
                                     _ps.VyskaDruhehoRadkuHlavickyTabulky, columnSize, _ps.Columns[i].Name,
                                     _ps.VyskaFontu, FontWeights.Bold, Brushes.Black);
                    deltaPosX += columnSize;
                    if (i < _ps.Columns.Count() - 1)
                        drawingContext.DrawLine(Pen, toPixel(new Point(point.X + deltaPosX, point.Y)),
                                                toPixel(new Point(point.X + deltaPosX,
                                                                  point.Y + _ps.VyskaDruhehoRadkuHlavickyTabulky)));
                }
                //kone hlavicka sloupec
                point.Y += _ps.VyskaDruhehoRadkuHlavickyTabulky;
                drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
            }
        }

        public override double GetSize()
        {
            if (_headerType == HeaderType.FirstHeader)
            {
                double vyska = _ps.VyskaHlavnihoNadpisu + _ps.VyskaPrvnihoRadkuHlavickyTabulky +
                               _ps.VyskaDruhehoRadkuHlavickyTabulky;
                if (_ps.PrintSetting.PrintInformaceOSoubor)
                    vyska += _ps.VyskaHlavniHlavicky;
                return vyska;
            }
            if (_headerType == HeaderType.Header)
            {
                return _ps.VyskaPrvnihoRadkuHlavickyTabulky + _ps.VyskaDruhehoRadkuHlavickyTabulky;
            }
            return 0;
        }
    }

    internal class DobleCoordinateRowPageElemnt : RowPageElement
    {
        public DobleCoordinateRowPageElemnt(TableDoubleCoordinateListNode tableDoubleCoordinateListNode, TableDoubleCoordinatePrintSetting ps)
        {
            _tableDoubleCoordinateListNode = tableDoubleCoordinateListNode;
            _ps = ps;
        }

        readonly TableDoubleCoordinateListNode _tableDoubleCoordinateListNode;
        new readonly TableDoubleCoordinatePrintSetting _ps;

        public override double GetSize()
        {
            return _ps.VyskaOpakovanehoRadku;
        }

        public override string GetColumnString(int columnIndex)
        {
            var souradniceConverter = new CoordinateConverter();
            var dcStringToString = new StringToIntWithInvisibleZero();
            string value = string.Empty;
            switch (columnIndex)
            {
                case 0: //NumberWithPrefix
                    if (_ps.IsVisiblePredcisli && _ps.IsVisibleCislo)
                        value = _tableDoubleCoordinateListNode.NumberWithPrefix;
                    else if (_ps.IsVisiblePredcisli)
                        value = _tableDoubleCoordinateListNode.Prefix;
                    else if (_ps.IsVisibleCislo)
                        value = _tableDoubleCoordinateListNode.Number;
                    else
                        Debug.Assert(false);
                    break;
                case 1: //Y
                    value = (string)souradniceConverter.Convert(_tableDoubleCoordinateListNode.Y, null, null, CultureInfo.InvariantCulture);
                    break;
                case 2: //X
                    value = (string)souradniceConverter.Convert(_tableDoubleCoordinateListNode.Y, null, null, CultureInfo.InvariantCulture);
                    break;
                case 3: //Y
                    value = (string)dcStringToString.Convert(_tableDoubleCoordinateListNode.Quality, null, null, CultureInfo.InvariantCulture);
                    break;
                case 4: //SpolY
                    value = (string)souradniceConverter.Convert(_tableDoubleCoordinateListNode.Y, null, null, CultureInfo.InvariantCulture);
                    break;
                case 5: //SpolX
                    value = (string)souradniceConverter.Convert(_tableDoubleCoordinateListNode.Y, null, null, CultureInfo.InvariantCulture);
                    break;
                case 6: //SpolQuality
                    value =
                        (string)dcStringToString.Convert(_tableDoubleCoordinateListNode.SpolQuality, null, null, CultureInfo.InvariantCulture);
                    break;
            }
            return value;
        }
    }
}
