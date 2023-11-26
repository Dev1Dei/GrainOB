import { Component } from '@angular/core';
import { UserBalanceService, UserBalanceResponse } from '../user-balance.service';
import { StorageContainerService, StorageContainerDto, StorageContainer } from '../storage-container.service';
import { BalanceUpdateService } from '../balance-update.service';
import { PricesService} from '../prices.service';
import { forkJoin, map } from 'rxjs';
@Component({
  selector: 'app-warehouse',
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.css']
})
export class WarehouseComponent {
  showBuyOptions = false;
  selectedCapacity: number = 500; // Default selected size
  containers: StorageContainer[] = []; // Array to hold the containers
  newContainerName: string = '';
  prices: any[] = [];
  latestPrice: number = 0;
  balance: number | null = null;
  gainedBalance: number = 0;

  constructor(
    private userBalanceService: UserBalanceService,
    private storageContainerService: StorageContainerService,
    private balanceUpdateService: BalanceUpdateService,
    private priceService: PricesService,
  ) {}

  ngOnInit(): void {
    this.storageContainerService.getContainers(1).subscribe({
      next: (containers: StorageContainer[]) => {
        this.containers = containers.slice(-6);
      },
      error: (error) => {
        console.error('Error fetching containers', error);
      }
    });
    this.fetchInitialData();
  }

  calculateCost(capacity: number): number {
    const costPerTonne = 80; // Cost per tonne
    return capacity * costPerTonne;
  }
  private fetchInitialData() {
    const priceObservables = this.containers.map(container => 
      this.priceService.getLatestPriceByGrainTypeAndClass(container.grainType, container.grainClass).pipe(
        map((latestPriceData: { price: any; }) => ({ containerId: container.containerId, price: latestPriceData.price }))
      )
    );

  forkJoin(priceObservables).subscribe(results => {
    results.forEach(result => {
      const correspondingContainer = this.containers.find(c => c.containerId === result.containerId);
      if (correspondingContainer) {
        correspondingContainer.gainingBalance = correspondingContainer.weight * result.price;
      }
    });
    // Now all containers have their gainedBalance calculated
  });
  }
  toggleBuyOptions(): void {
    this.showBuyOptions = !this.showBuyOptions;
  }

  buyContainer(): void {
    const cost = this.calculateCost(this.selectedCapacity);
    this.userBalanceService.getBalance().subscribe({
      next: (balanceResponse: UserBalanceResponse) => {
        const newBalance = balanceResponse.balance - cost;
        if (newBalance >= 0) {
          this.userBalanceService.updateBalance(balanceResponse.userId, newBalance).subscribe({
            next: () => {
              const containerData: StorageContainerDto = {
                userId: 1,
                grainType: 'None',
                grainClass: 'None',
                totalCapacity: this.selectedCapacity,
                name: this.newContainerName
              };
              this.storageContainerService.createContainer(containerData).subscribe({
                next: (containerResponse: StorageContainer) => {
                  this.containers.push(containerResponse);
                  if (this.containers.length > 6) {
                    this.containers.shift(); // Make sure only 6 containers are displayed
                  }
                  this.showBuyOptions = false; // Hide the buy options
                  this.balanceUpdateService.updateBalance(newBalance); // Notify other components of balance update
                },
                error: (error) => {
                  console.error('Error creating container', error);
                }
              });
            },
            error: (error) => {
              console.error('Error updating balance', error);
            }
          });
        } else {
          console.error('Insufficient funds.');
        }
      },
      error: (error) => {
        console.error('Error retrieving balance', error);
      }
    });
  }
  
  cleanContainer(containerId: number): void {
    this.storageContainerService.cleanContainer(containerId).subscribe(() => {
      // Handle successful cleaning, e.g., update the local state or refetch the containers
      this.fetchContainers(); // Assuming you have a method to refetch containers
    }, (error) => {
      console.error('Error cleaning container', error);
    });
  }
  dryContainer(containerId: number): void {
    this.storageContainerService.dryContainer(containerId).subscribe(() => {
      // Handle successful drying, e.g., update the local state or refetch the containers
      this.fetchContainers(); // Assuming you have a method to refetch containers
    }, (error) => {
      console.error('Error drying container', error);
    });
  }
  private fetchContainers(): void {
    this.storageContainerService.getContainers(1).subscribe({
      next: (containers: StorageContainer[]) => {
        this.containers = containers.slice(-6);
        // Optionally, if you want to ensure the sidebar balance is always updated:
        this.userBalanceService.getBalance().subscribe({
          next: (response: UserBalanceResponse) => {
            this.balanceUpdateService.updateBalance(response.balance);
          },
          error: (error) => {
            console.error('Error fetching balance for sidebar update', error);
          }
        });
      },
      error: (error) => {
        console.error('Error fetching containers', error);
      }
    });
  }
  displayPrice(containerId: number, grainType: string, grainClass: string, weight: number): void{
    this.priceService.getLatestPriceByGrainTypeAndClass(grainType, grainClass).subscribe((latestPriceData) => {
      if(latestPriceData){
        this.latestPrice = latestPriceData.price;
        console.log(latestPriceData);
        console.log(latestPriceData.price);
        this.gainedBalance = weight * latestPriceData.price;
      }
      return this.gainedBalance;
    })
  }
  sellGrain(containerId: number, grainType: string, grainClass: string, weight: number): void {
    // Fetch the latest price.
    this.priceService.getLatestPriceByGrainTypeAndClass(grainType, grainClass).subscribe((latestPriceData) => {
      if (latestPriceData) {
        this.latestPrice = latestPriceData.price;
        console.log(latestPriceData);
        this.gainedBalance = weight * this.latestPrice; // Calculate gained balance.
  
        // Fetch the current user balance.
        this.userBalanceService.getBalance().subscribe({
          next: (balanceResponse: UserBalanceResponse) => {
            const newBalance = balanceResponse.balance + this.gainedBalance; // Calculate new balance.
            
            // Update the user's balance.
            this.userBalanceService.updateBalance(balanceResponse.userId, newBalance).subscribe(() => {
              this.balanceUpdateService.updateBalance(newBalance); // Notify about the balance update.
              console.log("Users balance: ", balanceResponse.balance);
              console.log("Weight: ", weight);
              console.log("Latest Price: ", this.latestPrice);
              console.log("Gained amount:", this.gainedBalance);
              console.log("Updated Balance: ", newBalance); // Corrected text from "Updated Price" to "Updated Balance".
            });
          }
        });
  
      } else {
        // Handle the case where no price data is found.
        console.error("No price data available for the provided grainType and grainClass.");
      }
    });
  
    // Clear the storage container.
    this.storageContainerService.clearContainer(containerId).subscribe(() => {
      this.fetchContainers(); // Fetch the updated list of containers.
    });
  }  
}