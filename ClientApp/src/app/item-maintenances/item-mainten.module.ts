//Angular Core
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
//Modules
import { CustomMaterialModule } from '../shared/customer-material/customer-material.module';
import { ItemMaintenRoutingModule } from './item-mainten-routing.module';
//Services
import { ItemMaintenService, ItemMaintenCommunicateService } from './shared/item-mainten.service';
import { RequireMaintenService } from "../require-maintenances/shared/require-mainten.service";
import { TypeMaintenService } from '../type-maintenances/shared/type-mainten.service';
//Components
import { ItemMaintenMasterComponent } from './item-mainten-master/item-mainten-master.component';
import { ItemMaintenViewComponent } from './item-mainten-view/item-mainten-view.component';
import { ItemMaintenEditComponent } from './item-mainten-edit/item-mainten-edit.component';
import { ItemMaintenTableComponent } from './item-mainten-table/item-mainten-table.component';
import { ItemMaintenCenterComponent } from './item-mainten-center.component';
import { ItemMaintenHasRequireComponent } from './shared/item-mainten-has-require.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CustomMaterialModule,
    ItemMaintenRoutingModule
  ],
  declarations: [
    ItemMaintenMasterComponent,
    ItemMaintenViewComponent,
    ItemMaintenEditComponent,
    ItemMaintenTableComponent,
    ItemMaintenCenterComponent,
    ItemMaintenHasRequireComponent],
  providers: [
    ItemMaintenService,
    ItemMaintenCommunicateService,
    RequireMaintenService,
    TypeMaintenService,
  ]
})
export class ItemMaintenModule { }
