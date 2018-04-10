//Angular Core
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
//Components
import { ItemMaintenCenterComponent } from './item-mainten-center.component';
import { ItemMaintenMasterComponent } from './item-mainten-master/item-mainten-master.component';
import { ItemMaintenScheduleComponent } from './item-mainten-schedule/item-mainten-schedule.component';
import { AuthGuard } from '../core/auth/auth-guard.service';

const routes: Routes = [{
  path: "",
  component: ItemMaintenCenterComponent,
  children: [
    {
      path: ":condition",
      component: ItemMaintenMasterComponent,
      canActivate: [AuthGuard]
    },
    {
      path: "schedule",
      component: ItemMaintenScheduleComponent,
    },
    {
      path: "schedule/:condition",
      component: ItemMaintenScheduleComponent,
    },
    {
      path: "",
      component: ItemMaintenMasterComponent,
      canActivate: [AuthGuard]
    }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ItemMaintenRoutingModule { }
