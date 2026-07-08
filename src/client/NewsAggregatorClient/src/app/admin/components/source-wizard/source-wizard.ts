import { ChangeDetectionStrategy, Component, computed, output, signal } from '@angular/core';
import {
  FeedFieldMapping,
  FieldKey,
  HtmlSelectors,
  NEWS_CATEGORIES,
  NewsCategory,
  Source,
  SourceType,
} from '../../models/source.model';

type Mapping = Partial<Record<FieldKey, string>>;
type ValidationState = 'idle' | 'checking' | 'ok' | 'error';

@Component({
  standalone: false,
  selector: 'app-source-wizard',
  templateUrl: './source-wizard.html',
  styleUrl: './source-wizard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SourceWizard {
  readonly cancel = output<void>();
  readonly create = output<Source>();

  protected readonly step = signal<1 | 2 | 3 | 4>(1);
  protected readonly stepNumbers: readonly (1 | 2 | 3 | 4)[] = [1, 2, 3, 4];

  protected readonly url = signal('');
  protected readonly validation = signal<ValidationState>('idle');
  protected readonly detectedType = signal<SourceType | null>(null);
  protected readonly detectedName = signal<string>('');

  protected readonly id = signal<string>('');
  protected readonly publisherName = signal<string>('');
  protected readonly publisherLink = signal<string>('');
  protected readonly cronSchedule = signal<string>('0 */15 * * * *');
  protected readonly category = signal<NewsCategory>('Uncategorized');
  protected readonly encoding = signal<string>('');

  protected readonly mapping = signal<Mapping>({});

  protected readonly categories = NEWS_CATEGORIES;

  protected readonly canNext = computed(() => {
    switch (this.step()) {
      case 1: return this.validation() === 'ok' && !!this.detectedType();
      case 2: return !!this.id().trim() && this.isValidCron(this.cronSchedule());
      case 3: return Object.keys(this.mapping()).length > 0;
      default: return true;
    }
  });

  protected validate(): void {
    const u = this.url().trim();
    if (!u) {
      this.validation.set('error');
      return;
    }
    this.validation.set('checking');
    setTimeout(() => {
      try {
        const parsed = new URL(u);
        const isRss = /\/(feed|rss|atom)(\.xml)?\/?$/i.test(parsed.pathname) || /\.xml$/i.test(parsed.pathname);
        this.detectedType.set(isRss ? 'Feed' : 'Html');
        const host = parsed.hostname.replace(/^www\./, '');
        this.detectedName.set(host);
        if (!this.id()) {
          this.id.set(host.split('.')[0].toLowerCase());
        }
        if (!this.publisherName()) {
          this.publisherName.set(host);
        }
        if (!this.publisherLink()) {
          this.publisherLink.set(`${parsed.protocol}//${parsed.host}`);
        }
        this.validation.set('ok');
      } catch {
        this.validation.set('error');
      }
    }, 600);
  }

  protected onUrlInput(value: string): void {
    this.url.set(value);
    if (this.validation() !== 'idle') {
      this.validation.set('idle');
      this.detectedType.set(null);
    }
  }

  protected next(): void {
    if (!this.canNext()) return;
    if (this.step() < 4) this.step.update((s) => (s + 1) as 1 | 2 | 3 | 4);
  }

  protected prev(): void {
    if (this.step() > 1) this.step.update((s) => (s - 1) as 1 | 2 | 3 | 4);
  }

  protected goTo(step: 1 | 2 | 3 | 4): void {
    if (step <= this.step()) this.step.set(step);
  }

  protected confirm(): void {
    const type = this.detectedType();
    if (!type) return;
    const m = this.mapping();
    const feedMapping: FeedFieldMapping | null = type === 'Feed'
      ? {
          titlePath: m.title,
          descriptionPath: m.description,
          imagePath: m.image,
          dateTimePath: m.dateTime,
          publisherPath: m.publisher,
        }
      : null;
    const htmlSelectors: HtmlSelectors | null = type === 'Html'
      ? {
          titleSelector: m.title,
          descriptionSelector: m.description,
          imageSelector: m.image,
          dateTimeSelector: m.dateTime,
          publisherSelector: m.publisher,
        }
      : null;

    const source: Source = {
      id: this.id().trim(),
      name: this.detectedName() || this.url(),
      publisherName: this.publisherName().trim() || this.detectedName(),
      publisherLink: this.publisherLink().trim() || this.url(),
      url: this.url(),
      type,
      cronSchedule: this.cronSchedule().trim(),
      encoding: this.encoding().trim() || null,
      enabled: true,
      category: this.category(),
      feedMapping,
      htmlSelectors,
    };
    this.create.emit(source);
  }

  protected close(): void {
    this.cancel.emit();
  }

  private isValidCron(cron: string): boolean {
    const parts = cron.trim().split(/\s+/).filter(Boolean);
    return parts.length === 5 || parts.length === 6;
  }
}
