using System;

namespace VFK.Tables
{
    [Serializable]
    public class Header
    {
        public float VERZE
        {
            get;
            set;
        }
        public DateTime VYTVORENO
        {
            get;
            set;
        }
        public string PUVOD
        {
            get;
            set;
        }
        public string SKUPINA
        {
            get;
            set;
        }
        public string JMENO
        {
            get;
            set;
        }
        public DateTime PLATNOST_OD
        {
            get;
            set;
        }
        public DateTime PLATNOST_DO
        {
            get;
            set;
        }
        public UInt16 ZMENY
        {
            get;
            set;
        }
        public UInt16 NAVRHY
        {
            get;
            set;
        }
    }
}
