using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using CAD.Canvas;
using CAD.DTM.Configuration;
using CAD.DTM.Elements.GUI;
using CAD.DTM.Gui;
using CAD.VFK;

namespace CAD.DTM.Elements
{
    public abstract class DtmElement
    : IDtmElement
    {
        internal CAD.DTM.Validation.DtmValidationSnapshot ValidationOriginal { get; private set; }
        internal void CaptureValidationOriginal()
        {
            if (ValidationOriginal == null && (ZapisObjektu == 'r' || ZapisObjektu == 'd'))
                ValidationOriginal = new CAD.DTM.Validation.DtmValidationSnapshot(this);
        }

        public char ZapisObjektu { get; set; }
        public IDtmGeometryGroup Geometry { get; set; }
        public DtmElementSpolecneAtributy SpolecneAtributy { get; set; }
        public DtmSpolecneAtributyZPS SpolecneAtributyZPS { get; set; }
        public DtmSchemaAttributes SchemaAttributes { get; private set; }
        public DtmElementOption Options { get; private set; }
        public virtual void InitGUICustomProperties(IDtmCustomElementProperties properties)
        {
        }
        public virtual DtmGraphicElement GetGraphicElement(DtmElementOption dtmElement, DtmGraphicElementScaleEnum scale)
        {
            return dtmElement.GetGraphicElement("_", scale);
        }

        public DtmGraphicElement ResolveGraphicElement(DtmElementOption option, DtmGraphicElementScaleEnum scale)
        {
            if (option.GraphicAttribute == null) return GetGraphicElement(option, scale);
            SchemaAttributes?.SynchronizeLegacyValues();
            var value = SchemaAttributes?.Get<string>(option.GraphicAttribute);
            return option.GetGraphicElement(value ?? "_", scale);
        }

        public DtmSpolecneAtributyObjektuZPS_TI SpolecneAtributyObjektuZPS_TI { get; set; }
        public IDrawObject CreateDrawObject()
        {
            switch (ElementType)
            {
                case DtmElementType.Bod:
                    return new DtmDrawingPointElement(this);
                case DtmElementType.Linie:
                    return new DtmDrawingLineElement(this);
                case DtmElementType.DefinicniBod:
                    return new DtmDrawingDefinitionPointElement(this);
                case DtmElementType.Plocha:
                    return new DtmDrawingPlochaElement(this);
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        public abstract DtmElementType ElementType { get; }

        public bool IsDeleted { get; set; } = false;
        public bool ExportToOutput
        {
            get
            {
                if (ZapisObjektu == 'r')
                    return IsDeleted;
                return !IsDeleted && ZapisObjektu != 'r';
            }
        }

        public bool IsReferencePoint => ZapisObjektu == 'r';
        static IEnumerable<string> _settingsEmpty = new List<string>();
        public virtual IEnumerable<string> Settings => _settingsEmpty;
        public virtual void SelectedSetting(string value) { }

        public void ApplySelectedSetting(string value)
        {
            SelectedSetting(value);
            if (string.IsNullOrEmpty(value) || SchemaAttributes == null) return;
            var settings = Settings.ToArray();
            foreach (var property in GetType().GetProperties().Where(p => p.PropertyType.IsEnum && Enum.GetNames(p.PropertyType).SequenceEqual(settings)))
                if (DtmSchema.Instance.Attributes.ContainsKey(property.Name))
                    SchemaAttributes.SetText(property.Name, Convert.ToInt64(property.GetValue(this)).ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public virtual void ExportAttributesToDtm(IDtmExporter exporter)
        {
            if (SchemaAttributes == null) throw new InvalidOperationException("Objekt nemá definici XSD.");
            SchemaAttributes.Export(exporter, false);
        }

        public virtual void ImportDtmAttributes(XmlElement xmlElement)
        {
            SchemaAttributes?.Import(xmlElement);
            ImportSpolecneAtributyVsechObjektu(xmlElement);
            ImportSpolecneAtributyZPS(xmlElement);
            ImportSpolecneAtributyObjektuZPS_TI(xmlElement);
            SchemaAttributes?.ApplyToLegacyValues();
        }
        public string EvaluateZapisObjektuForExportToDtm()
        {
            if (IsDeleted)
            {
                if (ZapisObjektu != 'r')
                    throw new ArgumentOutOfRangeException();
                return "d";
            }
            if (ZapisObjektu == 'r')
                throw new ArgumentOutOfRangeException();
            return ZapisObjektu.ToString();
        }
        public char EvaluateZapisObjektuForGui()
        {
            return IsDeleted ? 'd' : ZapisObjektu;
        }

        public void ExportSpolecneAtributyVsechObjektu(IDtmExporter exporter)
        {
            if (SchemaAttributes != null)
            {
                foreach (var element in SchemaAttributes.ToXml().Where(e => e.Name.LocalName == "SpolecneAtributyVsechObjektu"))
                    DtmSchemaAttributes.WriteElement(exporter, element);
                return;
            }
            exporter.BeginElement("atr", "SpolecneAtributyVsechObjektu", true);
            exporter.AddElement(null, "DatumVkladu", SpolecneAtributy.DatumVkladu);
            exporter.AddElement(null, "DatumZmeny", SpolecneAtributy.DatumZmeny);
            if (ZapisObjektu == 'r')
            {
                exporter.AddElement(null, "ID", SpolecneAtributy.ID);
                exporter.AddElement(null, "IDEditora", SpolecneAtributy.IDEditora);
                exporter.AddElement(null, "IDZmeny", SpolecneAtributy.IDZmeny);
                exporter.AddElement(null, "PopisObjektu", SpolecneAtributy.PopisObjektu);
                exporter.AddElement(null, "VkladOsoba", SpolecneAtributy.VkladOsoba);
                exporter.AddElement(null, "ZmenaOsoba", SpolecneAtributy.ZmenaOsoba);
            }
            exporter.EndElement();
        }
        protected void ImportSpolecneAtributyVsechObjektu(XmlElement xmlElement)
        {
            xmlElement = DtmImporter.FindElement(xmlElement, "SpolecneAtributyVsechObjektu");
            if (xmlElement == null)
                return;
            var atributy = new DtmElementSpolecneAtributy();
            foreach (XmlElement e in xmlElement)
            {
                switch (e.LocalName)
                {
                    case "DatumVkladu":
                        atributy.DatumVkladu = DateTime.Parse(e.InnerText);
                        break;
                    case "DatumZmeny":
                        atributy.DatumZmeny = DateTime.Parse(e.InnerText);
                        break;
                    case "ID":
                        atributy.ID = e.InnerText;
                        break;
                    case "IDEditora":
                        atributy.IDEditora = e.InnerText;
                        break;
                    case "IDZmeny":
                        atributy.IDZmeny = e.InnerText;
                        break;
                    case "PopisObjektu":
                        atributy.PopisObjektu = e.InnerText;
                        break;
                    case "VkladOsoba":
                        atributy.VkladOsoba = e.InnerText;
                        break;
                    case "ZmenaOsoba":
                        atributy.ZmenaOsoba = e.InnerText;
                        break;
                }
            }
            SpolecneAtributy = atributy;
        }
        protected void ExportSpolecneAtributyObjektuZPS_TI(IDtmExporter exporter)
        {
            exporter.BeginElement("atr", "SpolecneAtributyObjektuZPS_TI", true);
            exporter.AddElement(null, "UrovenUmisteniObjektuTI", SpolecneAtributyObjektuZPS_TI.UrovenUmisteniObjektuTI);
            exporter.AddElement(null, "TridaPresnostiPoloha", SpolecneAtributyObjektuZPS_TI.TridaPresnostiPoloha);
            exporter.AddElement(null, "TridaPresnostiVyska", SpolecneAtributyObjektuZPS_TI.TridaPresnostiVyska);
            exporter.AddElement(null, "ZpusobPorizeniTI", SpolecneAtributyObjektuZPS_TI.ZpusobPorizeniTI);
            exporter.AddElement(null, "StavObjektu", (int)SpolecneAtributyObjektuZPS_TI.StavObjektu);
            exporter.EndElement();

        }
        protected void ImportSpolecneAtributyObjektuZPS_TI(XmlElement xmlElement)
        {
            xmlElement = DtmImporter.FindElement(xmlElement, "SpolecneAtributyObjektuZPS_TI");
            if (xmlElement == null)
                return;
            var atributy = new DtmSpolecneAtributyObjektuZPS_TI();
            foreach (XmlElement e in xmlElement)
            {
                switch (e.LocalName)
                {
                    case "UrovenUmisteniObjektuTI":
                        atributy.UrovenUmisteniObjektuTI = int.Parse(e.InnerText);
                        break;
                    case "TridaPresnostiPoloha":
                        atributy.TridaPresnostiPoloha = int.Parse(e.InnerText);
                        break;
                    case "TridaPresnostiVyska":
                        atributy.TridaPresnostiVyska = int.Parse(e.InnerText);
                        break;
                    case "ZpusobPorizeniTI":
                        atributy.ZpusobPorizeniTI = int.Parse(e.InnerText);
                        break;
                    case "StavObjektu":
                        atributy.StavObjektu = (DtmStavObjektuEnum)int.Parse(e.InnerText);
                        break;
                }
            }
            SpolecneAtributyObjektuZPS_TI = atributy;

        }
        protected void ExportSpolecneAtributyObjektuZPS(IDtmExporter exporter)
        {
            exporter.BeginElement("atr", "SpolecneAtributyObjektuZPS", true);
            exporter.AddElement(null, "TridaPresnostiPoloha", SpolecneAtributyZPS.TridaPresnostiPoloha);
            exporter.AddElement(null, "TridaPresnostiVyska", SpolecneAtributyZPS.TridaPresnostiVyska);
            exporter.AddElement(null, "UrovenUmisteniObjektuZPS", SpolecneAtributyZPS.UrovenUmisteniObjektuZPS);
            exporter.AddElement(null, "ZpusobPorizeniZPS", SpolecneAtributyZPS.ZpusobPorizeniZPS);
            exporter.EndElement();
        }
        protected void ImportSpolecneAtributyZPS(XmlElement xmlElement)
        {
            xmlElement = DtmImporter.FindElement(xmlElement, "SpolecneAtributyObjektuZPS");
            if (xmlElement == null)
                return;
            var atributy = new DtmSpolecneAtributyZPS();
            foreach (XmlElement e in xmlElement)
            {
                switch (e.LocalName)
                {
                    case "UrovenUmisteniObjektuZPS":
                        atributy.UrovenUmisteniObjektuZPS = int.Parse(e.InnerText);
                        break;
                    case "TridaPresnostiPoloha":
                        atributy.TridaPresnostiPoloha = int.Parse(e.InnerText);
                        break;
                    case "TridaPresnostiVyska":
                        atributy.TridaPresnostiVyska = int.Parse(e.InnerText);
                        break;
                    case "ZpusobPorizeniZPS":
                        atributy.ZpusobPorizeniZPS = int.Parse(e.InnerText);
                        break;
                }
            }
            SpolecneAtributyZPS = atributy;
        }

        public string ZapisObjektuPopis
        {
            get
            {
                switch (ZapisObjektu)
                {
                    case 'i': return "Insert";
                    case 'u': return "Update";
                    case 'd': return "Delete";
                    case 'r': return "Reference";
                }
                throw new NotImplemented();
            }
        }

        public virtual void Init(DtmElementOption dtmElementOption)
        {
            Options = dtmElementOption;
            if (dtmElementOption.Name != null && DtmSchema.Instance.Objects.TryGetValue(dtmElementOption.Name, out var definition))
            {
                SchemaAttributes = new DtmSchemaAttributes(this, definition);
                if (definition.Attributes.Any(a => a.Name == "SpolecneAtributyObjektuZPS")) SpolecneAtributyZPS = new();
                if (definition.Attributes.Any(a => a.Name == "SpolecneAtributyObjektuZPS_TI")) SpolecneAtributyObjektuZPS_TI = new();
            }
            ZapisObjektu = 'i';
            var dateTime = DateTime.Now;
            SpolecneAtributy = new DtmElementSpolecneAtributy
            {
                DatumVkladu = dateTime,
                DatumZmeny = dateTime
            };
            if (SchemaAttributes != null)
            {
                SchemaAttributes.Set("SpolecneAtributyVsechObjektu/DatumVkladu", dateTime);
                SchemaAttributes.Set("SpolecneAtributyVsechObjektu/DatumZmeny", dateTime);
            }
        }

        public virtual string GetInfoAsString()
        {
            return "";
        }
    }
}
