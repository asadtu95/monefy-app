import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { Header } from '../header/header';
import { Sidebar } from '../sidebar/sidebar';
import { ProfileApi } from '../../core/services/profile';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, Header, Sidebar],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
})
export class MainLayout {
  private readonly profileApi = inject(ProfileApi);

  /**
   * Pages wait for the profile: it carries the month start day and currency,
   * and a page that opened before it would pick the wrong month. If the API is
   * unreachable the pages still open (on calendar months) and show their own errors.
   */
  protected readonly ready = signal(false);

  constructor() {
    // Also provisions the profile row on the API side.
    this.profileApi.load().subscribe({
      next: () => this.ready.set(true),
      error: () => this.ready.set(true),
    });
  }
}
