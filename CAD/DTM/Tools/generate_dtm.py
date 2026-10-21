"""Regenerate DTM models, schema metadata and configuration from the checked-in sources.

Run with Python 3 (standard library only). The XLSX catalogue is read, never modified.
The generated files are checked in; the application does not need Python or Excel.
After regenerating, run build_symbols.mjs and the CAD.DtmTests project.
Rendering uses catalogue scale 1:500, solid lines and unhatched area fills.
"""
from pathlib import Path
import collections
import json
import re
import xml.etree.ElementTree as ET
import zipfile
import unicodedata

ROOT = Path(__file__).resolve().parents[1]
XS = '{http://www.w3.org/2001/XMLSchema}'
SS = '{http://schemas.openxmlformats.org/spreadsheetml/2006/main}'
NS = {'xs': XS[1:-1]}


def save(path, content):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content, encoding='utf-8', newline='\n')


def json_text(value):
    return json.dumps(value, ensure_ascii=False, indent=2) + '\n'


def cs(value):
    return json.dumps(value, ensure_ascii=False)


def normalized(value):
    return re.sub('[^a-z0-9]', '', unicodedata.normalize('NFKD', value.lower()).encode('ascii', 'ignore').decode())


def main():
    attributes_root = ET.parse(ROOT / 'Doc/xsd/common/atributy.xsd').getroot()
    declarations = {e.attrib['name']: e for e in attributes_root.findall('xs:element', NS)}
    definitions = {}
    for name, node in declarations.items():
        restriction = node.find('xs:simpleType/xs:restriction', NS)
        kind = node.get('type', 'xs:string')
        if restriction is not None:
            kind = restriction.get('base')
        children = node.findall('xs:complexType//xs:element', NS)
        values = {}
        if restriction is not None:
            for enum in restriction.findall('xs:enumeration', NS):
                doc = enum.find('xs:annotation/xs:documentation', NS)
                values[enum.get('value')] = ''.join(doc.itertext()).strip() if doc is not None else enum.get('value')
        doc = node.find('xs:annotation/xs:documentation', NS)
        definitions[name] = {
            'Name': name, 'Type': 'complex' if children else kind.split(':')[-1],
            'Description': ''.join(doc.itertext()).strip() if doc is not None else name,
            'Values': values,
            'Children': [{'Name': e.get('ref', e.get('name', '')).split(':')[-1],
                          'Namespace': 'gml' if e.get('ref', '').startswith('gml:') else 'atr',
                          'Required': e.get('minOccurs', '1') != '0'} for e in children],
            'Facets': {e.tag[len(XS):]: e.get('value') for e in (restriction if restriction is not None else [])
                       if e.tag != XS + 'enumeration' and 'value' in e.attrib},
        }

    with zipfile.ZipFile(ROOT / 'Doc/Katalog-kartografickych-symbolu-DTM-CR-verze-1-4-2-1.xlsx') as book:
        strings = [''.join(e.itertext()) for e in ET.fromstring(book.read('xl/sharedStrings.xml'))]
        records = []
        for sheet_number in range(5, 19):
            sheet = ET.fromstring(book.read(f'xl/worksheets/sheet{sheet_number}.xml'))
            active = None
            code_value, object_name, geometry, attribute_label = None, '', '', ''
            for row in sheet.findall('s:sheetData/s:row', {'s': SS[1:-1]}):
                cells = {}
                for cell in row.findall(SS + 'c'):
                    value = cell.find(SS + 'v')
                    if value is not None:
                        cells[re.sub(r'\d', '', cell.get('r'))] = strings[int(value.text)] if cell.get('t') == 's' else value.text
                new_code = re.fullmatch(r'\d{10}', cells.get('F', ''))
                if new_code:
                    code_value, object_name, geometry, attribute_label = cells['F'], cells.get('B', ''), '', ''
                label = cells.get('E', '').strip()
                if label in ('bod', 'linie', 'plocha'):
                    geometry, attribute_label = label, ''
                elif cells.get('D') and normalized(cells['D']) != 'geometrie':
                    attribute_label = cells['D']
                if code_value and (new_code or (label and cells.get('J'))):
                    active = {'Code': code_value, 'Name': object_name, 'Geometry': geometry,
                              'AttributeLabel': attribute_label, 'ValueLabel': label if attribute_label else '',
                              'Sheet': sheet_number, 'Row': int(row.get('r')), 'Rows': []}
                    records.append(active)
                if active:
                    active['Rows'].append(cells)

    config_path = ROOT / 'Configuration/DtmConfiguration.json'
    # Existing graphics are retained if they are the best available point symbol.
    pngs = {p.name: p for p in (ROOT / 'Doc/Kartograficke symboly').rglob('*.png')}
    svgs = {p.name: p for p in (ROOT / 'Doc/Kartograficke symboly').rglob('*.svg')}
    original_sources = [p for p in (ROOT / 'Elements').rglob('*.cs') if 'Generated' not in p.parts]
    existing = set(re.findall(r'\bclass\s+Dtm(\w+)Element\b', '\n'.join(p.read_text(encoding='utf-8-sig') for p in original_sources)))
    schemas, config, coverage, models = {}, {}, [], []
    symbol_sources = {}
    index = ET.parse(ROOT / 'Doc/xsd/index/index_data.xsd').getroot()
    exchange_names = {e.get('type'): e.get('name') for e in index.findall('xs:element', NS) if e.get('type')}
    namespace_prefixes = dict((value, prefix) for event, (prefix, value) in ET.iterparse(ROOT / 'Doc/xsd/index/index_data.xsd', events=['start-ns']))
    for file in sorted((ROOT / 'Doc/xsd/objekty').glob('*.xsd')):
        schema = ET.parse(file).getroot()
        name = schema.find('xs:element', NS).get('name')
        def fixed(field):
            e = schema.find(f'.//xs:element[@name="{field}"]', NS)
            return e.get('fixed') if e is not None else None
        def code(field):
            e = schema.find(f'.//xs:attribute[@name="{field}"]', NS)
            return e.get('fixed') if e is not None else None
        geometries = [e.get('ref').split(':')[-1] for e in schema.findall('.//xs:element', NS) if e.get('ref', '').startswith('gml:')]
        suffix = code('code_suffix')
        element_type = {'01': 'Bod', '02': 'Linie', '03': 'Plocha', '04': 'DefinicniBod'}.get(suffix, 'Plocha')
        attrs = schema.find('.//xs:element[@name="AtributyObjektu"]/xs:complexType', NS)
        refs = [{'Name': e.get('ref').split(':')[-1], 'Namespace': 'atr', 'Required': e.get('minOccurs', '1') != '0'}
                for e in attrs.findall('.//xs:element', NS)]
        record_sequence = schema.find('.//xs:element[@name="ZaznamObjektu"]/xs:complexType/xs:sequence', NS)
        record_attributes = [{'Name': e.get('ref').split(':')[-1], 'Namespace': 'atr', 'Required': e.get('minOccurs', '1') != '0'}
                             for e in record_sequence.findall('xs:element', NS) if e.get('ref', '').startswith('atr:')]
        object_type = schema.find('xs:element', NS).get('type').split(':')[-1]
        exchange_name = exchange_names.get(namespace_prefixes[schema.get('targetNamespace')] + ':' + object_type, name)
        schemas[name] = {'Name': name, 'ExchangeName': exchange_name, 'File': file.name, 'ElementType': element_type,
                         'Geometries': geometries, 'Attributes': refs, 'RecordAttributes': record_attributes}
        option = {
            'CodeBase': code('code_base'), 'CodeSuffix': suffix, 'Xmlns': schema.get('targetNamespace'),
            'ObjektovyTypNazev': fixed('ObjektovyTypNazev'), 'KategorieObjektu': fixed('KategorieObjektu'),
            'SkupinaObjektu': fixed('SkupinaObjektu'), 'ObsahovaCast': fixed('ObsahovaCast'),
            'ElementType': element_type, 'ExchangeName': exchange_name,
        }
        candidates = [r for r in records if r['Code'] == option['CodeBase']]
        wanted_geometry = {'Bod': 'bod', 'DefinicniBod': 'bod', 'Linie': 'linie', 'Plocha': 'plocha'}[element_type]
        exact = [r for r in candidates if wanted_geometry == r['Geometry']]
        if not candidates:
            related = [r for r in records if normalized(r['Name']) == normalized(option['ObjektovyTypNazev'] or '') and r['Geometry'] == wanted_geometry]
            candidates = related
        record = (exact or candidates or [None])[0]
        color = [110, 110, 110, 255]
        width = 0.00018
        symbol, size = None, 0.8
        if record:
            for row in record['Rows']:
                value = row.get('J', '').strip()
                rgb = re.fullmatch(r'(\d{1,3})\s+(\d{1,3})\s+(\d{1,3})', value)
                if rgb and all(int(v) <= 255 for v in rgb.groups()):
                    color = [int(v) for v in rgb.groups()] + [255]
                    break
                if re.fullmatch(r'\d{9}', value) and all(int(value[i:i+3]) <= 255 for i in (0, 3, 6)):
                    color = [int(value[i:i+3]) for i in (0, 3, 6)] + [255]
                    break
            for row in record['Rows']:
                value = row.get('J', '').strip()
                if value in svgs:
                    symbol_sources[value] = str(svgs[value].relative_to(ROOT)).replace('\\', '/')
                    symbol = Path(value).with_suffix('.png').name
                    break
            numeric = [float(r['J'].replace(',', '.')) for r in record['Rows'] if re.fullmatch(r'\d+[.,]?\d*', r.get('J', ''))]
            if numeric:
                size = next((v for v in numeric if 0.3 <= v <= 12), 0.8)
                width = next((v / 1000 for v in numeric if 0.05 <= v <= 0.6), 0.00018)
        if element_type in ('Bod', 'DefinicniBod'):
            if element_type == 'DefinicniBod':
                option['Graphic'] = [{'Color': color, 'Width': 0.5, 'Symbol': 'ZPS_db.png'}]
            elif symbol:
                option['Graphic'] = [{'Color': color, 'Width': size, 'Symbol': symbol}]
            else:
                # A definition point uses the conventional centroid marker.
                fallback = 'ZPS_db.png' if element_type == 'DefinicniBod' else 'dtm_neutral_point.png'
                option['Graphic'] = [{'Color': color, 'Width': 0.8, 'Symbol': fallback}]
            # Attribute-dependent point symbols use the XSD enumeration's actual numeric values.
            variants = [r for r in exact if r['ValueLabel']]
            selectors = []
            for ref in refs:
                attribute = definitions[ref['Name']]
                labels = {normalized(label): value for value, label in attribute['Values'].items()}
                matches = [(r, labels[normalized(r['ValueLabel'])]) for r in variants if normalized(r['ValueLabel']) in labels]
                if matches:
                    selectors.append((len(matches), attribute['Name'], matches))
            if selectors and element_type == 'Bod':
                _, selector, matches = max(selectors, key=lambda candidate: candidate[0])
                graphic_variants = {'_': option['Graphic']}
                for variant, enum_value in matches:
                    svg = next((r.get('J', '').strip() for r in variant['Rows'] if r.get('J', '').strip() in svgs), None)
                    if not svg:
                        continue
                    symbol_sources[svg] = str(svgs[svg].relative_to(ROOT)).replace('\\', '/')
                    dimensions = [float(r['J'].replace(',', '.')) for r in variant['Rows'] if re.fullmatch(r'\d+[.,]?\d*', r.get('J', ''))]
                    variant_size = next((v for v in dimensions if 0.3 <= v <= 12), size)
                    graphic_variants[enum_value] = [{'Color': color, 'Width': variant_size, 'Symbol': Path(svg).with_suffix('.png').name}]
                if len(graphic_variants) > 1:
                    option['GraphicAttribute'] = selector
                    option['Graphic'] = graphic_variants
            graphics = option['Graphic']
            for series in ([graphics] if isinstance(graphics, list) else graphics.values()):
                for graphic in series:
                    filename = graphic['Symbol']
                    if filename in pngs:
                        symbol_sources[filename] = str(pngs[filename].relative_to(ROOT)).replace('\\', '/')
        option['Color'], option['Width'] = color, width
        option['GraphicSource'] = f"catalogue:sheet{record['Sheet']}:row{record['Row']}" if record else 'neutral'
        config[name] = option
        coverage.append({'Object': name, 'Xsd': file.name, 'ExistingClass': name in existing,
                         'ExchangeName': exchange_name,
                         'GraphicSource': option['GraphicSource'],
                         'Match': 'exact' if exact else 'related' if candidates else 'neutral',
                         'Simplified': element_type in ('Plocha', 'Linie'),
                         'SymbolMatch': 'centroid' if element_type == 'DefinicniBod' else ('neutral' if element_type == 'Bod' and not symbol else 'catalogue' if record else 'neutral'),
                         'GraphicAttribute': option.get('GraphicAttribute')})
        if name not in existing:
            base = {'Bod': 'DtmBodBaseElement', 'DefinicniBod': 'DtmDefinicniBodBaseElement',
                    'Linie': 'DtmLinieElementBase', 'Plocha': 'DtmPlochaBaseElement'}[element_type]
            properties = []
            for ref in refs:
                attr = definitions[ref['Name']]
                if attr['Type'] == 'complex' or attr['Name'] == 'CisloBodu':
                    continue
                kind = attr['Type']
                typ = 'string'
                if attr['Values']:
                    typ = f"DtmSchema{attr['Name']}Enum?"
                elif kind in ('integer', 'int', 'nonNegativeInteger', 'positiveInteger', 'long', 'unsignedInt', 'short'):
                    typ = 'long?'
                elif kind in ('double', 'decimal', 'float'):
                    typ = 'decimal?'
                elif kind == 'boolean':
                    typ = 'bool?'
                elif kind in ('date', 'dateTime'):
                    typ = 'DateTime?'
                properties.append(f'        public {typ} {attr["Name"]} {{ get => SchemaAttributes.Get<{typ}>({cs(attr["Name"])}); set => SchemaAttributes.Set({cs(attr["Name"])}, value); }}')
            models.append(f'    [DtmGeneratedElement]\n    public sealed class Dtm{name}Element : {base}\n    {{\n' + '\n'.join(properties) + '\n    }')

    enums = []
    for attr in definitions.values():
        if attr['Values']:
            members = [f'        [System.ComponentModel.Description({cs(label)})]\n        Value{value.replace("-", "Minus")} = {value}' for value, label in attr['Values'].items()]
            enums.append(f'    public enum DtmSchema{attr["Name"]}Enum\n    {{\n' + ',\n'.join(members) + '\n    }')
    generated = ROOT / 'Elements/Generated/DtmSchemaElements.g.cs'
    save(generated, '// <auto-generated />\n// Regenerate with Tools/generate_dtm.py.\nusing System;\n\nnamespace CAD.DTM.Elements\n{\n' + '\n\n'.join(enums + models) + '\n}\n')
    save(ROOT / 'Configuration/DtmSchema.json', json_text({'Attributes': definitions, 'Objects': schemas}))
    # Old spellings are input aliases only; generated XML always uses XSD names.
    aliases = {'ParkovisteOdstavnaPlocha': 'ParkovisteOdstavnaPlochaPlocha', 'OPObjektuElektrickeSite': 'OPElektrickeSite'}
    aliases.update({obj['ExchangeName']: name for name, obj in schemas.items() if obj['ExchangeName'] != name})
    # Verify aliases against the schema inventory instead of guessing a target.
    aliases_path = ROOT / 'Configuration/DtmAliases.json'
    for alias, target in list(aliases.items()):
        if target not in schemas:
            raise ValueError(f'Cannot resolve legacy alias {alias}: {target}')
    save(aliases_path, json_text(aliases))
    save(config_path, json_text(config))
    save(ROOT / 'Configuration/DtmSymbolSources.json', json_text(symbol_sources))
    save(ROOT / 'Doc/DtmCoverage.json', json_text(coverage))
    save(ROOT / 'Doc/DtmCoverageSummary.json', json_text({
        'ObjectCount': len(schemas), 'GeneratedClassCount': len(models), 'XsdVersion': '1.4.3', 'CatalogueVersion': '1.4.2.1',
        'ContentParts': dict(collections.Counter(o['ObsahovaCast'] or 'other' for o in config.values())),
        'CatalogueMatches': dict(collections.Counter(o['Match'] for o in coverage)),
        'SymbolMatches': dict(collections.Counter(o['SymbolMatch'] for o in coverage)),
        'Rendering': 'Basic appearance at 1:500; solid lines and simple area fills. See DtmCoverage.json for object-level sources and fallbacks.'
    }))
    print(f'{len(schemas)} objects, {len(definitions)} attribute definitions, {len(models)} generated classes, {len(symbol_sources)} symbol sources')


if __name__ == '__main__':
    main()
