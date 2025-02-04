using System;
using System.Runtime.Serialization;
using GeoBase.Gui;
using GeoBase.Localization;
using GeoBase.Utils;
using VFK;

namespace CAD.VFK.GUI
{
    public partial class SouradniceDialog : DialogBase
    {
                public SouradniceDialog(VfkProxyActivePoint point)
            : base("SouradniceDialog")
        {
            InitializeComponent();
            SouradniceDataContext=new SouradniceDialogContext(point);
            DataContext = SouradniceDataContext;
        }
                        public SouradniceDialogContext SouradniceDataContext
        { get; private set; }
            }

    public class SouradniceDialogContext : DataObjectBase<SouradniceDialogContext>
    {
                public SouradniceDialogContext(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {
        }
        public SouradniceDialogContext(VfkProxyActivePoint point)
            : base(null, new StreamingContext())
        {
            Init(point);
        }
                        public readonly PropertyData _sobrUplneCisloBoduProperty = RegisterProperty("SobrUplneCisloBodu", typeof(string), string.Empty);
        public string SobrUplneCisloBodu
        {
            get { return GetValue<string>(_sobrUplneCisloBoduProperty); }
            set { SetValue(_sobrUplneCisloBoduProperty, value); }
        }

        public readonly PropertyData _sobrIsEanabledProperty = RegisterProperty("SobrIsEnabled", typeof(bool), false);
        public bool SobrIsEnabled
        {
            get { return GetValue<bool>(_sobrIsEanabledProperty); }
            set { SetValue(_sobrIsEanabledProperty, value); OnPropertyChanged("IsEnabledAny"); }
        }

        public readonly PropertyData _sobrYProperty = RegisterProperty("SobrY", typeof(double), double.NaN);
        public double SobrY
        {
            get { return GetValue<double>(_sobrYProperty); }
            set { SetValue(_sobrYProperty, value); }
        }

        public readonly PropertyData _sobrXProperty = RegisterProperty("SobrX", typeof (double), double.NaN);
        public double SobrX
        {
            get { return GetValue<double>(_sobrXProperty); }
            set { SetValue(_sobrXProperty, value); }
        }

        public readonly PropertyData _sobrKodChybProperty = RegisterProperty("SobrKodChyb", typeof (string), string.Empty);
        public string SobrKodChyb
        {
            get { return GetValue<string>(_sobrKodChybProperty); }
            set { SetValue(_sobrKodChybProperty, value); }
        }

        public readonly PropertyData _spolYProperty = RegisterProperty("SpolY", typeof (double), double.NaN);
        public double SpolY
        {
            get { return GetValue<double>(_spolYProperty); }
            set { SetValue(_spolYProperty, value); }
        }

        public readonly PropertyData _spolXProperty = RegisterProperty("SpolX", typeof (double), double.NaN);
        public double SpolX
        {
            get { return GetValue<double>(_spolXProperty); }
            set { SetValue(_spolXProperty, value); }
        }

        public readonly PropertyData _spolIsEnabledProperty = RegisterProperty("SpolIsEnabled", typeof (bool), false);
        public bool SpolIsEnabled
        {
            get { return GetValue<bool>(_spolIsEnabledProperty); }
            set { SetValue(_spolIsEnabledProperty, value); OnPropertyChanged("IsEnabledAny"); }
        }

        public readonly PropertyData _spolKatuzeKodMereniProperty = RegisterProperty("SpolKatuzeKodMereni", typeof(uint), 0);
        public uint SpolKatuzeKodMereni
        {
            get { return GetValue<uint>(_spolKatuzeKodMereniProperty); }
            set { SetValue(_spolKatuzeKodMereniProperty, value); }
        }

        public readonly PropertyData _spolCisloZpmzMerProperty = RegisterProperty("SpolCisloZpmzMer", typeof(uint), 0);
        public uint SpolCisloZpmzMer
        {
            get { return GetValue<uint>(_spolCisloZpmzMerProperty); }
            set { SetValue(_spolCisloZpmzMerProperty, value); }
        }

        public bool IsEnabledAny
        {
            get { return SpolIsEnabled || SobrIsEnabled; }
        }
        
        protected override void ValidateFields()
        {
            LanguageDictionary dictionary = LanguageConverter.ResolveDictionary();
            string errorMesg = (string)dictionary.Translate("81", "Text", "PolozkaNemuzeBytPrazdna", typeof(string));
            if (string.IsNullOrEmpty(SobrUplneCisloBodu) || SobrUplneCisloBodu.Length!=12) SetFieldError(_sobrUplneCisloBoduProperty, errorMesg);
            
        }
                        private void Init(VfkProxyActivePoint point)
        {
            SobrIsEnabled = !point.IsSobrReadOnly;
            SobrUplneCisloBodu = point.PointFullName;
            SobrY = point.VfkSobrItem.SOURADNICE_Y;
            SobrX = point.VfkSobrItem.SOURADNICE_X;

            SpolIsEnabled = !point.IsSpolReadOnly;
            SpolY = point.VfkSpolItem.SOURADNICE_Y;
            SpolX = point.VfkSpolItem.SOURADNICE_X;
            SpolKatuzeKodMereni = point.VfkSpolItem.KATUZE_KOD_MER;
            SpolCisloZpmzMer = point.VfkSpolItem.CISLO_ZPMZ_MER;

            SobrKodChyb = point.VfkKkb;
        }
        public bool AssigneInto(VfkProxyActivePoint point)
        {
            try
            {
                point.PointFullName = SobrUplneCisloBodu;
                point.VfkSobrItem.SOURADNICE_Y = SobrY;
                point.VfkSobrItem.SOURADNICE_X = SobrX;

                point.VfkSpolItem.SOURADNICE_Y = SpolY;
                point.VfkSpolItem.SOURADNICE_X = SpolX;
                point.VfkSpolItem.KATUZE_KOD_MER = SpolKatuzeKodMereni;
                point.VfkSpolItem.CISLO_ZPMZ_MER = SpolCisloZpmzMer;
                if (point.VfkSpolItem.STAV_DAT == VFKMain.SOBR_SPOL_STAV_DAT_NEED_UPDATE)
                    point.VfkSpolItem.STAV_DAT = VFKMain.SOBR_SPOL_STAV_DAT_NOVY_BOD;
                point.VfkKkb = SobrKodChyb;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
            }
}
