import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, switchMap } from 'rxjs';

export interface UserBalanceResponse {
  userId: number;
  balance: number;
}

@Injectable({ providedIn: 'root' })
export class UserBalanceService {
  private apiUrl = 'http://localhost:5003/api/userbalance';

  constructor(private http: HttpClient) {}

  getBalance(): Observable<UserBalanceResponse> {
    return this.http.get<UserBalanceResponse>(this.apiUrl);
  }

  updateBalance(userId: number, newBalance: number): Observable<UserBalanceResponse> {
    return this.http.put<UserBalanceResponse>(`${this.apiUrl}/${userId}`, { balance: newBalance });
  }
}
