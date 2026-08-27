import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { MonthlyProgress } from '../../shared/models/progress.model';

@Injectable({ providedIn: 'root' })
export class ProgressService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/progress';

  getMonthly(year: number, month: number) {
    const params = new HttpParams().set('year', year).set('month', month);
    return this.http.get<MonthlyProgress>(this.baseUrl, { params });
  }
}
