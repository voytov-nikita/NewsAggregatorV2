import { ChangeDetectionStrategy, Component } from '@angular/core';

/**
 * Centered card that hosts the sign-in and sign-up forms.
 *
 * Deliberately outside `<app-page-shell>`: the "every top-level screen is wrapped in PageShell" rule
 * covers screens inside the application frame, and these two render before that frame exists.
 */
@Component({
  standalone: false,
  selector: 'app-auth-shell',
  templateUrl: './auth-shell.html',
  styleUrl: './auth-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthShell {}
