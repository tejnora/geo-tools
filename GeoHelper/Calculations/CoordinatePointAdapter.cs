using System;
using GeoCalculations.MethodPoints;
using GeoHelper.Tables.TableNodes;

namespace GeoHelper.Calculations
{
    internal class CoordinatePointAdapter : IPointLoadAdapter
    {
        readonly TableCoordinateListNode _adaptee;

        public CoordinatePointAdapter(TableCoordinateListNode adaptee)
        {
            _adaptee = adaptee;
        }

        public LoadAdapterProperties Properties
        {
            get { return LoadAdapterProperties.CordinateData; }
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
            get { return _adaptee.X; }
        }

        public double Y
        {
            get { return _adaptee.Y; }
        }

        public double Z
        {
            get { return _adaptee.Z; }
        }

        public double Hz
        {
            get { throw new NotImplementedException(); }
        }

        public double HorizontalDistance
        {
            get { throw new NotImplementedException(); }
        }

        public double ElevationDifference
        {
            get { throw new NotImplementedException(); }
        }

        public double Signal
        {
            get { throw new NotImplementedException(); }
        }

        public double ZenitAgnel
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { return _adaptee.Description; }
        }

    }
}