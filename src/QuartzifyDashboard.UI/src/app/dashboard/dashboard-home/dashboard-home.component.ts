import { Component, OnInit } from '@angular/core';
import { SchedulerService, SchedulerStatus } from '../../core/services/scheduler.service';
import { JobsService, JobDetail } from '../../core/services/jobs.service';
import { TriggersService, TriggerDetail } from '../../core/services/triggers.service';
import { catchError, forkJoin } from 'rxjs';
import { of } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dashboard-home',
  standalone: false,
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.scss']
})
export class DashboardHomeComponent implements OnInit {
  schedulerStatus: SchedulerStatus | null = null;
  jobs: JobDetail[] = [];
  triggers: TriggerDetail[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(
    private schedulerService: SchedulerService,
    private jobsService: JobsService,
    private triggersService: TriggersService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      status: this.schedulerService.getSchedulerStatus().pipe(
        catchError(error => {
          this.errorMessage = 'Failed to load scheduler status';
          return of(null);
        })
      ),
      jobs: this.jobsService.getAllJobs().pipe(
        catchError(error => {
          this.errorMessage = 'Failed to load jobs';
          return of([]);
        })
      ),
      triggers: this.triggersService.getAllTriggers().pipe(
        catchError(error => {
          this.errorMessage = 'Failed to load triggers';
          return of([]);
        })
      )
    }).subscribe({
      next: (results) => {
        this.schedulerStatus = results.status;
        this.jobs = results.jobs;
        this.triggers = results.triggers;
      },
      error: (error) => {
        this.toastr.error('Failed to load dashboard data', 'Error');
        this.errorMessage = 'Failed to load dashboard data';
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  refreshData(): void {
    this.loadDashboardData();
    this.toastr.info('Dashboard data refreshed', 'Refresh');
  }
}