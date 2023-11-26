import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OffersComponent } from './offers/offers.component';
import { FarmersComponent } from './farmers/farmers.component';
import { PricesComponent } from './prices/prices.component';
import { WarehouseComponent } from './warehouse/warehouse.component';
const routes: Routes = [
  { path: 'offers', component: OffersComponent },
  { path: 'farmers', component: FarmersComponent},
  { path: 'prices', component: PricesComponent},
  { path: 'warehouse', component: WarehouseComponent},
  { path: '', redirectTo: '/warehouse', pathMatch: 'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }