import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiResponse, CreateTaskRequest, Task, TaskFilterParams, UpdateTaskRequest } from '../models/task.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/tasks`;

  getAll(filters: TaskFilterParams = {}): Observable<Task[]> {
    let params = new HttpParams();
    if (filters.search)                params = params.set('search',      filters.search);
    if (filters.isCompleted != null)   params = params.set('isCompleted', String(filters.isCompleted));
    if (filters.priority   != null)    params = params.set('priority',    String(filters.priority));
    if (filters.sortBy)                params = params.set('sortBy',      filters.sortBy);
    if (filters.sortDir)               params = params.set('sortDir',     filters.sortDir);

    return this.http.get<ApiResponse<Task[]>>(this.baseUrl, { params }).pipe(map(r => r.data));
  }

  create(dto: CreateTaskRequest): Observable<Task> {
    return this.http.post<ApiResponse<Task>>(this.baseUrl, dto).pipe(map(r => r.data));
  }

  update(id: number, dto: UpdateTaskRequest): Observable<Task> {
    return this.http.put<ApiResponse<Task>>(`${this.baseUrl}/${id}`, dto).pipe(map(r => r.data));
  }

  toggleComplete(id: number): Observable<Task> {
    return this.http.patch<ApiResponse<Task>>(`${this.baseUrl}/${id}/complete`, {}).pipe(map(r => r.data));
  }

  delete(id: number): Observable<void> {
    return this.http.delete<ApiResponse<unknown>>(`${this.baseUrl}/${id}`).pipe(map(() => void 0));
  }
}
