import { Component, signal } from '@angular/core';
import { NavigationService } from '@shared/services';

interface Comment {
  author: string;
  time: string;
  text: string;
  likes: number;
  dislikes: number;
}

interface RelatedArticle {
  id: number;
  source: string;
  title: string;
}

@Component({
  standalone: false,
  selector: 'app-news-detail',
  templateUrl: './detail.html',
  styleUrl: './detail.scss',
})
export class NewsDetail {
  constructor(protected readonly navigation: NavigationService) {}

  protected readonly article = signal({
    source: 'TechCrunch',
    readTime: '5 мин',
    timestamp: '2 часа назад',
    title: 'Новый прорыв в области квантовых вычислений обещает революцию в криптографии',
    category: 'Технологии',
    sourceUrl: 'techcrunch.com/quantum-breakthrough-2026',
    rating: 127,
    paragraphs: [
      'Исследователи из Массачусетского технологического института объявили о значительном прорыве в области квантовых вычислений, который может коренным образом изменить подход к криптографии и защите данных в ближайшие годы.',
      'Новая технология основана на использовании топологических кубитов, которые демонстрируют беспрецедентную стабильность при комнатной температуре. Это открытие решает одну из главных проблем квантовых компьютеров.',
      'По словам ведущего исследователя проекта, профессора Джейн Смит: "Мы достигли того, что многие считали невозможным. Наши топологические кубиты сохраняют квантовое состояние при температуре до 20 градусов Цельсия."',
      'Потенциальное влияние этого открытия на криптографию невозможно переоценить. Современные методы шифрования, такие как RSA, основаны на сложности факторизации больших чисел — задаче, которая легко решается квантовыми компьютерами с достаточной мощностью.',
    ],
  });

  protected readonly tags = signal([
    '#квантовые-вычисления',
    '#криптография',
    '#технологии',
    '#наука',
    '#MIT',
  ]);

  protected readonly relatedArticles = signal<RelatedArticle[]>([
    { id: 6, source: 'MIT News', title: 'Квантовые компьютеры: от теории к практике' },
    { id: 7, source: 'Nature', title: 'Топологические кубиты: новая эра' },
    { id: 8, source: 'Wired', title: 'Постквантовая криптография' },
    { id: 9, source: 'IEEE', title: 'Будущее шифрования' },
  ]);

  protected readonly comments = signal<Comment[]>([
    {
      author: 'Алексей Иванов',
      time: '1 час назад',
      text: 'Невероятное достижение! Наконец-то квантовые компьютеры становятся ближе к реальности.',
      likes: 15,
      dislikes: 2,
    },
    {
      author: 'Мария Петрова',
      time: '45 минут назад',
      text: 'А как быть с существующей инфраструктурой? Переход на новые стандарты может занять годы.',
      likes: 8,
      dislikes: 0,
    },
  ]);

  protected readonly commentText = signal('');

  protected onCommentInput(event: Event): void {
    this.commentText.set((event.target as HTMLTextAreaElement).value);
  }
}
