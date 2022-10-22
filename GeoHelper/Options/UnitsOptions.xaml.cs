using System.Runtime.Serialization;
using GeoBase.Gui;
using GeoBase.Utils;
using GeoHelper.Utils;

namespace GeoHelper.Options
{
    public partial class UnitsOptions : UserControlBase, IOptionItem
    {
        public UnitsOptions()
        {
            InitializeComponent();
            DataContext = this;
        }

        UnitsOptionsContext _context;

        public IOptionContextItem Context
        {
            get { return _context; }
            set { _context = (UnitsOptionsContext) value; }
        }
    }

    public class UnitsOptionsContext : DataObjectBase<UnitsOptionsContext>, IOptionContextItem
    {
        public UnitsOptionsContext()
            : base(null, new StreamingContext())
        {
        }

        public readonly PropertyData _delkyProperty = RegisterProperty("Delky", typeof (int), 0);
        public readonly PropertyData _plochyProperty = RegisterProperty("Plochy", typeof (int), 0);
        public readonly PropertyData _souradniceProperty = RegisterProperty("Souradnice", typeof (int), 0);
        public readonly PropertyData _uhlyProperty = RegisterProperty("Uhly", typeof (int), 0);

        public readonly PropertyData _vyskyProperty = RegisterProperty("Vysky", typeof (int), 0);

        public int Souradnice
        {
            get { return GetValue<int>(_souradniceProperty); }
            set { SetValue(_souradniceProperty, value); }
        }

        public int Vysky
        {
            get { return GetValue<int>(_vyskyProperty); }
            set { SetValue(_vyskyProperty, value); }
        }

        public int Delky
        {
            get { return GetValue<int>(_delkyProperty); }
            set { SetValue(_delkyProperty, value); }
        }

        public int Uhly
        {
            get { return GetValue<int>(_uhlyProperty); }
            set { SetValue(_uhlyProperty, value); }
        }

        public int Plochy
        {
            get { return GetValue<int>(_plochyProperty); }
            set { SetValue(_plochyProperty, value); }
        }

        public void LoadFromRegistry()
        {
            ProgramOption op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser,
                                                              "VstypVystupSouradnicePocetDesMist");
            Souradnice = op.getInt(2);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupVyskyPocetDesMist");
            Vysky = op.getInt(2);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupDelkyPocetDesMist");
            Delky = op.getInt(2);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupUhelPocetDesMist");
            Uhly = op.getInt(4);
            op = Singletons.MyRegistry.getEntry(Registry.SubKey.kCurrentUser, "VstypVystupPlochyPocetDesMist");
            Plochy = op.getInt(2);
        }

        public void SaveToRegistry()
        {
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "VstypVystupSouradnicePocetDesMist",
                                           new ProgramOption(Souradnice));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "VstypVystupVyskyPocetDesMist",
                                           new ProgramOption(Vysky));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "VstypVystupDelkyPocetDesMist",
                                           new ProgramOption(Delky));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "VstypVystupUhelPocetDesMist",
                                           new ProgramOption(Uhly));
            Singletons.MyRegistry.setEntry(Registry.SubKey.kCurrentUser, "VstypVystupPlochyPocetDesMist",
                                           new ProgramOption(Plochy));
        }
    }
}