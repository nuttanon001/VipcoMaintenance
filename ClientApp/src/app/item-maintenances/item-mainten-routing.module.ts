//Angular Core
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
//Components
import { ItemMaintenCenterComponent } from './item-mainten-center.component';
import { ItemMaintenMasterComponent } from './item-mainten-master/item-mainten-master.component';

const routes: Routes = [{
  path: "",
  component: ItemMaintenCenterComponent,
  children: [
    {
      path: ":condition",
      component: ItemMaintenMasterComponent,
    },
    {
      path: "",
      component: ItemMaintenMasterComponent,
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ItemMaintenRoutingModule { }
