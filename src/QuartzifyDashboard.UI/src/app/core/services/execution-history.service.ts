import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ExecutionRecord {
  id: string;
  firedTime: string;
  scheduledTime: string;
  jobRunTime: number;
  jobName: string;
  jobGroup: string;
  triggerName: string;
  triggerGroup: string;
  status: string;
  errorMessage?: string;
}

export interface ExecutionHistoryFilter {
  jobName?: string;
  jobGroup?: string;
  triggerName?: string;
  triggerGroup?: string;
  status?: string;
  startDate?: string;
  endDate?: string;
  page?: number;
  size?: number;
}

export interface PagedResponse<T> {
  content: T[];
  totalElements: number;
  totalPages: number;
  size: number;
  number: number;
}

@Injectable({
  providedIn: 'root'
})
export class ExecutionHistoryService {
  private apiUrl = `${environment.apiUrl}/api/history`;

  constructor(private http: HttpClient) {}

  getExecutionHistory(filter: ExecutionHistoryFilter): Observable<PagedResponse<ExecutionRecord>> {
    let params = new HttpParams();
    
    if (filter.jobName) params = params.set('jobName', filter.jobName);
    if (filter.jobGroup) params = params.set('jobGroup', filter.jobGroup);
    if (filter.triggerName) params = params.set('triggerName', filter.triggerName);
    if (filter.triggerGroup) params = params.set('triggerGroup', filter.triggerGroup);
    if (filter.status) params = params.set('status', filter.status);
    if (filter.startDate) params = params.set('startDate', filter.startDate);
    if (filter.endDate) params = params.set('endDate', filter.endDate);
    if (filter.page !== undefined) params = params.set('page', filter.page.toString());
    if (filter.size !== undefined) params = params.set('size', filter.size.toString());
    
    return this.http.get<PagedResponse<ExecutionRecord>>(this.apiUrl, { params });
  }

  getRecentExecutions(limit: number = 10): Observable<ExecutionRecord[]> {
    return this.http.get<ExecutionRecord[]>(`${this.apiUrl}/recent?limit=${limit}`);
  }

  getExecutionDetail(id: string): Observable<ExecutionRecord> {
    return this.http.get<ExecutionRecord>(`${this.apiUrl}/${id}`);
  }

  getJobExecutionStats(jobName: string, jobGroup: string, days: number = 7): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/stats/job/${jobGroup}/${jobName}?days=${days}`);
  }

  getOverallExecutionStats(days: number = 7): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/stats?days=${days}`);
  }
}