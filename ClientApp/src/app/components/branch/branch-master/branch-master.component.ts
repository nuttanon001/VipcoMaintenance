import { Component, ViewContainerRef } from "@angular/core";
// components
import { BaseMasterComponent } from "../../../classes/base-master-component";
// models
import {
  Branch, Scroll, ScrollData
} from "../../../models/model.index";
// services
import { AuthService } from "../../../services/auth/auth.service";
import { DialogsService } from "../../../services/dialog/dialogs.service";
import { DataTableServiceCommunicate } from "../../../services/base/data-table.service";
import { BranchService,BranchCommunicateService } from "../../../services/branch/branch.service";
// timezone
import * as moment from "moment-timezone";
// 3rd Party
import { TableColumn } from "@swimlane/ngx-datatable";
import { DateOnlyPipe } from "../../../pipes/date-only.pipe";

@Component({
  selector: "app-branch-master",
  templateUrl: "./branch-master.component.html",
  styleUrls: ["../../../styles/master.style.scss"],
  providers: [DataTableServiceCommunicate]
})
export class BranchMasterComponent
  extends BaseMasterComponent<Branch, BranchService> {

  /** require-painting-master ctor */
  constructor(
    service: BranchService,
    serviceCom: BranchCommunicateService,
    authService: AuthService,
    serviceComDataTable: DataTableServiceCommunicate<Branch>,
    dialogsService: DialogsService,
    viewContainerRef: ViewContainerRef,
  ) {
    super(
      service,
      serviceCom,
      authService,
      serviceComDataTable,
      dialogsService,
      viewContainerRef
    );
  }

  //Parameter
  datePipe: DateOnlyPipe = new DateOnlyPipe("it");
  columns: Array<TableColumn> = [
    { prop: "Name", name: "Name", width: 250 },
    { prop: "Address", name: "Address", width: 150 },
  ];

  // on change time zone befor update to webapi
  changeTimezone(value: Branch): Branch {
    let zone: string = "Asia/Bangkok";
    if (value !== null) {
      if (value.CreateDate !== null) {
        value.CreateDate = moment.tz(value.CreateDate, zone).toDate();
      }
      if (value.ModifyDate !== null) {
        value.ModifyDate = moment.tz(value.ModifyDate, zone).toDate();
      }
    }
    return value;
  }
}
