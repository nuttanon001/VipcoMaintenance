// angular
import { Component, Output, EventEmitter, Input } from "@angular/core";
// models
import { ItemMaintenance } from "../shared/item-maintenance.model";
import { RequireMaintenance } from "../../require-maintenances/shared/require-maintenance.model";
// components
import { BaseViewComponent } from "../../shared/base-view-component";
// services
import { RequireMaintenService } from "../../require-maintenances/shared/require-mainten.service";

@Component({
  selector: 'app-item-mainten-view',
  templateUrl: './item-mainten-view.component.html',
  styleUrls: ['./item-mainten-view.component.scss']
})
export class ItemMaintenViewComponent extends BaseViewComponent<ItemMaintenance> {
  constructor(
    private serviceRequireMainten:RequireMaintenService
  ) {
    super();
  }
  // Parameter
  requireMainten: RequireMaintenance;
  // load more data
  onLoadMoreData(value: ItemMaintenance) {
    if (value) {
      if (value.RequireMaintenanceId) {
        this.serviceRequireMainten.getOneKeyNumber({ RequireMaintenanceId: value.RequireMaintenanceId })
          .subscribe(dbData => {
            if (dbData) {
              this.requireMainten = dbData;
            }
          })
      }
    }
  }
}

