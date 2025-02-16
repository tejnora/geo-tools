using System.Runtime.Serialization;
using GeoBase.Utils;
using GeoBase;
using System.Collections.ObjectModel;
using System.Linq;
using CAD.DTM.Configuration;

namespace CAD.DTM
{
    public class DtmImportPointsCtx : DataObjectBase<DtmImportCtx>
    {
        public DtmImportPointsCtx()
            : base(null, new StreamingContext())
        {
            var po = SingletonsBase.Registry.getEntry(Registry.SubKey.kCurrentUser, "Dtm/ImportPointsFileName");
            FileName = po.getString("");
            PointTypeElements = new ObservableCollection<string>();
            foreach (var element in DtmConfigurationSingleton.Instance.ElementSetting.Where(element => element.Value.ElementType == ElementType.Point))
            {
                PointTypeElements.Add(element.Key);
            }
            PointTypeSelected = PointTypeElements[0];
        }
        public void SaveToRegistry()
        {
            SingletonsBase.Registry.setEntry(Registry.SubKey.kCurrentUser, "Dtm/ImportPointsFileName", new ProgramOption(FileName));
        }

        public readonly PropertyData FileNameProperty = RegisterProperty("FileName", typeof(string), string.Empty);
        public string FileName
        {
            get => GetValue<string>(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }
        public readonly PropertyData _pointTypeElements = RegisterProperty("PointTypeElements", typeof(ObservableCollection<string>), null);
        public ObservableCollection<string> PointTypeElements
        {
            get => GetValue<ObservableCollection<string>>(_pointTypeElements);
            set => SetValue(_pointTypeElements, value);
        }
        public readonly PropertyData _pointTypeSelected = RegisterProperty("PointTypeSelected", typeof(string), null);
        public string PointTypeSelected
        {
            get => GetValue<string>(_pointTypeSelected);
            set => SetValue(_pointTypeSelected, value);
        }
        protected override void ValidateFields()
        {
        }
    }
}