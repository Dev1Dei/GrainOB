import { HttpClient } from '@angular/common/http';
import { Injectable, NgZone } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private hubConnection: signalR.HubConnection;
  private farmerNotificationSubject = new Subject<any>();
  private truckNotificationSubject = new Subject<any>();
  private transactionNotificationSubject = new Subject<any>();
  private notificationSubject = new Subject<any>();

  public farmerNotifications$ = this.farmerNotificationSubject.asObservable();
  public truckNotifications$ = this.truckNotificationSubject.asObservable();
  public transactionNotifications$ = this.transactionNotificationSubject.asObservable();
  public notifications$ = this.notificationSubject.asObservable();

  constructor(private zone: NgZone, private http: HttpClient) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5003/notificationhub')
      .configureLogging(signalR.LogLevel.Debug)
      .build();
  }

  public startConnection(): Promise<void> {
    return this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.error('Error while starting connection: ' + err));
  }
  public emitTransactionCompleted(notification: any) {
    console.log('Emitting transaction completed notification:', notification);
    this.zone.run(() => {
        this.notificationSubject.next(notification);
    });
}
  public addNotificationListeners(): void {
    this.hubConnection.on('CreationEventOccurred', (notification) => {
      this.zone.run(() => {
        this.notificationSubject.next(notification);
        if (notification.hasOwnProperty('farmer')) {
          this.farmerNotificationSubject.next(notification.farmer);
        }
        if (notification.hasOwnProperty('truck')) {
          this.truckNotificationSubject.next(notification.truck);
        }
        if (notification.hasOwnProperty('transaction')) {
          this.transactionNotificationSubject.next(notification.transaction);
        }
      });
    });
  }
}
