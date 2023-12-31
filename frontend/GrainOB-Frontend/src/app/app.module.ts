import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { NotificationComponent } from './notification/notification.component';
import { OffersComponent } from './offers/offers.component';
import { AppRoutingModule } from './app-routing.module';
import { FarmersComponent } from './farmers/farmers.component';
import { PricesComponent } from './prices/prices.component';
import { WarehouseComponent } from './warehouse/warehouse.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    SidebarComponent,
    NotificationComponent,
    OffersComponent,
    FarmersComponent,
    PricesComponent,
    WarehouseComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    RouterModule,
    AppRoutingModule,
    FormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
