import { Injectable, signal } from '@angular/core';

export type ToastType = 'info' | 'success' | 'warning' | 'error';

export interface ToastContent {
  title?: string;
  body: string;
}

export interface Toast {
  id: number;
  type: ToastType;
  text: ToastContent;
}

const MAX_VISIBLE = 3;

@Injectable({ providedIn: 'root' })
export class ToastService {
  private nextId = 0;
  private readonly _toasts = signal<Toast[]>([]);

  readonly toasts = this._toasts.asReadonly();

  info(content: string | ToastContent) {
    return this.push('info', content);
  }
  success(content: string | ToastContent) {
    return this.push('success', content);
  }
  warning(content: string | ToastContent) {
    return this.push('warning', content);
  }
  error(content: string | ToastContent) {
    return this.push('error', content);
  }

  dismiss(id: number) {
    this._toasts.update((curr) => curr.filter((t) => t.id !== id));
  }

  private push(type: ToastType, content: string | ToastContent): number {
    const text: ToastContent = typeof content === 'string' ? { body: content } : content;
    const toast: Toast = { id: ++this.nextId, type, text };
    this._toasts.update((curr) => {
      const next = [...curr, toast];
      return next.length > MAX_VISIBLE ? next.slice(next.length - MAX_VISIBLE) : next;
    });
    return toast.id;
  }
}
