import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { NotificationService } from '../notification.service';
import { Farmer } from '../farmer.service';
import { Truck } from '../truck.service';
import { Transaction, TransactionService } from '../transaction.service';
import { animate, animateChild, query, stagger, style, transition, trigger, AnimationEvent } from '@angular/animations';
 
@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css'],
  animations: [
    trigger('slideInOut', [
      transition('void => in', [
        style({ transform: 'translateX(-100%)', opacity: 0 }),
        animate('0.5s ease-out', style({ transform: 'translateX(0)', opacity: 1 })),
      ]),
      transition('in => out', [
        animate('0.5s ease-out', style({ transform: 'translateY(0px)', opacity: 0 })),
      ]),
    ]),
    trigger('fadeShiftUp', [
      transition('* => *', [
        query(':leave', animateChild(), { optional: true }),
        query(':enter', [
          style({ opacity: 0, transform: 'translateY(20px)' }),
          stagger('50ms', animate('0.5s ease-out', style({ opacity: 1, transform: 'translateY(0px)' }))),
        ], { optional: true })
      ]),
    ]),
  ]
})
  
export class NotificationComponent implements OnInit {
  notifications: any[] = [];
  newFarmer?: Farmer;
  newTruck?: Truck;
  newTransaction?: Transaction;
  animationInProgress = false;

  constructor(
    private notificationService: NotificationService,
    private changeDetectorRef: ChangeDetectorRef,
    private transactionService: TransactionService
  ) {}

  ngOnInit(): void {
    this.notificationService.startConnection().then(() => {
      this.notificationService.addNotificationListeners();
    });
    this.notificationService.notifications$.subscribe(notification => {
      if(this.notifications.length >= 3 && !this.animationInProgress){
        this.animationInProgress = true;

        this.notifications[0].state = 'out';

        setTimeout(() => {
          this.addNotification(notification)},850);
        } else {
          this.addNotification(notification);}
        });
    
    this.notificationService.farmerNotifications$.subscribe(farmer => {
      this.newFarmer = farmer;
      this.changeDetectorRef.detectChanges();
    });

    this.notificationService.truckNotifications$.subscribe(truck => {
      this.newTruck = truck;
      this.changeDetectorRef.detectChanges();
    });

    this.notificationService.transactionNotifications$.subscribe(transaction => {
      this.newTransaction = transaction;
      this.changeDetectorRef.detectChanges();
    });
  }
  acceptOffer(index: number) {
    this.hideOffer(index);
  }
  denyOffer(index: number) {
    this.hideOffer(index);
  }
  hideOffer(index: number) {
    const notification = this.notifications[index];
    if(notification && notification.state !== 'out'){
      notification.state = 'out';
      this.changeDetectorRef.detectChanges();
    }
  }
  statusAccept(transactionId: number) {
    // Call the service to update the status to 'Accepted'
    this.transactionService.updateTransactionStatus(transactionId, 'Accepted').subscribe({
      next: (response: any) => {
        console.log('Accepting transaction with ID:', transactionId);
      },
      error: (error: any) => {
        console.error('Error accepting transaction', error);
      }
    });
  }
  acceptAndChangeStatus(transactionId: number, index:number) {
    this.acceptOffer(index);
    this.statusAccept(transactionId);
  }
  statusDeny(transactionId: number) {
    // Call the service to update the status to 'Denied'
    this.transactionService.updateTransactionStatus(transactionId, 'Denied').subscribe({
      next: (response: any) => {
        console.log('Accepting transaction with ID:', transactionId);
      },
      error: (error: any) => {
        console.error('Error denying transaction', error);
      }
    });
  }
  denyAndChangeStatus(transactionId: number, index:number) {
    this.denyOffer(index);
    this.statusDeny(transactionId);
  }
  addNotification(notification: any): void {
    this.notifications.push({...notification, state: 'in'});
    // Reset the flag after the animation duration to allow the next notification
    setTimeout(() => {
      this.animationInProgress = false;
    }, 850); // This should match the longest duration of enter or exit animations
    this.changeDetectorRef.detectChanges();
  }
  onRemoveAnimationDone(event: AnimationEvent, index: number) {
    if (this.notifications[index] && this.notifications[index].state === 'out') {
      this.notifications.splice(index, 1);
      this.animationInProgress = false;
      this.changeDetectorRef.detectChanges();
    }
  }
  
  getTopPosition(index: number): number {
    const notificationHeight = 100;
    const margin = 10;
    return 5 + (notificationHeight + margin) * index;
  }
  trackByFn(index: any, item: any) {
    return item.id;
  }
}
