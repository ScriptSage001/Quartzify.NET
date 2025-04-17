import { Component, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { JobDetail } from '../../../core/services/jobs.service';
import { TriggerDetail } from '../../../core/services/triggers.service';
import { Router } from '@angular/router';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-jobs-overview',
  standalone: false,
  templateUrl: './jobs-overview.component.html',
  styleUrls: ['./jobs-overview.component.scss']
})
export class JobsOverviewComponent implements OnChanges {
  @Input() jobs: JobDetail[] = [];
  @Input() triggers: TriggerDetail[] = [];
  
  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;
  
  // Chart configuration
  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: {
        display: true,
        position: 'right',
      }
    }
  };
  
  public pieChartType: ChartType = 'pie';
  public pieChartData: ChartData<'pie', number[], string | string[]> = {
    labels: [],
    datasets: [{ 
      data: [],
      backgroundColor: [
        '#4caf50',  // Green for normal
        '#ff9800',  // Orange for paused
        '#f44336',  // Red for error
        '#9e9e9e'   // Grey for other
      ]
    }]
  };

  totalJobs = 0;
  totalTriggers = 0;
  uniqueJobGroups = 0;

  triggerStates = {
    normal: 0,
    paused: 0,
    error: 0,
    other: 0
  };

  constructor(private router: Router) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['jobs'] || changes['triggers']) {
      this.updateStats();
      this.updateChartData();
    }
  }

  updateStats(): void {
    this.totalJobs = this.jobs.length;
    this.totalTriggers = this.triggers.length;
    
    // Count unique job groups
    const jobGroups = new Set<string>();
    this.jobs.forEach(job => {
      if (job.jobGroup) {
        jobGroups.add(job.jobGroup);
      }
    });
    this.uniqueJobGroups = jobGroups.size;
    
    // Count trigger states
    this.triggerStates = {
      normal: 0,
      paused: 0,
      error: 0,
      other: 0
    };
    
    this.triggers.forEach(trigger => {
      if (trigger.state === 'NORMAL') {
        this.triggerStates.normal++;
      } else if (trigger.state === 'PAUSED') {
        this.triggerStates.paused++;
      } else if (trigger.state === 'ERROR') {
        this.triggerStates.error++;
      } else {
        this.triggerStates.other++;
      }
    });
  }

  updateChartData(): void {
    this.pieChartData = {
      labels: ['Active', 'Paused', 'Error', 'Other'],
      datasets: [{
        data: [
          this.triggerStates.normal,
          this.triggerStates.paused,
          this.triggerStates.error,
          this.triggerStates.other
        ],
        backgroundColor: [
          '#4caf50',  // Green for normal/active
          '#ff9800',  // Orange for paused
          '#f44336',  // Red for error
          '#9e9e9e'   // Grey for other
        ]
      }]
    };
    
    // Update chart if it exists
    if (this.chart) {
      this.chart.update();
    }
  }

  navigateToJobs(): void {
    this.router.navigate(['/jobs']);
  }

  navigateToTriggers(): void {
    this.router.navigate(['/triggers']);
  }
}