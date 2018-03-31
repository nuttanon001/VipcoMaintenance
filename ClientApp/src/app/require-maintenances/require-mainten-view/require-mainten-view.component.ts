// angular
import { Component, Output, EventEmitter, Input } from "@angular/core";
// models
import { RequireMaintenance,RequireStatus } from "../shared/require-maintenance.model";
// components
import { BaseViewComponent } from "../../shared/base-view-component";

@Component({
  selector: 'app-require-mainten-view',
  templateUrl: './require-mainten-view.component.html',
  styleUrls: ['./require-mainten-view.component.scss']
})

export class RequireMaintenViewComponent extends BaseViewComponent<RequireMaintenance> {
  constructor() {
    super();
  }
  // load more data
  onLoadMoreData(value: RequireMaintenance) { }
}
