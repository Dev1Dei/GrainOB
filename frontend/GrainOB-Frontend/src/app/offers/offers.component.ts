import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ExtendedTransaction, TransactionService, PaginatedResponse } from '../transaction.service';
import { StorageContainer, StorageContainerService } from '../storage-container.service';
import { UserBalanceResponse, UserBalanceService } from '../user-balance.service';
import { BalanceUpdateService } from '../balance-update.service';
import { NotificationService } from '../notification.service';
@Component({
  selector: 'app-offers',
  templateUrl: './offers.component.html',
  styleUrls: ['./offers.component.css']
})
export class OffersComponent implements OnInit {
  transactions: ExtendedTransaction[] = [];
  totalRecords: number = 0;
  pageSize: number = 10;
  currentPage: number = 1;
  containers: StorageContainer[] = [];
  selectedContainerId!: number;
  showContainerDropdown: boolean = false;
  acceptingTransactionId?: number;
  filteredContainers: StorageContainer[] = [];

  constructor(
    private transactionService: TransactionService,
    private changeDetectorRef: ChangeDetectorRef,
    private storageContainerService: StorageContainerService,
    private userBalanceService: UserBalanceService,
    private balanceUpdateService: BalanceUpdateService,
    private notificationService: NotificationService,
    ) {}

  ngOnInit(): void {
    this.loadPendingTransactionsPage();
    this.loadContainers();
  }
  onAccept(transactionId: number): void {
    this.showContainerDropdown = true; // Show the dropdown
    this.acceptingTransactionId = transactionId;
    this.selectedContainerId = undefined!; // Reset selected container
    const transaction = this.transactions.find(t => t.transactionId === transactionId);
    if (transaction) {
      // Filter the containers based on the transaction's grain type and class
      this.filteredContainers = this.containers.filter(container =>
        container.grainType === transaction.grainType &&
        (container.grainClass === transaction.grainClass || container.grainClass === 'None' || transaction.grainClass === 'None') ||
        (container.grainType === 'None' && container.storedSpace === 0)
      );
    }
  }
  loadPendingTransactionsPage(): void {
    this.transactionService.getPendingTransactionsPage(this.currentPage, this.pageSize)
      .subscribe((response: PaginatedResponse<ExtendedTransaction>) => {
        this.transactions = response.transactions;
        this.totalRecords = response.totalRecords;
        this.changeDetectorRef.detectChanges();
      });
  }
  loadContainers(): void {
    this.storageContainerService.getContainers(1).subscribe({
      next: (data: StorageContainer[]) => {
        this.containers = data;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        console.error('Error loading containers', error);
      }
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

  acceptTransaction(transactionId: number, containerId: number): void {
    // First, get the current balance
    this.userBalanceService.getBalance().subscribe({
      next: (balanceResponse: UserBalanceResponse) => {
        // Find the transaction to get the wantedPay
        const transaction = this.transactions.find(t => t.transactionId === transactionId);
        if (!transaction) {
          console.error('Transaction not found.');
          return;
        }
        if (balanceResponse.balance >= transaction.wantedPay) {
          // If there are enough funds, proceed with accepting the transaction
          this.transactionService.updateTransactionStatus(transactionId, 'Accepted').subscribe(() => {
            // After successful status update, assign the container
            this.transactionService.assignContainer(transactionId, containerId).subscribe(() => {
              // If successful, deduct the wantedPay from the balance
              const newBalance = balanceResponse.balance - transaction.wantedPay;
              // Update the user balance
              this.userBalanceService.updateBalance(balanceResponse.userId, newBalance).subscribe(() => {
                // After updating the balance, complete other transaction steps
                this.completeTransaction(transactionId, containerId);
                // Refresh the list to show the updated container data
                this.loadPendingTransactionsPage();
                this.balanceUpdateService.updateBalance(newBalance);
                this.showContainerDropdown = false; // Hide the dropdown
              });
            }, error => {
              console.error('Error assigning container', error);
            });
          }, error => {
            console.error('Error accepting transaction', error);
          });
        } else {
          console.error('Not enough funds to accept the transaction.');
          // Here, you would typically show an error message to the user
        }
      },
      error: (error) => {
        console.error('Error fetching balance', error);
      }
    });

    this.acceptingTransactionId = undefined; // Reset the accepting transaction ID
  }

  onContainerSelected(transactionId: number): void {
    if(this.selectedContainerId) {
      this.acceptTransaction(transactionId, this.selectedContainerId);
    }
  }
  completeTransaction(transactionId: number, containerId: number): void {
    this.transactionService.completeTransaction(transactionId).subscribe({
        next: () => {
            // After completing the transaction, fetch the updated container data
            this.storageContainerService.getContainer(containerId).subscribe(updatedContainer => {
                // Update the specific container in your local state
                const index = this.containers.findIndex(c => c.containerId === containerId);
                if (index !== -1) {
                    this.containers[index] = updatedContainer;
                }
                // Emit the transaction completed notification
                const transaction = this.transactions.find(t => t.transactionId === transactionId);
                if (transaction) {
                    this.emitTransactionCompletedNotification(transaction);
                }
                this.changeDetectorRef.detectChanges(); // Update the UI
            });
        },
        error: (error) => console.error('Error completing transaction:', error)
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
  emitTransactionCompletedNotification(transaction: ExtendedTransaction): void {
    const completedNotification = {
        type: 'transactionCompleted',
        message: `Transaction Completed!\nYou bought: ${transaction.grainType} ${transaction.grainClass ? transaction.grainClass : ''}\nWeight: ${transaction.grainWeight}t\nFor: ${transaction.wantedPay.toFixed(2)} Eur`,
        transaction: transaction
    };
    this.notificationService.emitTransactionCompleted(completedNotification);
}
}
