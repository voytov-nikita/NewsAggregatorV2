import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';
import { AuthService } from '../../services/auth.service';

@Component({
  standalone: false,
  selector: 'app-register',
  templateUrl: './register.html',
  styleUrl: './register.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Register {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly isSubmitting = signal(false);
  protected readonly errors = signal<string[]>([]);

  protected readonly form = inject(FormBuilder).nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    userName: ['', [Validators.required, Validators.minLength(2)]],
    // Length and complexity are Identity's PasswordOptions on the server. Repeating the full rule
    // set here would guarantee the two drift apart; the minimum is checked so the obvious case does
    // not need a round trip, and the server's own messages are surfaced for the rest.
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  protected submit(): void {
    if (this.form.invalid || this.isSubmitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errors.set([]);

    this.auth
      .register(this.form.getRawValue())
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => void this.router.navigateByUrl('/allnews'),
        error: (error: HttpErrorResponse) => this.errors.set(this.messagesFor(error)),
      });
  }

  private messagesFor(error: HttpErrorResponse): string[] {
    if (error.status === 0) return ['Cannot reach the server. Check that the API is running.'];

    const detail = error.error?.detail as string | undefined;
    const errors = error.error?.errors as string[] | undefined;

    if (errors?.length) return errors;
    if (detail) return [detail];

    return ['Something went wrong. Please try again.'];
  }
}
