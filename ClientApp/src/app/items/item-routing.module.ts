import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// component
import { ItemCenterComponent } from "./item-center.component";
import { ItemMasterComponent } from "./item-master/item-master.component";
import { ItemByGroupMasterComponent } from './item-by-group-master/item-by-group-master.component';

const routes: Routes = [
  {
    path: "",
    component: ItemCenterComponent,
    children: [
      {
        path: "item-by-group/:key",
        component: ItemByGroupMasterComponent,
      },
      {
        path: "item-by-group",
        component: ItemByGroupMasterComponent,
      },
      {
        path: ":key",
        component: ItemMasterComponent,
      },
      {
        path: "",
        component: ItemMasterComponent,
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ItemRoutingModule { }
