namespace CAD.DTM.Elements
{
    public class DtmSpolecneAtributyZPS
    {
        public DtmSpolecneAtributyZPS()
        {
            TridaPresnostiPoloha = 3;
            TridaPresnostiVyska = 3;
            UrovenUmisteniObjektuZPS = 0;
            ZpusobPorizeniZPS = 1;
        }
        public int UrovenUmisteniObjektuZPS { get; set; }
        public int TridaPresnostiPoloha { get; set; }
        public int TridaPresnostiVyska { get; set; }
        public int ZpusobPorizeniZPS { get; set; }
    }
}
