using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVToBinCreator
{
    /*
    KOD  	K≤d prvku prostorov²ch dat
    VYZNAM 	Nßzev prvku prostorov²ch dat
    SKUP_KOD 	K≤d skupiny prvk∙ prostorov²ch dat
    SKUP_NAZEV 	Nßzev skupiny prvk∙ prostorov²ch dat
    CLEN_KOD 	K≤d Φlena skupiny prvk∙ prostorov²ch dat
    CLEN_NAZEV 	Nßzev Φlena skupiny prvk∙ prostorov²ch dat
    TUM_KOD 	K≤d typu umφst∞nφ prvku prostorov²ch dat
    TUM_NAZEV 	Nßzev typu umφst∞nφ prvku prostorov²ch dat
    POLOHOPIS 	RozliÜenφ, zda se jednß o polohopisn² prvek (a/n)
    EDITOVATELNY 	Definuje, zda je prvek editovateln² v rßmci editace polohopisn²ch prvk∙ mapy (a/n)
    PLATNOST_OD 	Datum vzniku prvku
    PLATNOST_DO 	Datum zßniku prvku
    MINULY_STAV 	Parametry pro zobrazenφ zruÜen²ch prvk∙ (prvky z minulosti). JednotlivΘ hodnoty jsou od sebe odd∞leny mezerou, jejich v²znam je nßsledujφcφ:
    Pro liniovΘ prvky: vrstva, barva, tlouÜ¥ka, styl linie, jmΘno znaΦky na linii, posun v mm, perioda v mm (zßpornß hodnota = otoΦenφ znaΦky).
    Pro textovΘ prvky: vrstva, barva, tlouÜ¥ka, styl, v²Üka v mm, Φφslo fontu, vzta₧n² bod, Üφ°ka v mm.
    Pro bodovΘ prvky: vrstva, barva, tlouÜ¥ka, styl.
    PRITOMNY_STAV 	Parametry pro zobrazenφ prvk∙ na tiskßrn∞ nebo na plotru. JednotlivΘ hodnoty jsou od sebe odd∞leny mezerou, jejich v²znam je nßsledujφcφ:
    Pro liniovΘ prvky: vrstva, barva, tlouÜ¥ka, styl, jmΘno znaΦky na linii, posun v mm, perioda v mm (zßpornß hodnota = otoΦenφ znaΦky).
    Pro textovΘ prvky: vrstva, barva, tlouÜ¥ka, styl, v²Üka v mm, Φφslo fontu, vzta₧n² bod, Üφ°ka v mm.
    Pro bodovΘ prvky: vrstva, barva, tlouÜ¥ka, styl, jmΘno znaΦkovΘho symbolu.
    BUDOUCI_STAV 	Parametry pro zobrazenφ prvk∙ na obrazovce. JednotlivΘ hodnoty jsou od sebe odd∞leny mezerou, jejich v²znam je nßsledujφcφ:
    Pro liniovΘ prvky: vrstva, barva, tlouÜ¥ka, styl, jmΘno znaΦky na linii, posun v mm, perioda v mm (zßpornß hodnota = otoΦenφ znaΦky).
    Pro textovΘ prvky: vrstva, barva, tlouÜ¥ka, styl, v²Üka v mm, Φφslo fontu, vzta₧n² bod, Üφ°ka v mm.
    Pro bodovΘ prvky: vrstva, barva, tlouÜ¥ka, styl.
    PRISLUSNOST 	Definuje, zda mß b²t topologicky urΦena p°φsluÜnost k parcele (a/n)
    KRIVKA 	Definuje, zda prvek m∙₧e b²t k°ivkou (a/n)
    TYP_PRVKU 	Typ prvku:
    1 ... liniov² prvek
    4 ... textov² prvek
    7 ... bodov² prvek (znaΦka) 
     */
    class Prv
    {
        public Prv()
        {
            
        }
        private struct Data
        {
            public Int32  KOD;
            public string VYZNAM;
            public Int32 SKUP_KOD;
            public string CLEN_KOD;
            public Int32  TUM_KOD;
            public bool POLOHOPIS;
            public bool EDITOVATELNY;
            public string PLATNOST_OD;
            public string PLATNOST_DO;
            public ElementDrawInfo MINULY_STAV;
            public ElementDrawInfo PRITOMNY_STAV;
            public ElementDrawInfo BUDOUCI_STAV;
            public bool PRISLUSNOS;
            public bool KRIVKA;
            public Int32 TYP_PRVKU;
            public String BLOK_VFK;
        }
        private class ElementDrawInfo
        {
            public ElementDrawInfo(string parseString)
            {
                _font = string.Empty;
                string[] items=parseString.Split(' ');
                _vrstva = int.Parse(items[0]);
                if (!Int32.TryParse(items[1], out _colorIndex))
                    _colorIndex = 1;
                //color items[1]
                _type = int.Parse(items[2]);
                _tlouska = double.Parse(items[3], CultureInfo.InvariantCulture);
                if(items.Length==4)
                    return;
                _vyskaTextu = double.Parse(items[4], CultureInfo.InvariantCulture);
                if (items.Length == 5)
                    return;
                //font item[5]
                if (items.Length == 6)
                    return;
                _vztaznyBod = int.Parse(items[6]);
                if (items.Length == 7)
                    return;
                _sirkaTextu = double.Parse(items[7], CultureInfo.InvariantCulture);
            }
            private int _vrstva;
            private int _colorIndex;
            private int _type;
            private double _tlouska;
            private double _vyskaTextu;
            private string _font;
            private int _vztaznyBod;
            private double _sirkaTextu;

            public void Save(BinaryWriter aW)
            {
                aW.Write(_vrstva);
                aW.Write(_colorIndex);
                aW.Write(_type);
                aW.Write(_tlouska);
                aW.Write(_vyskaTextu);
                aW.Write(_font);
                aW.Write(_vztaznyBod);
                aW.Write(_sirkaTextu);
            }
            public void Load(BinaryReader aR)
            {
                _vrstva = aR.ReadInt32();
                _colorIndex = aR.ReadInt32();
                _type = aR.ReadInt32();
                _tlouska = aR.ReadDouble();
                _vyskaTextu = aR.ReadDouble();
                _font = aR.ReadString();
                _vztaznyBod = aR.ReadInt32();
                _sirkaTextu = aR.ReadDouble();
            }
        }
        public void DoBinFile(string inLoc, string outLoc)
        {
            try
            {

                string text = File.ReadAllText(inLoc, Encoding.GetEncoding("windows-1250"));
                string[] lines = text.Split('\n');
                List<Data> pr = new List<Data>();
                bool def = true;
                foreach (var line in lines)
                {
                    if (def)
                    {
                        def = false;
                        continue;
                    }
                    string[] fiels = line.Split(';');
                    if (fiels.Length < 18)
                    {
                        Console.WriteLine("Invalid data;{0}", line);
                        continue;
                    }

                    Data d=new Data();
                    d.KOD = Int32.Parse(fiels[0]);
                    d.SKUP_KOD = Int32.Parse(fiels[1]);
                    d.CLEN_KOD = fiels[2];
                    d.TUM_KOD = Int32.Parse(fiels[3]);
                    d.POLOHOPIS = (fiels[4] == "n") ? false : true;
                    d.EDITOVATELNY = fiels[5] == "a" ? true : false;
                    d.PLATNOST_OD = fiels[6];
                    d.VYZNAM = fiels[7];
                    d.MINULY_STAV = new ElementDrawInfo(fiels[8]);
                    d.PRITOMNY_STAV = new ElementDrawInfo(fiels[9]);
                    d.BUDOUCI_STAV = new ElementDrawInfo(fiels[10]);
                    d.PRISLUSNOS = fiels[11] == "a" ? true : false;
                    d.KRIVKA = fiels[12] == "a" ? true : false;
                    d.TYP_PRVKU = Int32.Parse(fiels[13]);
                    d.PLATNOST_DO = fiels[14];
                    //prazdny 15
                    //prazdny 16
                    d.BLOK_VFK = fiels[17];
                    pr.Add(d);
                }
                BinaryWriter bw = new BinaryWriter(File.OpenWrite(outLoc));
                pr=(pr.OrderBy((n) => n.SKUP_KOD)).ToList();
                bw.Write(pr.Last().SKUP_KOD);
                Int32 groupIdx = Int32.MaxValue;
                foreach (var data in pr)
                {
                    if(data.SKUP_KOD!=groupIdx)
                    {
                        bw.Write(data.SKUP_KOD);
                    //    var texttt = (from n in pr where n.SKUP_KOD == data.SKUP_KOD select n).ToList();
                        Int32 count = (from n in pr where n.SKUP_KOD == data.SKUP_KOD select n).Count();
                        bw.Write(count);
                        groupIdx = data.SKUP_KOD;
                    }
                    bw.Write(data.KOD);
                    bw.Write(data.VYZNAM);
                    bw.Write(data.CLEN_KOD);
                    bw.Write(data.TUM_KOD);
                    bw.Write(data.PRISLUSNOS);
                    bw.Write(data.KRIVKA);
                    bw.Write(data.TYP_PRVKU);
                    bw.Write(data.POLOHOPIS);
                    data.MINULY_STAV.Save(bw);
                    data.PRITOMNY_STAV.Save(bw);
                    data.BUDOUCI_STAV.Save(bw);
                    if(data.BLOK_VFK.Contains("HBPEJ"))
                        bw.Write(7);
                    else if(data.BLOK_VFK.Contains("OBPEJ"))
                        bw.Write(8);
                    else if(data.BLOK_VFK.Contains("OP"))
                        bw.Write(1);
                    else if(data.BLOK_VFK.Contains("DPM"))
                        bw.Write(2);
                    else if(data.BLOK_VFK.Contains("OBBP"))
                        bw.Write(3);
                    else if(data.BLOK_VFK.Contains("HP"))
                        bw.Write(4);
                    else if(data.BLOK_VFK.Contains("ZVB"))
                        bw.Write(5);
                    else if(data.BLOK_VFK.Contains("OB"))
                    {
                        bw.Write(6);
                    }
                    else
                    {
                        bw.Write(0);
                    }

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Parse error:{0}",ex.Message);
            }
        }
    }
}
