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
        public int UrovenUmisteniObjektuTI;
        public int TridaPresnostiPoloha;
        public int TridaPresnostiVyska;
        public int ZpusobPorizeniTI;
        public DtmStavObjektuEnum StavObjektu;
    }
}
