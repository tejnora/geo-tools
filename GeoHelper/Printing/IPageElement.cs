using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GeoHelper.Tables.TablePrintingElements;
using GeoHelper.Tabulky;

namespace GeoHelper.Printing
{
    public interface IPageElement
    {
        void OnRender(DrawingContext drawingContext, ref Point point, double width);
        double GetSize();
    }

    public class PageElemnt : IPageElement
    {
        public enum HeaderType
        {
            FirstHeader,
            Header
        }

        public PageElemnt()
        {
            Pen = new Pen(Brushes.Black, 1);
        }

        public static double _mmToPixel = 3.779527;

        public static double toPixel(double mmValue)
        {
            return mmValue*_mmToPixel;
        }

        public static Point toPixel(Point point)
        {
            return new Point(point.X*_mmToPixel, point.Y*_mmToPixel);
        }

        public static FormattedText MakeText(string text, double textSize)
        {
            return new FormattedText(text, CultureInfo.CurrentCulture,
                                     FlowDirection.LeftToRight,
                                     new Typeface("Arial"), textSize, Brushes.Black);
        }

        public static double toMM(double pixelValue)
        {
            return pixelValue/_mmToPixel;
        }

        public static double GetRealWidth(double width, UInt32 percent)
        {
            return width/100.0*percent;
        }

        public virtual void OnRender(DrawingContext drawingContext, ref Point point, double width)
        {
        }

        public virtual double GetSize()
        {
            return 0;
        }

        public void DrawTextToCenter(DrawingContext drawingContext, Point point, double vyska, double sirka, string text,
                                     double vyskaFontu, FontWeight fontWeight, Brush brush)
        {
            FormattedText ftext = MakeText(text, vyskaFontu);
            ftext.SetFontWeight(fontWeight);
            ftext.SetForegroundBrush(brush);
            double xFontIndent = (sirka - toMM(ftext.Width))/2;
            double yFontIndet = (vyska - toMM(ftext.Height))/2;
            if (xFontIndent < 0 || yFontIndet < 0)
                return;
            drawingContext.DrawText(ftext, toPixel(new Point(point.X + xFontIndent, point.Y + yFontIndet)));
        }

        public void DrawTableHeaderWithOneRow(DrawingContext drawingContext, TablePrintSettingBase printSetting,
                                              HeaderType headerType, Pen pen, ref Point point, double width)
        {
            if (headerType == HeaderType.Header)
            {
                drawingContext.DrawLine(pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
                drawingContext.DrawLine(pen, toPixel(point),
                                        toPixel(new Point(point.X,
                                                          point.Y + printSetting.VyskaDruhehoRadkuHlavickyTabulky)));
                drawingContext.DrawLine(pen, toPixel(new Point(point.X + width, point.Y)),
                                        toPixel(new Point(point.X + width, point.Y +
                                                                           printSetting.VyskaDruhehoRadkuHlavickyTabulky)));
            }
            double deltaPosX = 0;
            for (Int32 i = 0; i < printSetting.Columns.Length; i++)
            {
                if (!printSetting.Columns[i].Visibility) continue;
                double columnSize = GetRealWidth(width, printSetting.Columns[i].Width);
                DrawTextToCenter(drawingContext, new Point(point.X + deltaPosX, point.Y),
                                 printSetting.VyskaDruhehoRadkuHlavickyTabulky, columnSize, printSetting.Columns[i].Name,
                                 printSetting.VyskaFontu, FontWeights.Bold, Brushes.Black);
                deltaPosX += columnSize;
                if (i < printSetting.Columns.Length - 1)
                    drawingContext.DrawLine(pen, toPixel(new Point(point.X + deltaPosX, point.Y)),
                                            toPixel(new Point(point.X + deltaPosX,
                                                              point.Y + printSetting.VyskaDruhehoRadkuHlavickyTabulky)));
            }

            //kone hlavicka sloupec
            point.Y += printSetting.VyskaDruhehoRadkuHlavickyTabulky;
            drawingContext.DrawLine(pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
        }

        public Pen Pen { get; set; }
    }

    public class RowPageElement : PageElemnt
    {
        protected TablePrintSettingBase _ps;

        public override void OnRender(DrawingContext drawingContext, ref Point point, double width)
        {
            double deltaPosX = 0;
            for (Int32 i = 0; i < _ps.Columns.Length; i++)
            {
                if (!_ps.Columns[i].Visibility) continue;
                string value = GetColumnString(i);
                double columnSize = GetRealWidth(width, _ps.Columns[i].Width);
                if (value.Length != 0)
                {
                    DrawTextToCenter(drawingContext, new Point(point.X + deltaPosX, point.Y),
                                     _ps.VyskaOpakovanehoRadku, columnSize, value,
                                     _ps.VyskaFontu, FontWeights.Normal, Brushes.Black);
                }
                drawingContext.DrawLine(Pen, toPixel(new Point(point.X + deltaPosX, point.Y)),
                                        toPixel(new Point(point.X + deltaPosX, point.Y + _ps.VyskaOpakovanehoRadku)));
                deltaPosX += columnSize;
            }
            drawingContext.DrawLine(Pen, toPixel(new Point(point.X + deltaPosX, point.Y)),
                                    toPixel(new Point(point.X + deltaPosX, point.Y + _ps.VyskaOpakovanehoRadku)));
            point.Y += GetSize();
            drawingContext.DrawLine(Pen, toPixel(point), toPixel(new Point(point.X + width, point.Y)));
        }

        public virtual string GetColumnString(int columnIndex)
        {
            Debug.Assert(false);
            return string.Empty;
        }
    }

    public class FooterElement : PageElemnt
    {
    }
}