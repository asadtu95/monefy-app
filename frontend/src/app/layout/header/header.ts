import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { Auth } from '../../core/services/auth';

/** First URL segment → page title. Anything unlisted falls back to Expenses. */
const TITLES: Record<string, string> = {
  incomes: 'Income',
  categories: 'Categories',
  dashboard: 'Dashboard',
  budgets: 'Budgets',
  goals: 'Savings goals',
  reports: 'Reports',
  settings: 'Settings',
};

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {
  private readonly router = inject(Router);

  protected readonly auth = inject(Auth);
  protected readonly menuOpen = signal(false);

  protected get title(): string {
    const section = this.router.url.split('?')[0].split('/')[1] ?? '';
    return TITLES[section] ?? 'Expenses';
  }

  protected get initial(): string {
    const name = this.auth.displayName();
    return name ? name.charAt(0).toUpperCase() : '?';
  }

  protected toggleMenu(): void {
    this.menuOpen.update((open) => !open);
  }

  protected closeMenu(): void {
    this.menuOpen.set(false);
  }

  protected async logout(): Promise<void> {
    this.menuOpen.set(false);
    await this.auth.logout();
  }
}
