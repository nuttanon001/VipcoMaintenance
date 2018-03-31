// Angular Core
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from "@angular/forms";
// Module
import { ItemRoutingModule } from './item-routing.module';
import { CustomMaterialModule } from '../shared/customer-material/customer-material.module';
// Service
import { BranchService } from '../branchs/shared/branch.service';
import { ItemTypeService } from '../item-types/shared/item-type.service';
import { ItemService, ItemCommunicateService } from '../items/shared/item.service';
// Component
import { ItemMasterComponent } from './item-master/item-master.component';
import { ItemViewComponent } from './item-view/item-view.component';
import { ItemEditComponent } from './item-edit/item-edit.component';
import { ItemCenterComponent } from './item-center.component';
import { ItemTableComponent } from './item-table/item-table.component';


@NgModule({
  imports: [
    CommonModule,
    ItemRoutingModule,
    ReactiveFormsModule,
    CustomMaterialModule
  ],
  declarations: [
    ItemMasterComponent,
    ItemViewComponent,
    ItemEditComponent,
    ItemCenterComponent,
    ItemTableComponent
  ],
  providers: [
    ItemService,
    ItemTypeService,
    BranchService,
    ItemCommunicateService,
  ]
})
export class ItemModule { }
