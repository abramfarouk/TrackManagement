import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Artist, CreateArtist } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ArtistService {
  private readonly baseUrl = `${environment.apiBaseUrl}/artists`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Artist[]> {
    return this.http.get<Artist[]>(this.baseUrl);
  }

  create(artist: CreateArtist): Observable<Artist> {
    return this.http.post<Artist>(this.baseUrl, artist);
  }
}
