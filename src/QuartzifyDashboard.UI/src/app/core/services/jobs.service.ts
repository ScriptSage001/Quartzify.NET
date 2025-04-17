import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from './scheduler.service';

export interface JobDetail {
  jobName: string;
  jobGroup: string;
  description?: string;
  jobClass: string;
  isDurable: boolean;
  requestsRecovery: boolean;
  jobDataMap: Record<string, any>;
}

export interface JobKey {
  name: string;
  group: string;
}

export interface JobExecutionRequest {
  jobName: string;
  jobGroup: string;
  jobDataMap?: Record<string, any>;
}

@Injectable({
  providedIn: 'root'
})
export class JobsService {
  private apiUrl = `${environment.apiUrl}/api/jobs`;

  constructor(private http: HttpClient) {}

  getAllJobs(): Observable<JobDetail[]> {
    return this.http.get<JobDetail[]>(this.apiUrl);
  }

  getJobsByGroup(group: string): Observable<JobDetail[]> {
    return this.http.get<JobDetail[]>(`${this.apiUrl}/group/${group}`);
  }

  getJobDetail(jobName: string, jobGroup: string): Observable<JobDetail> {
    return this.http.get<JobDetail>(`${this.apiUrl}/${jobGroup}/${jobName}`);
  }

  createJob(job: JobDetail): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.apiUrl, job);
  }

  updateJob(job: JobDetail): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.apiUrl}/${job.jobGroup}/${job.jobName}`, job);
  }

  deleteJob(jobName: string, jobGroup: string): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.apiUrl}/${jobGroup}/${jobName}`);
  }

  pauseJob(jobName: string, jobGroup: string): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/${jobGroup}/${jobName}/pause`, {});
  }

  resumeJob(jobName: string, jobGroup: string): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/${jobGroup}/${jobName}/resume`, {});
  }

  executeJob(request: JobExecutionRequest): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/execute`, request);
  }

  getJobGroups(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/groups`);
  }
}