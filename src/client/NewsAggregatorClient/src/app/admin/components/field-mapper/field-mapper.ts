import { ChangeDetectionStrategy, Component, computed, effect, input, model, signal } from '@angular/core';
import {
  AUTO_MAPPINGS,
  FIELD_TARGETS,
  HTML_SAMPLE,
  RSS_SAMPLE,
} from '../../data/admin-mock';
import { FieldKey, SamplePath, SourceType } from '../../models/source.model';

type Mapping = Partial<Record<FieldKey, string>>;

@Component({
  standalone: false,
  selector: 'app-field-mapper',
  templateUrl: './field-mapper.html',
  styleUrl: './field-mapper.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FieldMapper {
  readonly type = input.required<SourceType>();
  readonly mapping = model<Mapping>({});

  protected readonly fieldTargets = FIELD_TARGETS;
  protected readonly mode = signal<'default' | 'custom'>('default');
  protected readonly active = signal<FieldKey | null>(null);

  protected readonly sample = computed<SamplePath[]>(() =>
    this.type() === 'Html' ? HTML_SAMPLE : RSS_SAMPLE,
  );

  protected readonly autoMap = computed(() => AUTO_MAPPINGS[this.type()]);

  protected readonly mappedCount = computed(
    () => this.fieldTargets.filter((f) => this.mapping()[f.key]).length,
  );

  protected readonly activeLabel = computed(() => {
    const a = this.active();
    return a ? this.fieldTargets.find((f) => f.key === a)?.label ?? null : null;
  });

  constructor() {
    effect(() => {
      // Seed mapping from auto map when empty.
      if (Object.keys(this.mapping()).length === 0) {
        this.mapping.set({ ...this.autoMap() });
      }
    });
  }

  protected pathLabel(path: string): string {
    return this.sample().find((n) => n.path === path)?.label ?? path.split('/').pop()!;
  }

  protected assignedLabelFor(path: string): string | null {
    const m = this.mapping();
    const key = (Object.keys(m) as FieldKey[]).find((k) => m[k] === path);
    return key ? this.fieldTargets.find((f) => f.key === key)?.label ?? null : null;
  }

  protected setActive(key: FieldKey): void {
    this.active.set(this.active() === key ? null : key);
  }

  protected assign(path: string): void {
    const a = this.active();
    if (!a) return;
    const next: Mapping = { ...this.mapping(), [a]: path };
    this.mapping.set(next);
    const unmapped = this.fieldTargets.find((f) => f.key !== a && !next[f.key]);
    this.active.set(unmapped?.key ?? null);
  }

  protected clear(key: FieldKey): void {
    const next: Mapping = { ...this.mapping() };
    delete next[key];
    this.mapping.set(next);
  }

  protected resetToAuto(): void {
    this.mapping.set({ ...this.autoMap() });
    this.mode.set('default');
    this.active.set(null);
  }

  protected toCustom(): void {
    this.mode.set('custom');
  }
}
