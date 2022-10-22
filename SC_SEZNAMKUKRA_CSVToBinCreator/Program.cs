using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace CSVToBinCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Parse:{0}", "SC_SEZNAMKUKRA_DOTAZ_20100301_030005.csv");
            SeznamKukra sk=new SeznamKukra();
            sk.DoBinFile(@"SC_SEZNAMKUKRA_DOTAZ_20100301_030005.csv", @"SC_SEZNAMKUKRA.dat");
            Console.Write("Parse:{0}", "SC_T_PRV_DOTAZ_20100101_030015.csv");
            Prv prv=new Prv();
            prv.DoBinFile(@"t_prvku_p_dat_del_NVF.csv", @"SC_T_PRV.dat");
        }
    }
}
