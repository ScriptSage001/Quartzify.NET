import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';

import { SharedModule } from '../shared/shared.module';
import { DashboardHomeComponent } from './dashboard-home/dashboard-home.component';
import { StatusWidgetComponent } from './widgets/status-widget/status-widget.component';
import { JobsOverviewComponent } from './widgets/jobs-overview/jobs-overview.component';
import { RecentHistoryComponent } from './widgets/recent-history/recent-history.component';

const routes: Routes = [
  { path: '', component: DashboardHomeComponent }
];

@NgModule({
  declarations: [
    DashboardHomeComponent,
    StatusWidgetComponent,
    JobsOverviewComponent,
    RecentHistoryComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild(routes),
    BaseChartDirective
  ]
})
export class DashboardModule { }