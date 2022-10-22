using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeoHelper.Printing
{
    internal class PageElementBase : UserControl
    {
        public PageElementBase(List<IPageElement> pageElements, Int32 begin, Int32 end)
        {
            _pageElements = pageElements;
            _begin = begin;
            _end = end;
        }

        readonly Int32 _begin;
        readonly Int32 _end;
        readonly List<IPageElement> _pageElements;

        protected override void OnRender(DrawingContext drawingContext)
        {
            double tableWidth = Width - Margin.Left - Margin.Right;
            tableWidth /= PageElemnt._mmToPixel;
            if (tableWidth <= 0) return;

            //remove 1 pixel for table border form both sides
            var curPoint = new Point(PageElemnt.toMM(1), PageElemnt.toMM(1));
            tableWidth -= 2;
            for (Int32 i = _begin; i < _end; i++)
            {
                _pageElements[i].OnRender(drawingContext, ref curPoint, tableWidth);
            }
        }
    }
}