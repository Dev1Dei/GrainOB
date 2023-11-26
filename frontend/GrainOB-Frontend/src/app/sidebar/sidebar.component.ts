import { Component, OnInit } from '@angular/core';
import { UserBalanceService, UserBalanceResponse } from '../user-balance.service';
import { BalanceUpdateService } from '../balance-update.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  balance: number | null = null;

  constructor(
    private userBalanceService: UserBalanceService,
    private balanceUpdateService: BalanceUpdateService) {}

  ngOnInit(): void {
    this.fetchBalance();

    this.balanceUpdateService.balanceUpdated.subscribe((newBalance: number) =>{
      this.balance = newBalance;
    })
  }

  fetchBalance(): void{
    this.userBalanceService.getBalance().subscribe((response: UserBalanceResponse) => {
      console.log('Balance:', response.balance);
      this.balance = response.balance;
    }, error => {
      console.error('Failed to load balance:', error);
    });
  }
}