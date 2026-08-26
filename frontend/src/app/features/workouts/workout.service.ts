import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Workout, WorkoutRequest } from '../../shared/models/workout.model';

@Injectable({ providedIn: 'root' })
export class WorkoutService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/workouts';

  getAll() {
    return this.http.get<Workout[]>(this.baseUrl);
  }

  create(request: WorkoutRequest) {
    return this.http.post<Workout>(this.baseUrl, request);
  }

  update(id: number, request: WorkoutRequest) {
    return this.http.put<Workout>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
