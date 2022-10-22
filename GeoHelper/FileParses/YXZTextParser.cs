using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tabulky;

namespace GeoHelper.FileParses
{
    internal class YXZTextParser
    {
        public List<TableNodesBase> Nodes { get; set; }

        public void ParseFile(FileInfo location)
        {
            Nodes = new List<TableNodesBase>();
            string[] lines = File.ReadAllLines(location.FullName, Encoding.ASCII);
            uint linesCounter = 0;
            foreach (string line in lines)
            {
                var data = new string[5];
                Int32 j = 0;
                var bb = new StringBuilder();
                for (Int32 i = 0; i < line.Length; i++)
                {
                    if (line[i] == ' ')
                    {
                        if (j == 4)
                        {
                            j++;
                            throw new ParserMoreColumnItems(linesCounter, 0);
                        }
                        data[j++] = bb.ToString();
                        bb.Length = 0;
                    }
                    bb.Append(line[i]);
                }
                if (bb.Length > 0)
                    data[j++] = bb.ToString();
                if (j != 5)
                {
                    throw new ParserMoreColumnItems(linesCounter, 0);
                }
                try
                {
                    Double y = Double.Parse(data[1], CultureInfo.InvariantCulture);
                    Double x = Double.Parse(data[2], CultureInfo.InvariantCulture);
                    Double z = Double.Parse(data[3], CultureInfo.InvariantCulture);
                    UInt32 kodKvalita = UInt32.Parse(data[4]);
                    String uplneCislo = data[0].Trim();
                    if (uplneCislo.Length != 0)
                        throw new Exception();
                    string predcisli = uplneCislo.Substring(0, 8);
                    string cislo = uplneCislo.Substring(8, 10);
                    Nodes.Add(new TableCoordinateListNode
                                  {Prefix = predcisli, Number = cislo, Y = y, X = x, Z = z, Quality = kodKvalita});
                }
                catch (Exception)
                {
                    throw new ParserInvalidRow(linesCounter);
                }
                linesCounter++;
            }
        }
    }
}