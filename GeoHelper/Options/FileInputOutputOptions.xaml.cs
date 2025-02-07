using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using GeoBase.Gui;
using GeoBase.Utils;
using GeoHelper.Utils;

namespace GeoHelper.Options
{
    public partial class FileInputOutputOptions : UserControlBase, IOptionItem
    {
        public FileInputOutputOptions()
        {
            InitializeComponent();
        }

        FileInputOutputOptionsContext _context;

        public IOptionContextItem Context
        {
            get { return _context; }
            set
            {
                _context = (FileInputOutputOptionsContext)value;
                DataContext = _context;
            }
        }

        void OnSave(object sender, EventArgs args)
        {
            var newItem = new FileInputOutputOptionsContext.OFItem
            {
                Name = _nazev.Text,
                Format = _format.Text,
                NedefinovaneNahraditNulami = _undefinedReplaceZero.IsChecked.Value
            };
            _context.Nodes.Add(newItem);
            _context.OnPropertyChanged("CanDelete");
        }

        public void OnDelete(object sender, EventArgs args)
        {
            _context.Nodes.Remove(_context.SelectedNode);
            if (_context.Nodes.Count > 0)
                _context.SelectedNode = _context.Nodes[0];
            _context.OnPropertyChanged("CanDelete");
        }

        public void OnPatternsSelectionChanged(object sender, EventArgs args)
        {
            if (_context.SelectedNode == null) return;
            _nazev.Text = _context.SelectedNode.Name;
            _undefinedReplaceZero.IsChecked = _context.SelectedNode.NedefinovaneNahraditNulami;
        }
    }

    public class FileInputOutputOptionsContext : IOptionContextItem, INotifyPropertyChanged
    {
        public FileInputOutputOptionsContext()
        {
            Nodes = new ObservableCollection<OFItem>();
        }

        public class OFItem : INotifyPropertyChanged
        {
            string _format;
            string _name;
            bool _nedefinovaneNahraditNulami;

            public OFItem()
            {
                Name = string.Empty;
                Format = string.Empty;
            }

            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }

            public string Format
            {
                get { return _format; }
                set
                {
                    _format = value;
                    OnPropertyChanged("Format");
                }
            }

            public bool NedefinovaneNahraditNulami
            {
                get { return _nedefinovaneNahraditNulami; }
                set
                {
                    _nedefinovaneNahraditNulami = value;
                    OnPropertyChanged("NedefinovaneNahraditNulami");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            void OnPropertyChanged(string property)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }

            public void Serialize(BinaryWriter writer)
            {
                writer.Write(Name);
                writer.Write(Format);
                writer.Write(NedefinovaneNahraditNulami);
            }

            public void Deserialize(BinaryReader reader)
            {
                Name = reader.ReadString();
                Format = reader.ReadString();
                NedefinovaneNahraditNulami = reader.ReadBoolean();
            }

            public OFItem GetCloneItem()
            {
                var newItem = new OFItem
                {
                    Name = Name,
                    Format = Format,
                    NedefinovaneNahraditNulami = NedefinovaneNahraditNulami
                };
                return newItem;
            }
        }

        string _parserPattern;
        OFItem _selectedNode;
        public ObservableCollection<OFItem> Nodes { get; set; }

        public OFItem SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                if (_selectedNode != null)
                    ParserPattern = _selectedNode.Format;
                OnPropertyChanged("SelectedNode");
            }
        }

        public string ParserPattern
        {
            get { return _parserPattern; }
            set
            {
                _parserPattern = value;
                OnPropertyChanged("ParserPattern");
            }
        }

        public bool CanDelete
        {
            get { return _selectedNode != null; }
            set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public void LoadFromRegistry()
        {
            ProgramOption op = Singletons.MyRegistry.getEntry("VstupVystupSouradniceParserPattern");
            ParserPattern = op.getString("");
            op = Singletons.MyRegistry.getEntry("VstupVystupSouradniceContextItem");
            if (op.isString())
            {
                string s = op.getString();
                if (s == null || s.Length == 0)
                    return;
                var m = new MemoryStream();
                var sw = new StreamWriter(m);
                sw.Write(s);
                sw.Flush();
                m.Position = 0;
                var reader = new BinaryReader(m);
                Int32 count = reader.ReadInt32();
                for (Int32 i = 0; i < count; i++)
                {
                    var of = new OFItem();
                    of.Deserialize(reader);
                    Nodes.Add(of);
                    if (ParserPattern == of.Format)
                    {
                        SelectedNode = of;
                    }
                }
            }
            else
            {
                Nodes.Add(new OFItem
                {
                    Name = "S předčíslím",
                    Format = "<Num:12> <SobrX:6.2> <SobrY:7.2> <SobrZ:4.2> <SobrPrec:1>"
                });
                SelectedNode = Nodes[0];
            }
        }

        public void SaveToRegistry()
        {
            var stream = new MemoryStream();
            var bw = new BinaryWriter(stream);
            bw.Write(Nodes.Count);
            foreach (OFItem item in Nodes)
            {
                item.Serialize(bw);
            }
            bw.Flush();
            stream.Position = 0;
            var sr = new StreamReader(stream);
            string s = sr.ReadToEnd();
            Singletons.MyRegistry.setEntry("VstupVystupSouradniceContextItem", new ProgramOption(s));
            Singletons.MyRegistry.setEntry("VstupVystupSouradniceParserPattern", new ProgramOption(ParserPattern));
        }
    }
}