using System;
using GeoCalculations.MethodPoints;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Calculations
{
    internal class MeasurePointAdapter : IPointLoadAdapter
    {
        readonly TableMeasureListNode _adaptee;

        public MeasurePointAdapter(TableMeasureListNode adaptee)
        {
            _adaptee = adaptee;
        }

        public LoadAdapterProperties Properties
        {
            get { return LoadAdapterProperties.MeasureData; }
        }

        public string Prefix
        {
            get { return _adaptee.Prefix; }
        }

        public string Number
        {
            get { return _adaptee.Number; }
        }

        public double X
        {
            get { throw new NotImplementedException(); }
        }

        public double Y
        {
            get { throw new NotImplementedException(); }
        }

        public double Z
        {
            get { throw new NotImplementedException(); }
        }

        public double Hz
        {
            get { return _adaptee.Hz; }
        }

        public double HorizontalDistance
        {
            get { return _adaptee.HorizontalDistance; }
        }

        public double ElevationDifference
        {
            get { return _adaptee.ElevationDefference; }
        }

        public double Signal
        {
            get { return _adaptee.Signal; }
        }

        public double ZenitAgnel
        {
            get { return _adaptee.ZenitAngle; }
        }

        public string Description
        {
            get { return _adaptee.Description; }
        }
    }
}