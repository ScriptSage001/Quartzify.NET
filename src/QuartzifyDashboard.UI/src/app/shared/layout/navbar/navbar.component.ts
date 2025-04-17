import { Component, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../../../auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  standalone: false
})
export class NavbarComponent {
  @Output() toggleSidebar = new EventEmitter<void>();
  
  constructor(
    public authService: AuthService,
    private router: Router
  ) {}

  public onToggleSidebar(): void {
    this.toggleSidebar.emit();
  }

  public onLogout(): void {
    this.authService.logout();
  }
}