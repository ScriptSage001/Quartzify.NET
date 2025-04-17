import { Component } from '@angular/core';

interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  standalone: false,
})
export class SidebarComponent {
  navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/' },
    { label: 'Jobs', icon: 'work', route: '/jobs' },
    { label: 'Triggers', icon: 'alarm', route: '/triggers' },
    { label: 'Scheduler', icon: 'schedule', route: '/scheduler' }
  ];
}