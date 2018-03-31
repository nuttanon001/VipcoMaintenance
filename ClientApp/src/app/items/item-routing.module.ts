import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
// component
import { ItemCenterComponent } from "./item-center.component";
import { ItemMasterComponent } from "./item-master/item-master.component";

const routes: Routes = [
  {
    path: "",
    component: ItemCenterComponent,
    children: [
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
