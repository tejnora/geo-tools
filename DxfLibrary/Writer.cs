using System.IO;
using System.Globalization;
using System.Text;

namespace DxfLibrary
{
    public class Writer
    {
                public static void Write(Document d, Stream s)
        {
            StreamWriter sr = new StreamWriter(s, Encoding.GetEncoding("iso-8859-2"));

            WriteHeader(d.Header, sr);
            WriteTables(d.Tables, sr);
            WriteBlocks(d.Blocks, sr);
            WriteEntities(d.Entities, sr);

            WriteData(new Data(0, "EOF"), sr);
            sr.Close();
        }
                        private static void WriteHeader(Header h, StreamWriter sr)
        {
            if (h == null) return;
            WriteElement(h, sr);
        }
        private static void WriteEntities(Entities e, StreamWriter sr)
        {
            if (e == null) return;
            WriteElement(e, sr);
        }
        private static void WriteBlocks(Blocks b, StreamWriter sr)
        {
            if (b == null) return;
            WriteElement(b, sr);
        }
        private static void WriteTables(Tables t, StreamWriter sr)
        {
            if (t == null) return;
            WriteElement(t, sr);
        }
        private static void WriteElement(Element e, StreamWriter sr)
        {
            if (e == null) return;
            if (e.S != null)
            {
                sr.Write(e.S);
                return;
            }
            WriteData(e.StartTag, sr);
            for (int i = 0; i < e.DataCount(); i++)
            {
                WriteData(e.GetData(i), sr);
            }
            for (int i = 0; i < e.ElementCount(); i++)
                WriteElement(e.GetElement(i), sr);
            WriteData(e.EndTag, sr);
        }
        private static void WriteData(Data d, StreamWriter sr)
        {
            if (d.Code == -10) return;
            sr.Write(d.Code);
            sr.Write("\n");
            if (d._data.GetType().ToString() == "System.Double")
                sr.Write(((double)d._data).ToString(CultureInfo.InvariantCulture));
            else
                sr.Write(d._data);
            sr.Write("\n");
        }
            }
}
