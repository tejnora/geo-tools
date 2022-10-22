using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GeoBase.Localization;
using GeoBase.Utils;
using GeoHelper.FileParses;

namespace GeoHelper.Tables.TableNodes
{
    public class TableNodesBase : DataObjectBase<TableNodesBase>
    {
        public TableNodesBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public TableNodesBase()
            : base(null, new StreamingContext())
        {
        }

        readonly PropertyData _numberProperty = RegisterProperty("Number", typeof(string), string.Empty);
        public string Number
        {
            get { return GetValue<string>(_numberProperty); }
            set
            {
                SetValue(_numberProperty, value);
                OnPropertyChanged("NumberWithPrefix");
            }
        }

        readonly PropertyData _prefixProperty = RegisterProperty("Prefix", typeof(string), string.Empty);
        public string Prefix
        {
            get { return GetValue<string>(_prefixProperty); }
            set
            {
                SetValue(_prefixProperty, value);
                OnPropertyChanged("NumberWithPrefix");
            }
        }

        readonly PropertyData _descriptionProperty = RegisterProperty("Description", typeof(string), string.Empty);
        public string Description
        {
            get { return GetValue<string>(_descriptionProperty); }
            set { SetValue(_descriptionProperty, value); }
        }

        readonly PropertyData _lockedProperty = RegisterProperty("Locked", typeof(bool), false);
        public bool Locked
        {
            get { return GetValue<bool>(_lockedProperty); }
            set
            {
                SetValue(_lockedProperty, value);
                OnPropertyChanged("LockImage");
            }
        }

        readonly PropertyData _selectedProperty = RegisterProperty("Selected", typeof(bool), false);
        public bool Selected
        {
            get { return GetValue<bool>(_selectedProperty); }
            set
            {
                SetValue(_selectedProperty, value);
                OnPropertyChanged("SelectedRowBackground");
            }
        }

        BitmapImage _lockImageSrc;
        public string NumberWithPrefix
        {
            get { return string.Format("{0:00000000}{1:0000}", Prefix, Number); }
            set
            {
                try
                {
                    if (value.Length <= 4)
                    {
                        Number = value;
                        return;
                    }
                    Number = value.Substring(value.Length - 4);
                    Prefix = value.Substring(0, value.Length - 4);
                }
                catch (Exception)
                {
                    var par = new ResourceParams();
                    par.Add("value", value);
                    LanguageConverter.ResolveDictionary().ShowMessageBox("Texts.1", par, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ImageSource LockImage
        {
            get
            {
                if (!Locked)
                    return null;
                if (_lockImageSrc == null)
                {
                    _lockImageSrc = new BitmapImage();
                    _lockImageSrc.BeginInit();
                    _lockImageSrc.UriSource = new Uri("pack://application:,,,/Images/zamek.png", UriKind.Absolute);
                    _lockImageSrc.CacheOption = BitmapCacheOption.OnLoad;
                    _lockImageSrc.EndInit();
                }
                return _lockImageSrc;
            }
            set { }
        }

        public Brush SelectedRowBackground
        {
            get
            {
                return Selected ? Brushes.LightGray : Brushes.White;
            }
        }

        public string NumberWithPrefixForSorting
        {
            get { return Prefix.PadLeft(8, '0') + Number.PadLeft(4, '0'); }
        }

        public virtual void AssignNode(TableNodesBase aNode)
        {
            Prefix = aNode.Prefix;
            Number = aNode.Number;
            Description = aNode.Description;
            Selected = aNode.Selected;
            Locked = aNode.Locked;
        }

        public virtual void Serialize(BinaryWriter w)
        {
            w.Write(Prefix);
            w.Write(Number);
            w.Write(Description);
            w.Write(Selected);
            w.Write(Locked);
        }

        public virtual void Deserialize(BinaryReader r)
        {
            Prefix = r.ReadString();
            Number = r.ReadString();
            Description = r.ReadString();
            Selected = r.ReadBoolean();
            Locked = r.ReadBoolean();
        }

        public virtual bool SetValue(SymbolToken propertyToken, string propertyValue)
        {
            switch (propertyToken.Symbol)
            {
                case "Num":
                    {
                        if (propertyValue.Length <= 4)
                        {
                            Number = propertyValue;
                            return true;
                        }
                        Number = propertyValue.Substring(propertyValue.Length - 4);
                        Prefix = propertyValue.Substring(0, propertyValue.Length - 4);
                        return true;
                    }
                case "P":
                    {
                        Prefix = propertyValue;
                        return true;
                    }
                case "N":
                    {
                        Number = propertyValue;
                        return true;
                    }
                case "Des":
                    {
                        Description = propertyValue;
                        return true;
                    }
            }
            return false;
        }

        public virtual bool GetValue(SymbolToken propertyToken, out string value)
        {
            switch (propertyToken.Symbol)
            {
                case "Num":
                    {
                        value = propertyToken.ToString(Prefix + Number);
                        return true;
                    }
                case "P":
                    {
                        value = propertyToken.ToString(Prefix);
                        return true;
                    }
                case "N":
                    {
                        value = propertyToken.ToString(Number);
                        return true;
                    }
                case "Des":
                    {
                        value = propertyToken.ToString(Description);
                        return true;
                    }
            }
            value = string.Empty;
            return false;
        }
    }
}