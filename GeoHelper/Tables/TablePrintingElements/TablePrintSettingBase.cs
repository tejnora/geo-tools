using System;
using GeoHelper.Printing;
using GeoHelper.Tabulky;

namespace GeoHelper.Tables.TablePrintingElements
{
    public class TablePrintSettingBase
    {
        public struct ColumnIfno
        {
            public string Name;
            public bool Visibility;
            public uint Width;

            public ColumnIfno(string name, uint width)
            {
                Name = name;
                Width = width;
                Visibility = true;
            }
        }

        public TablePrintSettingBase(PrintSetting printSetting, ColumnVisibility columnVisibility)
        {
            PrintSetting = printSetting;
            VyskaHlavnihoNadpisu = 7;
            VyskaHlavniHlavicky = 10;
            VyskaPrvnihoRadkuHlavickyTabulky = 5;
            VyskaDruhehoRadkuHlavickyTabulky = 5;
            VyskaOpakovanehoRadku = 3.2;
            VyskaFontu = 10;
        }

        public double VyskaHlavnihoNadpisu { get; set; }
        public double VyskaHlavniHlavicky { get; set; }
        public double VyskaPrvnihoRadkuHlavickyTabulky { get; set; }
        public double VyskaDruhehoRadkuHlavickyTabulky { get; set; }
        public double VyskaOpakovanehoRadku { get; set; }
        public double VyskaFontu { get; set; }
        public PrintSetting PrintSetting { get; set; }
        public bool IsVisibleCislo { get; set; }
        public bool IsVisiblePredcisli { get; set; }
        public ColumnIfno[] Columns { get; set; }

        public void RecalcTableColumnWidth()
        {
            uint sum = 0;
            for (int i = 0; i < Columns.Length; i++)
            {
                if (Columns[i].Visibility)
                    sum += Columns[i].Width;
            }
            var scale = 100.0/sum;
            uint correctionSum = 0;
            for (var i = 0; i < Columns.Length; i++)
            {
                if (Columns[i].Visibility)
                {
                    var width = Columns[i].Width*scale;
                    Columns[i].Width = Convert.ToUInt32(width);
                    correctionSum += Columns[i].Width;
                }
                else
                {
                    Columns[i].Width = 0;
                }
            }
            correctionSum -= 100;
            if (correctionSum <= 0) return;
            for (var i = 0; i < Columns.Length; i++)
            {
                if (!Columns[i].Visibility) continue;
                Columns[i].Width -= correctionSum;
                break;
            }
        }
    }
}