import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Camera } from '../models/camera';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CameraService {
    private baseUrl = "https://localhost:7028/api";
    constructor(private http: HttpClient) { }

    getAll(): Observable<Camera[]> {
        return this.http.get<Camera[]>(`${this.baseUrl}/cameras`);
    }

    search(name: string): Observable<Camera[]> {
        return this.http.get<Camera[]>(`${this.baseUrl}/cameras/search?name=${encodeURIComponent(name)}`);
    }
}
