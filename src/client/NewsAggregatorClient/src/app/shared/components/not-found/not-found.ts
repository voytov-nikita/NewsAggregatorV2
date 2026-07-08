import { Location } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  standalone: false,
  selector: 'app-not-found',
  templateUrl: './not-found.html',
  styleUrl: './not-found.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NotFound {
  private readonly router = inject(Router);
  private readonly location = inject(Location);

  protected readonly requestedPath = this.router.url;

  protected goHome() {
    this.router.navigate(['/allnews']);
  }

  protected goBack() {
    if (history.length > 1) this.location.back();
    else this.goHome();
  }
}
