import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { Auth } from '../../core/services/auth';
import { ProfileApi } from '../../core/services/profile';

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit {
  private readonly router = inject(Router);
  private readonly profileApi = inject(ProfileApi);

  protected readonly auth = inject(Auth);
  protected readonly menuOpen = signal(false);

  /** Also provisions the profile row on the API side. */
  ngOnInit(): void {
    this.profileApi.load().subscribe({ error: () => undefined });
  }

  protected get title(): string {
    const url = this.router.url.split('?')[0];

    if (url.startsWith('/incomes')) return 'Income';
    if (url.startsWith('/categories')) return 'Categories';
    if (url.startsWith('/dashboard')) return 'Dashboard';

    return 'Expenses';
  }

  protected get initial(): string {
    const name = this.auth.displayName();
    return name ? name.charAt(0).toUpperCase() : '?';
  }

  protected toggleMenu(): void {
    this.menuOpen.update((open) => !open);
  }

  protected async logout(): Promise<void> {
    this.menuOpen.set(false);
    await this.auth.logout();
  }
}
