using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace CAD.DTM.Validation.Rules
{
    public class DtmXsdValidationRule : DtmValidationRule
    {
        public override string Code => "1.1";
        public override string Name => "Kontrola struktury souboru JVF DTM / dodržení datového modelu (XSD)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            var xsdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DTM", "Doc", "xsd", "index", "index_data.xsd");
            if (ctx.Items.Count == 0)
            {
                result.IsSkipped = true;
                result.Note = "Žádná data k exportu.";
                return;
            }
            if (!File.Exists(xsdPath))
            {
                result.IsSkipped = true;
                result.Note = "XSD schémata nebyla nalezena: " + xsdPath;
                return;
            }
            var tmp = Path.Combine(Path.GetTempPath(), "dtm_validation_" + Guid.NewGuid().ToString("N") + ".xml");
            try
            {
                var exportCtx = new DtmExportCtx { FileName = tmp };
                var src = ctx.ExportCtx;
                if (src != null)
                {
                    exportCtx.NazevZakazky = src.NazevZakazky;
                    exportCtx.Zpracovatel = src.Zpracovatel;
                    exportCtx.OrganizaceZpracovatele = src.OrganizaceZpracovatele;
                    exportCtx.DatumMereni = src.DatumMereni;
                    exportCtx.DatumZpracovani = src.DatumZpracovani;
                    exportCtx.AZI = src.AZI;
                    exportCtx.DatumOvereni = src.DatumOvereni;
                    exportCtx.CisloOvereni = src.CisloOvereni;
                }
                new DtmExporter(ctx.Main).CreateFile(exportCtx);

                var schemas = new XmlSchemaSet { XmlResolver = new XmlUrlResolver() };
                schemas.Add("objtyp", xsdPath);
                var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = schemas };
                settings.ValidationEventHandler += (s, e) =>
                {
                    if (e.Severity == XmlSeverityType.Error)
                        result.AddError($"řádek {e.Exception?.LineNumber}", e.Message);
                };
                using (var reader = XmlReader.Create(tmp, settings))
                {
                    while (reader.Read()) { }
                }
            }
            catch (Exception ex)
            {
                result.AddError(null, ex.Message);
            }
            finally
            {
                try { File.Delete(tmp); } catch { }
            }
        }
    }

    public class DtmChangeAreaExistsRule : DtmValidationRule
    {
        public override string Code => "1.2";
        public override string Name => "Kontrola existence oblasti změny";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            if (ctx.GetChangeArea() != null)
                return;
            var pointsCount = ctx.Items.SelectMany(i => i.Geometries).SelectMany(DtmValidationContext.GetPoints).Count();
            if (pointsCount < 3)
                result.AddError(null, "Oblast změny neexistuje a nelze ji vytvořit (méně než 3 body).");
            else
                result.Note = "Oblast změny bude při exportu vytvořena jako konvexní obálka dat.";
        }
    }

    public class DtmExtentRule : DtmValidationRule
    {
        public override string Code => "1.3";
        public override string Name => "Kontrola extentu (umístění dat v rámci kraje)";

        protected override void Check(DtmValidationContext ctx, DtmRuleResult result)
        {
            result.IsSkipped = true;
            result.Note = "Zatím neimplementováno.";
        }
    }
}
