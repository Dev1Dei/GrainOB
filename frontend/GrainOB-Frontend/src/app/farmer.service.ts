import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Farmer{
  farmerId: number;
  farmerFirstName: string;
  farmerLastName: string;
}

@Injectable({
  providedIn: 'root'
})
export class FarmerService {
  private apiUrl = 'http://localhost:5003/api/farmers';
  constructor(private http: HttpClient) { }

  getFarmers(): Observable<Farmer[]> {
    return this.http.get<Farmer[]>(this.apiUrl);
  }

  getFarmerById(id: number): Observable<Farmer> {
    return this.http.get<Farmer>('${this.apiUrl}/${id}');
  }
}
