import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OffersComponent } from './offers/offers.component';
import { FarmersComponent } from './farmers/farmers.component';
const routes: Routes = [
  { path: 'offers', component: OffersComponent },
  { path: 'farmers', component: FarmersComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }