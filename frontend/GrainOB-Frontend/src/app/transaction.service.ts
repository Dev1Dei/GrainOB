import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Transaction {
  transactionId: number;
  truckId: number;
  grainType: string;
  grainClass: string | null;
  dryness: number;
  cleanliness: number;
  grainWeight: number;
  arrivalTime: string;
  wantedPay: number;
  pricePerTonne: number;
  status: string;
}

@Injectable({
  providedIn: 'root'
})


export class TransactionService {
  private apiUrl = 'http://localhost:5003/api/transactions';

  constructor(private http: HttpClient) {}

  getTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.apiUrl);
  }
  updateTransactionStatus(transactionId: number, status: string): Observable<any> {
    console.log(`Updating transaction status for ID: ${transactionId} to ${status}`);
    const url = `${this.apiUrl}/${transactionId}/status`;
    return this.http.put(url, { status });
  }
}
