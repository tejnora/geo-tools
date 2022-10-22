using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GeoHelper.Converters;
using GeoHelper.Printing;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Tables.TablePrintingElements
{
    internal class TableMeasurePrintSetting
        : TablePrintSettingBase
    {
        public TableMeasurePrintSetting(PrintSetting printSetting, ColumnVisibility columnVisibility)
            : base(printSetting, columnVisibility)
        {
            Columns = new[]
                          {
                              new ColumnIfno("Číslo bodu", 20),
                              new ColumnIfno("Hz", 10),
                              new ColumnIfno("Z", 10),
                              new ColumnIfno("Vod. délka", 10),
                              new ColumnIfno("dH", 10),
                              new ColumnIfno("Signál", 10),
                              new ColumnIfno("Description", 30),
                          };
            VyskaHlavniHlavicky = 15;
            IsVisibleCislo = (columnVisibility & ColumnVisibility.Number) == ColumnVisibility.Number;
            IsVisiblePredcisli = (columnVisibility & ColumnVisibility.Prefix) == ColumnVisibility.Prefix;
            Columns[0].Visibility = (IsVisibleCislo || IsVisiblePredcisli);
            Columns[1].Visibility = ((columnVisibility & ColumnVisibility.Hz) == ColumnVisibility.Hz);
            Columns[2].Visibility = ((columnVisibility & ColumnVisibility.ZenitAngle) == ColumnVisibility.ZenitAngle);
            Columns[3].Visibility = ((columnVisibility & ColumnVisibility.HorizontalDistance) ==
                                     ColumnVisibility.HorizontalDistance);
            Columns[4].Visibility = ((columnVisibility & ColumnVisibility.dH) == ColumnVisibility.dH);
            Columns[5].Visibility = ((columnVisibility & ColumnVisibility.Signal) == ColumnVisibility.Signal);
            Columns[6].Visibility = ((columnVisibility & ColumnVisibility.Description) == ColumnVisibility.Description);
            RecalcTableColumnWidth();
        }
    }

    internal class MeasureHeaderPageElemnt : PageElemnt
    {
        public MeasureHeaderPageElemnt(SeznamMereniInfoDialogContext seznamSouradnicInfo, TableMeasurePrintSetting ps,
                                    HeaderType headerType)
        {
            _seznamSouradnicInfo = seznamSouradnicInfo;
            _ps = ps;
            _headerType = headerType;
        }

        readonly HeaderType _headerType;
        readonly TableMeasurePrintSetting _ps;
        readonly SeznamMereniInfoDialogContext _seznamSouradnicInfo;

        public override void OnRender(DrawingContext drawingContext, ref Point point, double width)
        {
            //left and right line
            if (_headerType == HeaderType.FirstHeader)
            {
                drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X, point.Y + GetSize())));
                drawingContext.DrawLine(Pen, toPixel(new Point(point.X + width, point.Y)),
                                        toPixel(new Point(point.X + width, point.Y + GetSize())));
                drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
                FormattedText text = MakeText("S E Z N A M   M Ě Ř EN Í", _ps.VyskaFontu + 2);
                text.SetFontWeight(FontWeights.Bold);
                double xFontIndent = (width - toMM(text.Width)) / 2;
                double yFontIndet = (_ps.VyskaHlavnihoNadpisu - toMM(text.Height)) / 2;
                drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent, point.Y + yFontIndet)));
                point.Y += _ps.VyskaHlavnihoNadpisu;
                drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
                //zacatek hlavicka
                if (_ps.PrintSetting.PrintInformaceOSoubor)
                {
                    //first line
                    text = MakeText(string.Format("Zakázka: {0}", _seznamSouradnicInfo.Job),
                                    _ps.VyskaFontu);
                    yFontIndet = (_ps.VyskaHlavniHlavicky / 3 - toMM(text.Height)) / 2;
                    xFontIndent = 10;
                    drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent, point.Y + yFontIndet)));
                    text = MakeText(string.Format("Měřič: {0}", _seznamSouradnicInfo.MeasureMan), _ps.VyskaFontu);
                    drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent + 90, point.Y + yFontIndet)));
                    //second line
                    yFontIndet = (_ps.VyskaHlavniHlavicky / 3) +
                                 ((_ps.VyskaHlavniHlavicky / 3 - toMM(text.Height)) / 2);
                    text = MakeText(string.Format("Locality: {0}", _seznamSouradnicInfo.Locality),
                                    _ps.VyskaFontu);
                    drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent, point.Y + yFontIndet)));
                    text = MakeText(string.Format("TotalStation: {0}", _seznamSouradnicInfo.TotalStation),
                                    _ps.VyskaFontu);
                    drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent + 90, point.Y + yFontIndet)));
                    //third line
                    text = MakeText(string.Format("Data: {0}", _seznamSouradnicInfo.Date),
                                    _ps.VyskaFontu);
                    yFontIndet = _ps.VyskaHlavniHlavicky / 3 * 2 + ((_ps.VyskaHlavniHlavicky / 3 - toMM(text.Height)) / 2);
                    drawingContext.DrawText(text, toPixel(new Point(point.X + xFontIndent + 90, point.Y + yFontIndet)));
                    //botton line
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

    internal class MeasureRowPageElemnt : RowPageElement
    {
        public MeasureRowPageElemnt(TableMeasureListNode tableMeasureListNode, TableMeasurePrintSetting ps)
        {
            _tableMeasureListNode = tableMeasureListNode;
            _ps = ps;
        }

        readonly TableMeasureListNode _tableMeasureListNode;

        public override void OnRender(DrawingContext drawingContext, ref Point point, double width)
        {
            double deltaPosX = 0;
            if (_tableMeasureListNode.IsPointOfView)
                drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
            for (Int32 i = 0; i < _ps.Columns.Length; i++)
            {
                if (!_ps.Columns[i].Visibility) continue;
                string value = GetColumnString(i);
                double columnSize = GetRealWidth(width, _ps.Columns[i].Width);
                if (value.Length != 0)
                {
                    DrawTextToCenter(drawingContext, new Point(point.X + deltaPosX, point.Y),
                                     _ps.VyskaOpakovanehoRadku, columnSize, value,
                                     _ps.VyskaFontu, _tableMeasureListNode.FontWeight, _tableMeasureListNode.FontColor);
                }
                drawingContext.DrawLine(Pen, toPixel(new Point(point.X + deltaPosX, point.Y)),
                                        toPixel(new Point(point.X + deltaPosX, point.Y + _ps.VyskaOpakovanehoRadku)));
                deltaPosX += columnSize;
            }
            drawingContext.DrawLine(Pen, toPixel(new Point(point.X + deltaPosX, point.Y)),
                                    toPixel(new Point(point.X + deltaPosX, point.Y + _ps.VyskaOpakovanehoRadku)));
            point.Y += GetSize();
        }

        public override double GetSize()
        {
            return _ps.VyskaOpakovanehoRadku;
        }

        public override string GetColumnString(int columnIndex)
        {
            var value = string.Empty;
            switch (columnIndex)
            {
                case 0: //"NumberWithPrefix"
                    if (_ps.IsVisiblePredcisli && _ps.IsVisibleCislo)
                        value = _tableMeasureListNode.NumberWithPrefix;
                    else if (_ps.IsVisiblePredcisli)
                        value = _tableMeasureListNode.Prefix;
                    else if (_ps.IsVisibleCislo)
                        value = _tableMeasureListNode.Number;
                    else
                        Debug.Assert(false);
                    break;
                case 1: //HZ
                    value = (string)new AngleConverter().Convert(_tableMeasureListNode.Hz, null, null, CultureInfo.InvariantCulture);
                    break;
                case 2: //Z
                    value =
                        (string)
                        new AngleConverter().Convert(_tableMeasureListNode.ZenitAngle, null, null, CultureInfo.InvariantCulture);
                    break;
                case 3: //Vod. delka
                    value =
                        (string)
                        new Converters.LengthConverter().Convert(_tableMeasureListNode.HorizontalDistance, null, null, CultureInfo.InvariantCulture);
                    break;
                case 4: //dh
                    value = (string)new AngleConverter().Convert(_tableMeasureListNode.ElevationDefference, null, null, CultureInfo.InvariantCulture);
                    break;
                case 5: //Signal
                    value =
                        (string)new Converters.LengthConverter().Convert(_tableMeasureListNode.Signal, null, null, CultureInfo.InvariantCulture);
                    break;
                case 6: //Description
                    value = _tableMeasureListNode.Description;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            return value;
        }
    }

    internal class MeasureFooterElement : FooterElement
    {
        public override void OnRender(DrawingContext drawingContext, ref Point point, double width)
        {
            drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
        }
    }
}
