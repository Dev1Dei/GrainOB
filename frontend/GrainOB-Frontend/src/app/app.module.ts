import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { TransactionListComponent } from './transaction-list/transaction-list.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { NotificationComponent } from './notification/notification.component';
import { OffersComponent } from './offers/offers.component';
import { AppRoutingModule } from './app-routing.module';
import { FarmersComponent } from './farmers/farmers.component';

const routes: Routes = [
  {path: 'offers', component: OffersComponent}
]

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    TransactionListComponent,
    SidebarComponent,
    NotificationComponent,
    OffersComponent,
    FarmersComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    RouterModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
