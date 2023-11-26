import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PricesService {
  private apiUrl = 'http://localhost:5003/api/price';

  constructor(private http: HttpClient) {}

  getPricesByGrainTypeAndClass(grainType: string, grainClass?: string): Observable<any[]> {
    let params = new HttpParams()
      .set('grainType', grainType)
      .set('grainClass', grainClass ? grainClass : 'None'); // Send 'None' when grainClass is not provided
  
    return this.http.get<any[]>(`${this.apiUrl}/GetPrices`, { params });
  }
  getLatestPriceByGrainTypeAndClass(grainType: string, grainClass?: string): Observable<any> {
    let params = new HttpParams()
      .set('grainType', grainType);

    if (grainClass) {
      params = params.set('grainClass', grainClass);
    } else {
      params = params.set('grainClass', 'None');
    }

    return this.http.get<any>(`${this.apiUrl}/GetLatestPrice`, { params });
  }
}
