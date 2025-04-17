// src/app/dashboard/widgets/status-widget/status-widget.component.ts
import { Component, Input, OnChanges } from '@angular/core';
import { SchedulerStatus } from '../../../core/services/scheduler.service';
import { SchedulerService } from '../../../core/services/scheduler.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-status-widget',
  standalone: false,
  templateUrl: './status-widget.component.html',
  styleUrls: ['./status-widget.component.scss']
})
export class StatusWidgetComponent implements OnChanges {
  @Input() schedulerStatus: SchedulerStatus | null = null;
  statusClass = 'status-unknown';
  statusText = 'Unknown';
  isProcessing = false;

  constructor(
    private schedulerService: SchedulerService,
    private toastr: ToastrService,
    private dialog: MatDialog
  ) {}

  ngOnChanges(): void {
    this.updateStatusDisplay();
  }

  updateStatusDisplay(): void {
    if (!this.schedulerStatus) {
      this.statusClass = 'status-unknown';
      this.statusText = 'Unknown';
      return;
    }

    if (this.schedulerStatus.isShutdown) {
      this.statusClass = 'status-shutdown';
      this.statusText = 'Shutdown';
    } else if (this.schedulerStatus.isStandbyMode) {
      this.statusClass = 'status-standby';
      this.statusText = 'Standby';
    } else if (this.schedulerStatus.isStarted) {
      this.statusClass = 'status-running';
      this.statusText = 'Running';
    } else {
      this.statusClass = 'status-unknown';
      this.statusText = 'Unknown';
    }
  }

  startScheduler(): void {
    this.isProcessing = true;
    this.schedulerService.startScheduler().subscribe({
      next: (response) => {
        if (response.success) {
          this.toastr.success('Scheduler started successfully', 'Success');
          // Update local status while waiting for refresh
          if (this.schedulerStatus) {
            this.schedulerStatus.isStarted = true;
            this.schedulerStatus.isStandbyMode = false;
            this.updateStatusDisplay();
          }
        } else {
          this.toastr.error(response.message || 'Failed to start scheduler', 'Error');
        }
      },
      error: (error) => {
        this.toastr.error('Failed to start scheduler', 'Error');
      },
      complete: () => {
        this.isProcessing = false;
      }
    });
  }

  standbyScheduler(): void {
    this.isProcessing = true;
    this.schedulerService.standbyScheduler().subscribe({
      next: (response) => {
        if (response.success) {
          this.toastr.success('Scheduler put in standby mode', 'Success');
          // Update local status while waiting for refresh
          if (this.schedulerStatus) {
            this.schedulerStatus.isStarted = false;
            this.schedulerStatus.isStandbyMode = true;
            this.updateStatusDisplay();
          }
        } else {
          this.toastr.error(response.message || 'Failed to put scheduler in standby', 'Error');
        }
      },
      error: (error) => {
        this.toastr.error('Failed to put scheduler in standby', 'Error');
      },
      complete: () => {
        this.isProcessing = false;
      }
    });
  }

  openShutdownDialog(): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '400px',
      data: {
        title: 'Shutdown Scheduler',
        message: 'Do you want to wait for running jobs to complete before shutting down?',
        confirmButtonText: 'Shutdown',
        cancelButtonText: 'Cancel',
        isDestructive: true
      }
    });

    dialogRef.afterClosed().subscribe(waitForJobs => {
      if (waitForJobs !== undefined) {
        this.shutdownScheduler(waitForJobs === true);
      }
    });
  }

  shutdownScheduler(waitForJobsToComplete: boolean): void {
    this.isProcessing = true;
    this.schedulerService.shutdownScheduler(waitForJobsToComplete).subscribe({
      next: (response) => {
        if (response.success) {
          this.toastr.success('Scheduler shutdown successfully', 'Success');
          // Update local status while waiting for refresh
          if (this.schedulerStatus) {
            this.schedulerStatus.isStarted = false;
            this.schedulerStatus.isStandbyMode = false;
            this.schedulerStatus.isShutdown = true;
            this.updateStatusDisplay();
          }
        } else {
          this.toastr.error(response.message || 'Failed to shutdown scheduler', 'Error');
        }
      },
      error: (error) => {
        this.toastr.error('Failed to shutdown scheduler', 'Error');
      },
      complete: () => {
        this.isProcessing = false;
      }
    });
  }
}