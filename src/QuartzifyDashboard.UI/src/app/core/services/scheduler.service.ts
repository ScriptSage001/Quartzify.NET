import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface SchedulerStatus {
  isStarted: boolean;
  isShutdown: boolean;
  isStandbyMode: boolean;
  schedulerName: string;
  schedulerInstanceId: string;
  threadPoolSize: number;
  threadPoolType: string;
  jobStoreType: string;
  version: string;
}

export interface ApiResponse {
  success: boolean;
  message?: string;
  data?: any;
}

@Injectable({
  providedIn: 'root'
})
export class SchedulerService {
  private apiUrl = `${environment.apiUrl}/api/scheduler`;

  constructor(private http: HttpClient) {}

  getSchedulerStatus(): Observable<SchedulerStatus> {
    return this.http.get<SchedulerStatus>(`${this.apiUrl}/status`);
  }

  startScheduler(): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/start`, {});
  }

  standbyScheduler(): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/standby`, {});
  }

  shutdownScheduler(waitForJobsToComplete: boolean): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/shutdown`, { waitForJobsToComplete });
  }

  getSchedulerMetadata(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/metadata`);
  }
}