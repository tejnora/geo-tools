namespace CAD.DTM.Elements
{
    public class DtmSpolecneAtributyObjektuZPS_TI
    {
        public DtmSpolecneAtributyObjektuZPS_TI()
        {
            UrovenUmisteniObjektuTI = 0;
            TridaPresnostiPoloha = 3;
            TridaPresnostiVyska = 3;
            ZpusobPorizeniTI = 3;
            StavObjektu = DtmStavObjektuEnum.Nezjisteno;

        }
        public int UrovenUmisteniObjektuTI { get; set; }
        public int TridaPresnostiPoloha { get; set; }
        public int TridaPresnostiVyska { get; set; }
        public int ZpusobPorizeniTI { get; set; }
        public DtmStavObjektuEnum StavObjektu { get; set; }

    }
}
