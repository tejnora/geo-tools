# GeoTools – CAD

**CAD** je desktopová aplikace pro Windows určená geodetům. Slouží k prohlížení, editaci a přípravě dat pro ČÚZK. Zpracovává dva formáty:

- **DTM**: Digitální technická mapa ČR (výměnný formát JVF DTM)
- **VFK**: výměnný formát katastru nemovitostí (ISKN)

## Stažení

Poslední build: **[CAD.zip](https://github.com/tejnora/geo-tools/releases/download/latest/CAD.zip)**

Zip stačí rozbalit a spustit `CAD.exe`. Nic se neinstaluje a .NET není potřeba, je součástí balíčku. Aplikace běží na Windows 10/11 (x64).

## Digitální technická mapa (DTM)

- **Import a export JVF DTM 1.5.0.2** (XML podle XSD schémat ČÚZK, platných od listopadu 2026), včetně údajů o změnách a údajů o výdeji; starší soubory 1.4.3 lze stále načíst
- **Import bodů** ze seznamu souřadnic
- **Editace objektů DTM**: body, linie a plochy, jejich atributy a geometrie, vykreslení kartografickými symboly podle katalogu DTM ČR
- **Správa vrstev** DTM
- **Validace dat** před odevzdáním: kontrola atributů podle schématu, geometrie, topologie, souřadnic a odvozených objektů
- **Identické body**: mapování a generování protokolu
- **Kontrola příloh GAD** podle metodiky ČÚZK (popisové pole, měřický náčrt, technická zpráva, seznam souřadnic)
- **Elektronický podpis výstupního adresáře** certifikátem s časovým razítkem (`Overeni.txt`, `.p7s`, `.tsr`)

## Katastr nemovitostí (VFK)

- **Import a export VFK** souborů
- **Editace dotčených parcel**, přidání nové parcely
- **Seznam souřadnic**: import a export
- **Generování SIP**
- Výběr katastrálního území, správa vrstev a nastavení zobrazení elementů

## Obecné funkce CAD

- Kreslicí nástroje: čáry, lomené čáry, kružnice, texty, aktivní body
- Rastrové podklady: přidání, odebrání a správa
- Export do DXF
- Undo/redo, posun, zoom a přizpůsobení pohledu

## Sestavení ze zdrojů

```
dotnet publish CAD/CAD.csproj -c Release -r win-x64 --self-contained true -o publish/CAD
```

Potřebuješ .NET 10 SDK. Při každém pushi do větve `main` se aplikace automaticky sestaví přes GitHub Actions a zveřejní jako release `latest`.
