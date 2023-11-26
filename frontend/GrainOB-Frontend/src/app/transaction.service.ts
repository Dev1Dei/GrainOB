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

export interface ExtendedTransaction extends Transaction{
  farmerFirstName: string;
  farmerLastName: string;
  truckNumbers: string;
  truckStorage: number;
}

export interface PaginatedResponse<T> {
  totalRecords: number;
  transactions: T[];
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
    const url = `${this.apiUrl}/${transactionId}/status`;
    return this.http.put(url, { status });
  }
 
  getTotalPendingTransactions(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/pending/count`);
  }
  getTotalAoDTransactions(): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/AoD/count`);
  }

  assignContainer(transactionId: number, containerId: number): Observable<any> {
    const url = `${this.apiUrl}/${transactionId}/assign-container`;
    const body = { containerId: containerId };
    return this.http.put(url, body);
  }

  getPendingTransactionsPage(page: number, pageSize: number): Observable<PaginatedResponse<ExtendedTransaction>> {
    return this.http.get<PaginatedResponse<ExtendedTransaction>>(`${this.apiUrl}/pending`, {
      params: { pageNumber: page.toString(), pageSize: pageSize.toString() }
    });
  }
  getAoDTransactionsPage(page: number, pageSize: number, sort = 'asc'): Observable<PaginatedResponse<ExtendedTransaction>> {
    return this.http.get<PaginatedResponse<ExtendedTransaction>>(`${this.apiUrl}/AcceptedOrDenied`, {
      params: { pageNumber: page.toString(), pageSize: pageSize.toString(), sort}
    });
  }
  completeTransaction(transactionId: number): Observable<any> {
    const url = `${this.apiUrl}/${transactionId}/complete`;
    return this.http.post(url, {});
  }
}
