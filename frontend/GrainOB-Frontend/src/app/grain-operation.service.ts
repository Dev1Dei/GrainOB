import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, forkJoin } from 'rxjs';
import { switchMap } from 'rxjs/operators';

export interface StorageContainerDto {
  // Define properties as per your requirement
}

export interface StorageContainer {
  containerId: number;
  // Add other properties as per your StorageContainer model
  userId: number;
  grainType: string;
  grainClass: string;
  weight: number;
}

@Injectable({ providedIn: 'root' })
export class GrainOperationService {
  private apiUrl = 'http://localhost:5003/api';

  constructor(private http: HttpClient) {}

  getContainer(containerId: number): Observable<StorageContainer> {
    return this.http.get<StorageContainer>(`${this.apiUrl}/storagecontainer/${containerId}`);
  }

  getLatestPrice(grainType: string, grainClass: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/price/GetLatestPrice`, {
      params: new HttpParams().set('grainType', grainType).set('grainClass', grainClass)
    });
  }

  updateBalance(userId: number, newBalance: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/userbalance/${userId}`, { balance: newBalance });
  }

  clearContainer(containerId: number, updatedContainerData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/storagecontainer/${containerId}/average`, updatedContainerData);
  }

  sellGrain(containerId: number, userId: number) {
    return this.getContainer(containerId).pipe(
      switchMap(container => {
        if (!container) {
          throw new Error('Container not found');
        }
        return forkJoin([
          of(container),
          this.getLatestPrice(container.grainType, container.grainClass)
        ]);
      }),
      switchMap(([container, priceInfo]) => {
        const saleAmount = container.weight * priceInfo.price;
        const newBalance = /* Calculate new balance based on existing balance and saleAmount */;
        const updatedContainerData = { /* Updated container data, e.g., weight set to 0 */ };
        return forkJoin([
          this.updateBalance(userId, newBalance),
          this.clearContainer(containerId, updatedContainerData)
        ]);
      })
    );
  }

  // Add other methods as per your requirement
}
