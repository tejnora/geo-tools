using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVToBinCreator
{
    class SeznamKukra
    {
        private struct Data
        {
            public string KRAJ_KOD;
            public string KRAJ_NAZEV;
            public string OKRES_KOD;
            public string NUTS4;
            public string OKRES_NAZEV;
            public string OBEC_KOD;
            public string OBEC_NAZEV;
            public string KU_KOD;
            public string KU_PRAC;
            public string KU_NAZEV;
            public string MAPA;
            public string CISELNA_RADA;
            public string PLATNOST_OD;
            public string PLATNOST_DO;
            public string PRARES_KOD;
            public string PRARES_NAZEV;
            public string POZNAMKA;
        }
        public void DoBinFile(string inLoc, string outLoc)
        {
            try
            {

                string text = File.ReadAllText(inLoc, Encoding.GetEncoding("windows-1250"));
                string[] lines = text.Split('\n');
                List<Data> ku = new List<Data>();
                bool def = true;
                foreach (var line in lines)
                {
                    if (def)
                    {
                        def = false;
                        continue;
                    }
                    string[] fiels = line.Split(';');
                    if (fiels.Length < 16)
                    {
                        Console.WriteLine("Invalid data;{0}", line);
                        continue;
                    }
                    Data d;
                    d.KRAJ_KOD = fiels[0];
                    d.KRAJ_NAZEV = fiels[1];
                    d.OKRES_KOD = fiels[2];
                    d.NUTS4 = fiels[3];
                    d.OKRES_NAZEV = fiels[4];
                    d.OBEC_KOD = fiels[5];
                    d.OBEC_NAZEV = fiels[6];
                    d.KU_KOD = fiels[7];
                    d.KU_PRAC = fiels[8];
                    d.KU_NAZEV = fiels[9];
                    d.MAPA = fiels[10];
                    d.CISELNA_RADA = fiels[11];
                    d.PLATNOST_OD = fiels[12];
                    d.PLATNOST_DO = fiels[13];
                    d.PRARES_KOD = fiels[14];
                    d.PRARES_NAZEV = fiels[15];
                    d.POZNAMKA = fiels[16];
                    ku.Add(d);
                }
                ku.Sort((ku1, ku2) =>
                {
                    return string.Compare(ku1.KU_NAZEV, ku2.KU_NAZEV, true, new CultureInfo("cs-CZ"));
                });
                BinaryWriter bw = new BinaryWriter(File.OpenWrite(outLoc));
                bw.Write(ku.Count());
                foreach (var data in ku)
                {
                    bw.Write(data.KU_NAZEV);
                    bw.Write(data.OKRES_NAZEV);
                    bw.Write(data.OBEC_NAZEV);
                    bw.Write(data.KU_KOD);
                    bw.Write(data.KU_PRAC);
                    bw.Write(data.CISELNA_RADA);
                    bw.Write(data.MAPA);
                }
            }
            catch (Exception ex)
            {
                Console.Write("Parse error:{0}",ex.Message);
            }
            
        }
    }
}
