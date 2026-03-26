import { Component, signal } from '@angular/core';
import { NavigationService } from '@shared/services';

interface Category {
  icon: string;
  name: string;
  selected: boolean;
}

interface NotificationSetting {
  title: string;
  description: string;
  active: boolean;
}

interface StatSource {
  name: string;
  articles: string;
  percentage: string;
}

@Component({
  standalone: false,
  selector: 'app-settings',
  templateUrl: './settings.html',
  styleUrl: './settings.scss',
})
export class Settings {
  constructor(protected readonly navigation: NavigationService) {}

  protected readonly activeTab = signal<'profile' | 'integrations' | 'notifications' | 'stats'>('profile');

  protected readonly categories = signal<Category[]>([
    { icon: '\uD83D\uDCBB', name: 'Технологии', selected: true },
    { icon: '\uD83D\uDD2C', name: 'Наука', selected: true },
    { icon: '\uD83D\uDCBC', name: 'Бизнес', selected: false },
    { icon: '\uD83C\uDFDB\uFE0F', name: 'Политика', selected: false },
    { icon: '\uD83C\uDFA8', name: 'Культура', selected: true },
    { icon: '\u26BD', name: 'Спорт', selected: false },
  ]);

  protected readonly privacySettings = signal<NotificationSetting[]>([
    { title: 'Публичный профиль', description: 'Другие пользователи могут видеть ваш профиль', active: false },
    { title: 'Показывать активность', description: 'Отображать вашу историю чтения', active: true },
  ]);

  protected readonly webhookActive = signal(true);
  protected readonly dailyDigest = signal(true);
  protected readonly weeklyReview = signal(false);

  protected readonly notificationSettings = signal<NotificationSetting[]>([
    { title: 'Новости из любимых категорий', description: 'Уведомления о новостях в выбранных категориях', active: true },
    { title: 'Популярные новости', description: 'Уведомления о самых обсуждаемых статьях', active: true },
    { title: 'Комментарии к вашим статьям', description: 'Когда кто-то отвечает на ваш комментарий', active: true },
    { title: 'Еженедельная статистика', description: 'Сводка вашей активности за неделю', active: false },
  ]);

  protected readonly stats = signal({
    articlesRead: 247,
    saved: 18,
    comments: 52,
  });

  protected readonly favoriteSources = signal<StatSource[]>([
    { name: 'TechCrunch', articles: '72 прочитанные статьи', percentage: '29%' },
    { name: 'The Verge', articles: '54 прочитанные статьи', percentage: '22%' },
    { name: 'Wired', articles: '41 прочитанная статья', percentage: '17%' },
  ]);

  protected switchTab(tab: 'profile' | 'integrations' | 'notifications' | 'stats'): void {
    this.activeTab.set(tab);
  }

  protected toggleCategory(index: number): void {
    this.categories.update(cats => {
      const updated = [...cats];
      updated[index] = { ...updated[index], selected: !updated[index].selected };
      return updated;
    });
  }

  protected togglePrivacy(index: number): void {
    this.privacySettings.update(settings => {
      const updated = [...settings];
      updated[index] = { ...updated[index], active: !updated[index].active };
      return updated;
    });
  }

  protected toggleNotification(index: number): void {
    this.notificationSettings.update(settings => {
      const updated = [...settings];
      updated[index] = { ...updated[index], active: !updated[index].active };
      return updated;
    });
  }
}
