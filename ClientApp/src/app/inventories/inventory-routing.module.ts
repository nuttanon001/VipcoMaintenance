import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InventoryCenterComponent } from './inventory-center.component';
import { ReceiveMasterComponent } from './receive-master/receive-master.component';

const routes: Routes = [{
  path: "",
  component: InventoryCenterComponent,
  children: [
    {
      path: "receive:key",
      component: ReceiveMasterComponent,
    },
    {
      path: "receive",
      component: ReceiveMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InventoriyRoutingModule { }
