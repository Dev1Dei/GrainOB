import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ExtendedTransaction, TransactionService, PaginatedResponse } from '../transaction.service';

@Component({
  selector: 'app-offers',
  templateUrl: './offers.component.html',
  styleUrls: ['./offers.component.css']
})
export class OffersComponent implements OnInit {
  transactions: ExtendedTransaction[] = [];
  totalRecords: number = 0;
  pageSize: number = 15;
  currentPage: number = 1;

  constructor(
    private transactionService: TransactionService,
    private changeDetectorRef: ChangeDetectorRef
    ) {}

  ngOnInit(): void {
    this.loadPendingTransactionsPage();
  }

  loadPendingTransactionsPage(): void {
    this.transactionService.getPendingTransactionsPage(this.currentPage, this.pageSize)
      .subscribe((response: PaginatedResponse<ExtendedTransaction>) => {
        this.transactions = response.transactions;
        this.totalRecords = response.totalRecords;
        this.changeDetectorRef.detectChanges();
      });
  }

  nextPage(): void {
    if (this.currentPage < this.totalRecords / this.pageSize) {
      this.currentPage++;
      this.loadPendingTransactionsPage();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadPendingTransactionsPage();
    }
  }

  acceptTransaction(transactionId: number): void {
    this.transactionService.updateTransactionStatus(transactionId, 'Accepted').subscribe({
      next: (response: any) => {
        // Reload the current page to reflect the changes
        this.loadPendingTransactionsPage();
      },
      error: (error: any) => {
        console.error('Error accepting transaction', error);
      }
    });
  }

  denyTransaction(transactionId: number): void {
    this.transactionService.updateTransactionStatus(transactionId, 'Denied').subscribe({
      next: (response: any) => {
        // Reload the current page to reflect the changes
        this.loadPendingTransactionsPage();
      },
      error: (error: any) => {
        console.error('Error denying transaction', error);
      }
    });
  }
  getMaxPage(): number {
    return Math.ceil(this.totalRecords / this.pageSize);
  }
}
