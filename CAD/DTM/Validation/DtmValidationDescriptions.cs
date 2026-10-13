using System.Collections.Generic;

namespace CAD.DTM.Validation
{
    // Popisy kontrol zobrazovane v dialogu jako tooltip (podle DTM wiki, kap. 1.4); klic = nazev tridy pravidla,
    // u preskocenych kontrol "Skipped:" + kod.
    public static class DtmValidationDescriptions
    {
        static readonly Dictionary<string, string> Map = new Dictionary<string, string>
        {
            ["DtmExchangeFormatGroupRule"] = "1.4.1 Kontroly výměnného formátu.\nChyby v této skupině jsou důvodem k odmítnutí aktualizační dokumentace.",
            ["DtmXsdValidationRule"] = "1.1 Kontrola struktury souboru JVF DTM / dodržení datového modelu.\n\n" +
                "Exportovaný soubor se validuje proti platným definičním souborům XSD. Nevalidní soubory nemohou být naimportovány do IS DTM a jsou chybou opravňující k odmítnutí aktualizační dokumentace.",
            ["DtmChangeAreaExistsRule"] = "1.2 Kontrola existence oblasti změny.\n\n" +
                "Kontrola ověřuje, zda v předávané aktualizační dokumentaci existuje polygon vymezující oblast změny. Neexistence polygonu změny je chyba opravňující k odmítnutí aktualizační dokumentace.\n\n" +
                "Pokud oblast změny není zadána, vytváří se při exportu z exportovaných bodů (konvexní obálka).",
            ["DtmExtentRule"] = "1.3 Kontrola extentu (umístění dat v rámci kraje).\n\n" +
                "Kontrola ověřuje, zda rozsah „Oblasti změny“ alespoň částečně zasahuje do území příslušného kraje. Oblast změny mimo území kraje je chyba opravňující k odmítnutí aktualizační dokumentace.\n\n" +
                "Zatím se neprovádí (nejsou k dispozici hranice krajů).",
            ["DtmGeometryRule"] = "1.4 Kontrola geometrií.\n\n" +
                "Kontrola ověřuje, zda jsou v souboru pouze povolené typy geometrií (bod, linie, plocha). Nepovolené typy geometrie jsou chybou opravňující k odmítnutí dokumentace. " +
                "Oblouky nejsou umožněny, kromě objektů DI a TI – zóny nejistoty, ochranná pásma a bezpečnostní pásma, kde jsou oblouky povoleny.\n\nKontrola se skládá z podskupin níže.",
            ["DtmGeometryExistsRule"] = "Každý exportovaný objekt musí mít geometrii.",
            ["DtmGeometryAllowedTypesRule"] = "Povolené jsou pouze geometrie bod, linie a plocha. Oblouky nejsou povoleny (výjimka: DI a TI – zóny nejistoty, ochranná a bezpečnostní pásma).",
            ["DtmGeometryMatchesElementRule"] = "Typ geometrie musí odpovídat typu objektu (bod – bodová, linie – liniová, plocha – plošná, definiční bod – bodová).",
            ["DtmPointGeometryRule"] = "Bodová geometrie musí obsahovat právě jeden bod se souřadnicemi X, Y (a Z).",
            ["DtmLineGeometryRule"] = "Liniová geometrie musí obsahovat nejméně 2 body.",
            ["DtmPolygonGeometryRule"] = "Plošná geometrie musí být uzavřená (první bod = poslední) a mít nejméně 4 body.",
            ["DtmCoordinatesRule"] = "1.5 Kontrola souřadnic (X, Y, Z).\n\n" +
                "Kontrola ověřuje, zda jsou souřadnice X, Y všech objektů umístěny ve 3. kvadrantu S-JTSK (EPSG: 5514) v definovaném rozsahu hodnot. " +
                "Pro ZPS je kontrolován povolený rozsah souřadnice Z (1–1620 m, definiční body 0–1620 m). Pro DTI se rozsah Z nekontroluje.\n\n" +
                "Souřadnice mimo povolený rozsah nebo mimo 3. kvadrant jsou chyba opravňující k odmítnutí dokumentace.\n" +
                "Povolené rozsahy: wiki 1.3.2 Minimální rozměry a tolerance.",
            ["DtmCoordinatesPrecisionRule"] = "1.6 Kontrola přesnosti souřadnic na cm.\n\n" +
                "Kontrola ověřuje, zda jsou souřadnice všech objektů uvedeny s přesností na centimetry. Souřadnice vedené s vyšší přesností (mm a menší jednotky) jsou chyba opravňující k odmítnutí dokumentace.",
            ["DtmChangesInsideAreaRule"] = "1.7 Kontrola umístění změn v JVF DTM v oblasti změny.\n\n" +
                "Kontrola ověřuje, zda geometrie aktualizovaných objektů (insert, update, delete) leží celá uvnitř nebo na hranici oblasti změny, a to původní i nový stav. " +
                "Výskyt aktualizovaných dat mimo „Oblast změny“ je chyba opravňující k odmítnutí dokumentace.\n\n" +
                "Aktualizovanými objekty jsou podrobné body, bodové objekty, liniové objekty a definiční body.\n" +
                "Kontroluje se jen aktuální geometrie (původní stav se nepamatuje).",

            ["DtmAttributesGroupRule"] = "1.4.2 Atributové kontroly.",
            ["DtmAttributesRule"] = "2.1 Kontrola atributů.\n\n" +
                "Atributová kontrola zajišťuje kontrolu vyplnění povinných hodnot, souladu s číselníky, syntaxe systémových atributů a datumu u datumových polí.\n\n" +
                "Kontrola probíhá při přijímání geodetické aktualizační dokumentace. Třída přesnosti nových PB se nekontroluje, pokud je v doprovodných informacích Konsolidace=1.",
            ["DtmAziRule"] = "Povinný atribut „AZI“ v doprovodných informacích se vyplňuje jako číslo oprávnění bez lomítka a roku vydání.",
            ["DtmNewPointAccuracyClassRule"] = "V souladu s vyhláškou 393/2020 Sb. musí být nové podrobné body (insert) pořizovány alespoň ve třídě přesnosti 3 v poloze i ve výšce. Výsledkem je chyba.",
            ["DtmMinimalDateRule"] = "Datumová pole (DatumVkladu, DatumMereni, DatumZpracovani, DatumOvereni apod.): nejstarší možné datum je 1900-01-01. Dokumentace se starším datem bude vrácena k opravě.",
            ["DtmCodelistRule"] = "Kontrola souladu s číselníky: třída přesnosti v poloze a výšce 1–5, úroveň umístění (LEVEL) -3 až 3.",
            ["DtmIcsRule"] = "2.2 Kontrola IČS.\n\nKontrola dosud nebyla spuštěna z důvodu nepovinného přidělování IČS v digitalizaci stavebního řízení a neexistujícího rozhraní pro propojení systémů.",
            ["DtmHeightAccuracyClassRule"] = "2.3 Kontrola vyplnění třídy přesnosti ve výšce ZPS.\n\n" +
                "Na všech podrobných bodech ZPS (0100000218) v režimu INSERT musí být vyplněna hodnota třídy přesnosti ve výšce (nesmí být prázdná). Chybějící hodnota je chyba opravňující k odmítnutí dokumentace.",
            ["DtmUnknownLineTypeRule"] = "2.4 Kontrola výskytu linií typu nezjištěno nebo jiný.\n\n" +
                "V režimu insert nebo update nesmí být v cílovém stavu liniový objekt s hodnotou typu jiný/nezjištěno. Výskyt je chyba opravňující k odmítnutí dokumentace.\n\n" +
                "Týká se objektů:\n• 0100000300 hranice stavby – nezjištěno\n• 0100000304 hranice dopravní stavby nebo plochy – nezjištěno, jiný\n" +
                "• 0100000306 hranice vodního díla – nezjištěno\n• 0100000305 hranice přírodního a polopřírodního objektu – nezjištěno\n• 0100000307 hranice ostatní plochy – nezjištěno",
            ["DtmTpZpAttributeRule"] = "2.5 Kontrola správnosti vyplnění atributu TP a ZP.\n\nPravidla nejsou na wiki popsána, kontrola se neprovádí.",

            ["DtmTopologyGroupRule"] = "1.4.3 Topologické kontroly.",
            ["DtmVertexPodrobnyBodRule"] = "3.1 Závislost objektů na podrobných bodech.\n\n" +
                "Pro každý vrchol geometrického objektu musí existovat podrobný bod ZPS (0100000218) se shodnou souřadnicí X, Y i Z ve stejné úrovni umístění (LEVEL). Kontrola se provádí ve 3D, samostatně pro každý LEVEL. " +
                "Chybějící podrobný bod je chyba.\n\n" +
                "PB sdílený mezi úrovněmi se eviduje jen jednou, v úrovni nejblíž povrchu.\nNekontrolují se objekty výškový bod na terénu (0100000219) a identický bod (0100000220).",
            ["DtmLineOverlapRule"] = "3.2 Kolize prvků – překryv.\n\n" +
                "Liniové objekty ve stejném levelu se v definované skupině nesmí překrývat. Překryv je stejný průběh dvou linií, jehož délka je větší než nula; linie mohou mít obrácené pořadí vrcholů. Kontrola ve 2D po levelech, překryv je chyba.\n\n" +
                "Za překryv se nepovažují zcela identické objekty (řeší 3.6 Duplicity). Samostatně se kontrolují: konstrukční linie (vstupující do odvozených ploch), mostní konstrukce, provozní plocha tunelu, provozní plocha podchodu. Svislé hrany nevstupují.",
            ["DtmLineCrossingRule"] = "3.3 Kolize prvků – křížení.\n\n" +
                "Každá linie může začínat nebo končit pouze v počátečním nebo koncovém bodu jiné linie (ne na lomovém bodě). Jinak jde o nepovolené křížení. Křížení je bodový průnik s nulovou délkou. " +
                "Kontrola ve 2D, samostatně pro každý LEVEL a skupinu (konstrukční linie, most, tunel, podchod). Svislé hrany nevstupují. Křížení je chyba.",
            ["DtmLineSelfCrossingRule"] = "3.4 Kolize prvků – křížení sebe sama.\n\n" +
                "Prvek nesmí sám sebe křížit ani překrývat. Povoleno je pouze umístění počátečního a koncového bodu ve stejném bodě. Kontrola ve 2D, chyba.",
            ["DtmZeroSegmentLengthRule"] = "3.5 Nulová délka.\n\nKontrola liniových objektů ZPS. Délka segmentu ve 3D nesmí být rovna 0. Segment o nulové délce je chyba.",
            ["DtmLineDuplicityRule"] = "3.6 Duplicity prvků.\n\n" +
                "Kontrola liniových objektů ve stejném levelu s identickými vrcholy na úrovni XY (i s obráceným pořadím vrcholů). Duplicitní jsou prvky, u nichž jsou všechny rozdíly Z na stejných XY menší než povolený rozdíl Z (0,12 m). " +
                "Jsou-li XYZ stejné, jde o chybu; je-li rozdíl Z menší než povolená hodnota, jde o varování.\n\n" +
                "Samostatně pro konstrukční linie, most, tunel, podchod apod. Svislé hrany nevstupují.",
            ["DtmPointDuplicityRule"] = "3.8 Duplicita bodů.\n\n" +
                "Vrchol XYZ musí být unikátní. Podrobné body ZPS a bodové objekty ZPS se kontrolují ve 3D napříč všemi úrovněmi (levely); definiční body ve 2D po jednotlivých levelech. Duplicita je chyba.\n\n" +
                "Nekontrolují se identické body (0100000220) a výškové body na terénu (0100000219).",
            ["DtmPointProximityRule"] = "3.9 Blízkost bodů (bodových objektů).\n\n" +
                "V definované skupině objektů se nesmí vyskytovat 2 nebo více objektů ve vzdálenosti menší než 0,05 m. Kontrola ve 3D. Bližší objekty jsou označeny varováním.",
            ["DtmShortSegmentRule"] = "3.10 Minimální délky.\n\nKontrola liniových objektů. Varování, pokud je délka segmentu linie větší než 0 a zároveň menší než 0,05 m. Kontrola ve 3D.",
            ["DtmSolitaryPodrobnyBodRule"] = "3.11 Solitérní podrobné body ZPS.\n\n" +
                "Každý podrobný bod ZPS (0100000218) musí být použit minimálně jedním bodovým nebo liniovým prvkem ve stejné úrovni umístění (LEVEL). Kontrola ve 3D a pouze v Oblasti změny. Solitérní PB je chyba.\n\n" +
                "Vztah PB k výškovému bodu na terénu a identickému bodu se nezohledňuje (PB pod samostatným identickým bodem je solitérní).",
            ["DtmPointToLineDistanceRule"] = "3.13 Minimální vzdálenost bodu od linie.\n\nPodle wiki popis kontroly zatím není vyplněn. Implementováno jako varování, pokud je bod od linie (mimo její vrcholy) blíž než 0,05 m ve 3D.",

            ["DtmDerivedObjectsGroupRule"] = "1.4.4 Kontroly odvozených objektů.",
            ["DtmMinimalAreaRule"] = "4.1 Minimální velikost ploch.\n\nKontrola plošných objektů ve 2D. Plocha musí být větší než definovaná hodnota (0,05 m²). Menší plocha je chyba.",
            ["DtmAreaDefinitionPointRule"] = "4.2 Plocha s více definičními body / 4.3 Plocha bez definičního bodu.\n\n" +
                "Každá plocha musí mít právě jeden definiční bod ve stejném levelu. Pokud nemá žádný nebo má více než jeden, je to chyba. Kontrola ve 2D.",
            ["DtmAreaCollisionRule"] = "4.5 Kolize ploch.\n\nPlochy se nesmí překrývat v rámci jednoho levelu. Kontrola ve 2D, překryv je chyba. " +
                "Samostatně pro běžné plochy, plochu mostní konstrukce, provozní plochu tunelu a podchodu a podzemní objekt.",
            ["DtmDefinitionPointWithoutAreaRule"] = "4.7 Definiční bod bez plochy.\n\nKaždý definiční bod musí ležet v jedné ploše stejného levelu. Kontrola ve 2D. Definiční bod bez plochy je chyba.\n\n" +
                "Wiki ukládá kontrolu jen v oblasti kompletní ZPS; zde se kontrolují všechny definiční body.",

            ["Skipped:3.7"] = "3.7 Volné konce.\n\nKontrola volných konců liniových objektů vstupujících do tvorby odvozených objektů. Volný konec je chyba (konstrukční linie podzemního objektu vždy jen varování). Probíhá po jednotlivých levelech (-3 až 3) a pouze v „Oblasti kompletní ZPS“.",
            ["Skipped:3.12"] = "3.12 Kontrola obvodu polygonu Oblasti kompletní ZPS.\n\nObvod polygonu Oblasti kompletní ZPS musí být vrcholově identický s průběhem příslušných konstrukčních linií ZPS. Kontrola ve 2D, pro každý level zvlášť; hranice neprobíhající po liniích ZPS je chyba.",
            ["Skipped:4.4"] = "4.4 Plocha s chybným ohraničením.\n\nPro každý typ plošného objektu je dáno, jaké konstrukční linie mohou tvořit jeho hranici (hierarchie ZPS, wiki 1.3.1). Linie musí mít hierarchii stejnou nebo vyšší než plocha; jako hranici lze použít i neidentifikovaný objekt (0100000202) nebo linii s HraniceJinehoObjektu=ANO. Kontrola ve 3D (včetně svislých hran), v Oblasti kompletní ZPS, po levelech.",
            ["Skipped:4.6"] = "4.6 Bezešvost ploch.\n\nPlošné objekty musí být v Oblasti kompletní ZPS celistvé a topologicky navázané. Mezera mezi plochami je chyba. Kontrola ve 2D, po levelech, bez plochy mostní konstrukce, provozní plochy tunelu a podchodu.",
            ["Skipped:4.8"] = "4.8 Kontrola existence svislých hran.\n\n3D obvod plochy musí být uzavřený a vytvořený z konstrukčních linií nebo linií s HraniceJinehoObjektu=1, včetně svislých linií. Chybějící svislé linie jsou chyba (při prvotním importu jen varování).",
            ["Skipped:4.9"] = "4.9 Kontrola kolize prvků ve 3D.\n\nKontrola pouze pro svislice, ve 3D po levelech a skupinách objektů. Chyby: částečný překryv prvků, jeden prvek obsahuje jiný, prvek je částí jiného.",
            ["Skipped:4.10"] = "4.10 Posun definičního bodu.\n\nDefiniční bod nelze přesunout mimo původní plochu v referenčním stavu ani do jiné plochy v referenčním stavu. Porušení je chyba.",
            ["Skipped:4.11"] = "4.11 Kontrola validního zápisu geometrie ploch ZPS.\n\nInterní kontrola IS DTM (podle OGC Simple Features): Exterior Ring proti směru hodinových ručiček, Interior Ringy po směru, uzavřené a jednoduché, bez dotyků mimo vrcholy (tolerance 1 mm). Není součástí kontrol při příjmu GAD."
        };

        public static string Get(string key) => Map.TryGetValue(key, out var d) ? d : null;
    }
}
