import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Dsp } from '../models/models';

@Injectable({ providedIn: 'root' })
export class DspService {
  private readonly baseUrl = `${environment.apiBaseUrl}/dsps`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Dsp[]> {
    return this.http.get<Dsp[]>(this.baseUrl);
  }
}
