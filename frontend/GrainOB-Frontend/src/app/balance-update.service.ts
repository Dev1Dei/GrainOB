import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BalanceUpdateService {
  balanceUpdated: EventEmitter<number> = new EventEmitter();

  constructor() { }

  updateBalance(newBalance: number): void {
    this.balanceUpdated.emit(newBalance);
  }
}
