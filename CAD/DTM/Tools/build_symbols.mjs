// Convert the checked-in catalogue assets; no network access is required.
// Usage: node build_symbols.mjs [absolute path to sharp package]
import fs from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { createRequire } from 'node:module';
const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const require = createRequire(import.meta.url);
const sharpPath = process.argv[2] ?? path.join(process.env.USERPROFILE, '.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
const sharp = require(sharpPath);
const sources = JSON.parse(await fs.readFile(path.join(root, 'Configuration/DtmSymbolSources.json'), 'utf8'));
const output = path.join(root, 'Symbols');
await fs.mkdir(output, { recursive: true });
for (const [filename, relativePath] of Object.entries(sources)) {
    const destination = path.join(output, filename.replace(/\.svg$/i, '.png'));
    const source = path.join(root, relativePath);
    if (filename.endsWith('.png')) await fs.copyFile(source, destination);
    else await sharp(source, { density: 600 }).resize({ width: 192, height: 192, fit: 'inside' }).png().toFile(destination);
}
const neutral = '<svg xmlns="http://www.w3.org/2000/svg" width="64" height="64"><g fill="none" stroke="#6e6e6e" stroke-width="4"><circle cx="32" cy="32" r="20"/><path d="M32 4v56M4 32h56"/></g></svg>';
await sharp(Buffer.from(neutral)).png().toFile(path.join(output, 'dtm_neutral_point.png'));
console.log(`Prepared ${Object.keys(sources).length + 1} symbols.`);
