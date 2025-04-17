import { Component, ViewChild } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { map, Observable, shareReplay } from 'rxjs';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatSidenav, MatSidenavContainer, MatSidenavContent } from '@angular/material/sidenav';

import { SharedModule } from './shared/shared.module';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, MatSidenav, MatSidenavContainer, MatSidenavContent, SharedModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Quartzify Dashboard';

  @ViewChild('drawer') drawer!: MatSidenav;
  
  isHandset$: Observable<boolean>;

  constructor(private breakpointObserver: BreakpointObserver) {
    this.isHandset$ = this.breakpointObserver.observe(Breakpoints.Handset)
      .pipe(
        map(result => result.matches),
        shareReplay()
      );
  }

  toggleSidebar(): void {
    this.drawer.toggle();
  }
}
