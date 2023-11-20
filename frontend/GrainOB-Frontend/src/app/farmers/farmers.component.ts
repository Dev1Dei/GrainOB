import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ExtendedTransaction, TransactionService, PaginatedResponse } from '../transaction.service';
@Component({
  selector: 'app-farmers',
  templateUrl: './farmers.component.html',
  styleUrls: ['./farmers.component.css']
})
export class FarmersComponent implements OnInit{
transactions: ExtendedTransaction[] = [];
totalRecords: number = 0;
pageSize: number = 15;
currentPage: number = 1;
sort: string = 'asc';

constructor(
  private transactionService: TransactionService,
  private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadFarmersPage();
  }

  loadFarmersPage(): void {
    this.transactionService.getAoDTransactionsPage(this.currentPage, this.pageSize, this.sort)
    .subscribe((response: PaginatedResponse<ExtendedTransaction>) => {
      this.transactions = response.transactions;
      this.totalRecords = response.totalRecords;
      this.changeDetectorRef.detectChanges();
    });
  }
  toggleSortOrder(): void {
    this.sort = this.sort === 'asc' ? 'desc' : 'asc';
    this.loadFarmersPage();
  }
  nextPage(): void {
    if (this.currentPage < this.totalRecords / this.pageSize) {
      this.currentPage++;
      this.loadFarmersPage();
    }
  }
  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadFarmersPage();
    }
  }
  getMaxPage(): number {
    return Math.ceil(this.totalRecords / this.pageSize);
  }
}
