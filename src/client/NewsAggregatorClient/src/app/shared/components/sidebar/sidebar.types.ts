export interface NavChild {
  id: string;
  label: string;
  route: string;
}

/**
 * `icon` is rendered verbatim as a unicode glyph (★, ⊞, ⊕). For SVG icons
 * that need their own component, set `iconKind` instead — currently only
 * `'flame'` is wired up (see `FlameIcon`). Don't pile new glyphs in as strings
 * just to dodge wiring a real icon component.
 */
export type NavIconKind = 'flame';

export interface NavItem {
  id: string;
  label: string;
  icon: string;
  iconKind?: NavIconKind;
  route?: string;
  comingSoon?: boolean;
  children?: NavChild[];
}
