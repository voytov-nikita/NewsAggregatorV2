#!/usr/bin/env node
// Multiply px values in SCSS files by 1.2 (to bring 1.25× → 1.5× scale).
// Skips 1px/2px solid borders, animations, percentages, em, unitless values.
import { readFileSync, writeFileSync, readdirSync, statSync } from 'node:fs';
import { join } from 'node:path';

const FACTOR = 1.2;
const DRY = process.argv.includes('--dry');

function walk(dir, out = []) {
  for (const name of readdirSync(dir)) {
    const p = join(dir, name);
    const s = statSync(p);
    if (s.isDirectory()) walk(p, out);
    else if (p.endsWith('.scss')) out.push(p);
  }
  return out;
}

const SKIP = [
  'empty-state',
  'error-state',
  'error-banner',
  'not-found',
  'toast-host',
].map((s) => s.toLowerCase());

const files = [...walk('src/app'), ...walk('src/assets/scss')].filter(
  (f) => !SKIP.some((s) => f.toLowerCase().includes(`\\${s}\\`)),
);

let total = 0;
let touched = 0;
const BORDER_KEYWORDS = /\b(solid|dashed|dotted|inset|outset|groove|ridge|double)\b/i;

for (const file of files) {
  const src = readFileSync(file, 'utf8');
  let changes = 0;

  const out = src.replace(/(\d+(?:\.\d+)?)px\b/g, (match, num, offset, fullStr) => {
    const v = parseFloat(num);

    // Skip 1px / 2px when followed by a border keyword within the next ~20 chars
    const lookahead = fullStr.slice(offset, offset + 60);
    if ((v === 1 || v === 2) && BORDER_KEYWORDS.test(lookahead)) return match;

    // Skip very small (<=2) — likely border thicknesses or rounding-sensitive
    if (v <= 2) return match;

    const scaled = Math.round(v * FACTOR);
    if (scaled === v) return match;
    changes++;
    return `${scaled}px`;
  });

  if (changes > 0) {
    total += changes;
    touched++;
    if (!DRY) writeFileSync(file, out);
    console.log(`${changes.toString().padStart(4)}  ${file}`);
  }
}

console.log(`\n${touched} files, ${total} replacements${DRY ? ' (DRY RUN)' : ''}`);
