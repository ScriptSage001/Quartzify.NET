import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from './scheduler.service';

export interface TriggerKey {
  name: string;
  group: string;
}

export interface TriggerDetail {
  triggerName: string;
  triggerGroup: string;
  jobName: string;
  jobGroup: string;
  description?: string;
  triggerType: string;
  state: string;
  startTime: string;
  endTime?: string;
  nextFireTime?: string;
  previousFireTime?: string;
  priority?: number;
  misfireInstruction: number;
  jobDataMap: Record<string, any>;
  // For cron triggers
  cronExpression?: string;
  // For simple triggers
  repeatCount?: number;
  repeatInterval?: number;
  timesTriggered?: number;
}

@Injectable({
  providedIn: 'root'
})
export class TriggersService {
  private apiUrl = `${environment.apiUrl}/api/triggers`;

  constructor(private http: HttpClient) {}

  getAllTriggers(): Observable<TriggerDetail[]> {
    return this.http.get<TriggerDetail[]>(this.apiUrl);
  }

  getTriggersByGroup(group: string): Observable<TriggerDetail[]> {
    return this.http.get<TriggerDetail[]>(`${this.apiUrl}/group/${group}`);
  }

  getTriggersByJob(jobName: string, jobGroup: string): Observable<TriggerDetail[]> {
    return this.http.get<TriggerDetail[]>(`${this.apiUrl}/job/${jobGroup}/${jobName}`);
  }

  getTriggerDetail(triggerName: string, triggerGroup: string): Observable<TriggerDetail> {
    return this.http.get<TriggerDetail>(`${this.apiUrl}/${triggerGroup}/${triggerName}`);
  }

  createSimpleTrigger(trigger: TriggerDetail): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/simple`, trigger);
  }

  createCronTrigger(trigger: TriggerDetail): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/cron`, trigger);
  }

  updateTrigger(trigger: TriggerDetail): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.apiUrl}/${trigger.triggerGroup}/${trigger.triggerName}`, trigger);
  }

  deleteTrigger(triggerName: string, triggerGroup: string): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.apiUrl}/${triggerGroup}/${triggerName}`);
  }

  pauseTrigger(triggerName: string, triggerGroup: string): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/${triggerGroup}/${triggerName}/pause`, {});
  }

  resumeTrigger(triggerName: string, triggerGroup: string): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/${triggerGroup}/${triggerName}/resume`, {});
  }

  getTriggerGroups(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/groups`);
  }
}