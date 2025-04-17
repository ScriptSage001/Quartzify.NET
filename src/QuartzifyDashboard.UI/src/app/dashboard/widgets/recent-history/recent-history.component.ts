import { Component, OnInit } from '@angular/core';
import { ExecutionHistoryService, ExecutionRecord } from '../../../core/services/execution-history.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-recent-history',
  standalone: false,
  templateUrl: './recent-history.component.html',
  styleUrls: ['./recent-history.component.scss']
})
export class RecentHistoryComponent implements OnInit {
  recentExecutions: ExecutionRecord[] = [];
  isLoading = true;
  errorMessage = '';
  displayedColumns: string[] = ['jobName', 'startTime', 'duration', 'status'];

  constructor(
    private executionHistoryService: ExecutionHistoryService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadRecentHistory();
  }

  loadRecentHistory(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.executionHistoryService.getRecentExecutions(5).subscribe({
      next: (executions) => {
        this.recentExecutions = executions;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load recent executions';
        this.toastr.error('Failed to load recent execution history', 'Error');
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status.toUpperCase()) {
      case 'SUCCESS':
        return 'status-success';
      case 'FAILURE':
        return 'status-failure';
      case 'VETOED':
        return 'status-vetoed';
      default:
        return 'status-unknown';
    }
  }

  formatDuration(milliseconds: number): string {
    const seconds = Math.floor(milliseconds / 1000);
    if (seconds < 60) {
      return `${seconds}s`;
    }
    
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}m ${remainingSeconds}s`;
  }

  navigateToHistory(): void {
    this.router.navigate(['/history']);
  }

  refresh(): void {
    this.loadRecentHistory();
  }
}