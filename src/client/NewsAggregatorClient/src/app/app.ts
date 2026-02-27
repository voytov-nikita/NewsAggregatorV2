import { Component, signal } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('NewsAggregatorClient');
}
