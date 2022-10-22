using System;
using System.Diagnostics;
using System.Windows;


namespace GeoBase.Utils
{
    public class ProgramOption
    {
        public enum Type
        {
            tBool = 0,
            tInt = 1,
            tDouble = 2,
            tString = 3,
            tPoint = 4,
            tUnset = 5
        };

        public static bool operator ==(ProgramOption aA, ProgramOption aB)
        {
            if (aA.getType() != aB.getType()) return false;
            switch (aA.getType())
            {
                case Type.tBool:
                    return aA.getBool() == aB.getBool();
                case Type.tDouble:
                    return aA.getDouble() == aB.getDouble();
                case Type.tInt:
                    return aA.getDouble() == aB.getDouble();
                case Type.tString:
                    return aA.getString() == aB.getString();
                case Type.tPoint:
                    return aA.getPoint() == aB.getPoint();
                case Type.tUnset:
                    return true;

            }
            return false;
        }
        public static bool operator !=(ProgramOption aA, ProgramOption aB)
        {
            bool ret = aA == aB;
            return !ret;
        }
        public ProgramOption() { unset(); }
        public ProgramOption(bool aBool) { setBool(aBool); }
        public ProgramOption(double aValue) { setDouble(aValue); }
        public ProgramOption(Int32 aValue) { setInt(aValue); }
        public ProgramOption(string aValue) { setString(aValue); }
        public ProgramOption(Point aValue) { setPoint(aValue); }
        public bool getBool() { Debug.Assert(isBool()); return mBool; }
        public bool getBool(bool aDefault)
        {
            if (isBool()) return mBool;
            return aDefault;
        }

        public void setBool(bool aBool) { mType = Type.tBool; mBool = aBool; }

        public Int32 getInt() { Debug.Assert(isInt()); return mInt; }
        public Int32 getInt(Int32 aDefault) { if (isInt()) return mInt; return aDefault; }
        public void setInt(Int32 aInt) { mType = Type.tInt; mInt = aInt; }

        public Double getDouble() { Debug.Assert(isDouble()); return mDouble; }
        public Double getDouble(Double aDefault) { if (isDouble()) return mDouble; return aDefault; }
        public void setDouble(Double aDouble) { mType = Type.tDouble; mDouble = aDouble; }
        public void setPoint(Point aPoint) { mType = Type.tPoint; mPoint = aPoint; }
        public Point getPoint(Point aDefault) { if (isPoint())return mPoint; return aDefault; }
        public Point getPoint() { Debug.Assert(isPoint()); return mPoint; }

        public String getString() { Debug.Assert(isString()); return mString; }
        public String getString(String aDefault) { if (isString()) return mString; return aDefault; }
        public void setString(String aString) { mType = Type.tString; mString = aString; }

        public Type getType() { return mType; }

        public bool isBool() { return mType == Type.tBool; }
        public bool isInt() { return mType == Type.tInt; }
        public bool isDouble() { return mType == Type.tDouble; }
        public bool isString() { return mType == Type.tString; }
        public bool isPoint() { return mType == Type.tPoint; }

        public bool isSet() { return mType != Type.tUnset; }
        public void unset() { mType = Type.tUnset; }

        private Type mType;
        private bool mBool;
        private Int32 mInt;
        private Double mDouble;
        private String mString;
        private Point mPoint;
    };
}
