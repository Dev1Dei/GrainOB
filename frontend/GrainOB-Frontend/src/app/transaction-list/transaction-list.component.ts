import { Component, OnInit } from '@angular/core';
import { TransactionService, Transaction } from '../transaction.service';
import { FarmerService, Farmer } from '../farmer.service';
import { TruckService, Truck } from '../truck.service';
@Component({
  selector: 'app-transaction-list',
  templateUrl: './transaction-list.component.html',
  styleUrls: ['./transaction-list.component.css']
})
export class TransactionListComponent implements OnInit {
  transactions: Transaction[] = [];
  farmers: Farmer[] = [];
  trucks: Truck[] = [];

  constructor(
    private transactionService: TransactionService,
    private farmerService: FarmerService,
    private truckService: TruckService
    
    ) {}

  ngOnInit(): void {
    this.transactionService.getTransactions().subscribe(data => {
      console.log('Transactions:', data)
      this.transactions = data;
    });

    this.farmerService.getFarmers().subscribe(farmers => {
      console.log('Farmers:', farmers)
      this.farmers = farmers;
    });

    this.truckService.getTrucks().subscribe(trucks => {
      console.log('Trucks:', trucks)
      this.trucks = trucks;
    });
  }
}
