using CAD.DTM.Configuration;
using CAD.DTM.Elements.GUI;
using CAD.DTM.Gui;

namespace CAD.DTM.Elements
{
    class DtmTrasaOdbernehoElektrickehoZarizeniElement
        : DtmLinieElementBase
    {
        public override void ExportAttributesToDtm(IDtmExporter exporter)
        {
            ExportSpolecneAtributyObjektuZPS_TI(exporter);
        }
        public override void Init(DtmElementOption dtmElementOption)
        {
            base.Init(dtmElementOption);
            SpolecneAtributyObjektuZPS_TI = new DtmSpolecneAtributyObjektuZPS_TI();
            SpolecneAtributyObjektuZPS_TI.UrovenUmisteniObjektuTI = -1;
        }
        public override void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
        }
    }
}
