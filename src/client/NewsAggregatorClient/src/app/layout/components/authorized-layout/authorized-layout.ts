import { Component, signal } from '@angular/core';
import { NavigationService } from '@shared/services';

@Component({
  standalone: false,
  selector: 'app-authorized-layout',
  templateUrl: './authorized-layout.html',
  styleUrl: './authorized-layout.scss',
})
export class AuthorizedLayout {
  constructor(protected readonly navigation: NavigationService) {}

  protected readonly sidebarSections = signal([
    { label: 'Все новости', link: '/' },
    { label: 'Технологии', link: '/' },
    { label: 'Политика', link: '/' },
    { label: 'Бизнес', link: '/' },
    { label: 'Наука', link: '/' },
    { label: 'Культура', link: '/' },
  ]);

  protected readonly sidebarPersonal = signal([
    { label: 'Избранное', link: '/' },
    { label: 'История чтения', link: '/' },
  ]);
}
