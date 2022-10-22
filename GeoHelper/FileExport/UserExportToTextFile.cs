using System.Collections.Generic;
using System.IO;
using System.Text;
using GeoBase.Utils;
using GeoHelper.FileParses;
using GeoHelper.Tables;
using GeoHelper.Tables.TableNodes;
using GeoHelper.Tabulky;

namespace GeoHelper.FileExport
{
    internal class UserExportToTextFile : IUserExportIface
    {
        public UserExportToTextFile(string location, string parserPatter, TableBase table, bool undefineReplaceZero)
        {
            Location = location;
            Tokens = Token.ParsePatter(parserPatter);
            Table = table;
            UndefinedReplaceZero = undefineReplaceZero;
        }

        string Location { get; set; }

        List<Token> Tokens { get; set; }

        TableBase Table { get; set; }

        bool UndefinedReplaceZero { get; set; }

        public void Export()
        {
            using (TextWriter writer = new StreamWriter(new FileStream(Location, FileMode.Create), Encoding.UTF8))
            {
                int row = 0;
                foreach (TableNodesBase node in Table.Nodes)
                {
                    row++;
                    string line = string.Empty;
                    foreach (Token token in Tokens)
                    {
                        if (token is TextToken)
                            line += (token as TextToken).Text;
                        else if (token is SymbolToken)
                        {
                            try
                            {
                                string value;
                                if (!node.GetValue(token as SymbolToken, out value))
                                {
                                    if (UndefinedReplaceZero)
                                    {
                                        value = (token as SymbolToken).ToString(0.0);
                                    }
                                    else
                                    {
                                        var par = new ResourceParams();
                                        par.Add("value", (token as SymbolToken).Symbol);
                                        throw new ExportException("257", par);
                                    }
                                }
                                line += value;
                            }
                            catch (TokenFormatStringException ex)
                            {
                                var par = new ResourceParams();
                                par.Add("value", ex.Value);
                                par.Add("pattern", (token as SymbolToken).ToString());
                                par.Add("row", row.ToString());
                                throw new ExportException("256", par);
                            }
                        }
                    }
                    writer.WriteLine(line);
                }
            }
        }
    }
}