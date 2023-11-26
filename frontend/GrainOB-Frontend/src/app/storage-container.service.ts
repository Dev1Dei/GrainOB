import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface StorageContainerDto {
  userId: number;
  grainType: string;
  grainClass: string;
  totalCapacity: number;
  name: string;
}

export interface StorageContainer {
  gainingBalance: number;
  gainedBalance: number;
  containerId: number;
  userId: number;
  grainType: string;
  grainClass: string;
  name: string;
  weight: number;
  dryness: number;
  cleanliness: number;
  totalCapacity: number;
  freeSpace: number;
  storedSpace: number;
}

@Injectable({ providedIn: 'root' })
export class StorageContainerService {
  private apiUrl = 'http://localhost:5003/api/storagecontainer';

  constructor(private http: HttpClient) {}

  createContainer(containerData: StorageContainerDto): Observable<StorageContainer> {
    return this.http.post<StorageContainer>(this.apiUrl, containerData);
  }
  getContainers(userId: number): Observable<StorageContainer[]> {
    return this.http.get<StorageContainer[]>(`${this.apiUrl}/user/${userId}`);
  }
  cleanContainer(containerId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${containerId}/clean`, {});
  }
  getContainer(containerId: number): Observable<StorageContainer> {
    return this.http.get<StorageContainer>(`${this.apiUrl}/${containerId}`);
  }
  dryContainer(containerId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${containerId}/dry`, {});
  }
  sellGrain(containerId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${containerId}/sell`, {});
  }
  clearContainer(containerId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${containerId}/clear`, {});
  }
}
