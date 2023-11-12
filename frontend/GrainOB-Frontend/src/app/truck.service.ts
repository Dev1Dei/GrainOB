import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Farmer } from './farmer.service';
import { Transaction } from './transaction.service';
export interface Truck{
  truckId: number;
  farmerId: number;
  truckNumbers: string;
  truckStorage: number;
}
@Injectable({
  providedIn: 'root'
})
export class TruckService {
  private apiUrl = 'http://localhost:5003/api/trucks';
  constructor(private http: HttpClient) { }

  getTrucks(): Observable<Truck[]> {
    return this.http.get<Truck[]>(this.apiUrl);
  }

  getTruckById(id: number): Observable<Truck> {
    return this.http.get<Truck>('${this.apiUrl}/${id}');
  }
}
