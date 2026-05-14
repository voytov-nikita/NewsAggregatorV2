import { ChangeDetectionStrategy, Component, computed, output, signal } from '@angular/core';
import { FREQUENCY_OPTIONS, RESTART_POLICIES } from '../../data/admin-mock';
import { FieldKey, RestartPolicy, Source, SourceType } from '../../models/source.model';

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
  readonly create = output<Omit<Source, 'id' | 'articles' | 'success' | 'avgDelay' | 'lastCrawl'>>();

  protected readonly step = signal<1 | 2 | 3 | 4>(1);
  protected readonly stepNumbers: readonly (1 | 2 | 3 | 4)[] = [1, 2, 3, 4];

  protected readonly url = signal('');
  protected readonly validation = signal<ValidationState>('idle');
  protected readonly detectedType = signal<SourceType | null>(null);
  protected readonly detectedName = signal<string>('');

  protected readonly frequency = signal<number>(15);
  protected readonly restartPolicy = signal<RestartPolicy>('OnFailure');
  protected readonly timeout = signal<number>(10);
  protected readonly retries = signal<number>(3);

  protected readonly mapping = signal<Mapping>({});

  protected readonly frequencyOptions = FREQUENCY_OPTIONS;
  protected readonly restartPolicies = RESTART_POLICIES;

  protected readonly canNext = computed(() => {
    switch (this.step()) {
      case 1: return this.validation() === 'ok' && !!this.detectedType();
      case 2: return this.frequency() > 0 && this.timeout() > 0 && this.retries() >= 0;
      case 3: return Object.keys(this.mapping()).length > 0;
      default: return true;
    }
  });

  protected readonly summaryFrequency = computed(() => {
    return this.frequencyOptions.find((o) => o.value === this.frequency())?.label ?? `${this.frequency()} min`;
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
        this.detectedType.set(isRss ? 'RSS' : 'HTML');
        this.detectedName.set(parsed.hostname.replace(/^www\./, ''));
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
    this.create.emit({
      name: this.detectedName() || this.url(),
      url: this.url(),
      type,
      active: true,
      frequency: this.frequency(),
      restartPolicy: this.restartPolicy(),
      timeout: this.timeout(),
      retries: this.retries(),
      fieldMapping: this.mapping(),
    });
  }

  protected close(): void {
    this.cancel.emit();
  }
}
