import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

interface NavItem {
  label: string;
  path: string;
  icon: string;
}

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  // Expenses leads — it is the screen used every day.
  protected readonly items: NavItem[] = [
    { label: 'Expenses', path: '/expenses', icon: '↗' },
    { label: 'Income', path: '/incomes', icon: '↙' },
    { label: 'Dashboard', path: '/dashboard', icon: '◧' },
    { label: 'Categories', path: '/categories', icon: '☰' },
  ];
}
