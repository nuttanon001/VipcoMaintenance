import { Component, ViewContainerRef, ViewChild } from "@angular/core";
// components
import { BaseMasterComponent } from "../../shared/base-master-component";
import { RequireMaintenTableComponent } from "../require-mainten-table/require-mainten-table.component";
// models
import { RequireMaintenance, RequireStatus } from "../shared/require-maintenance.model";
// services
import { AuthService } from "../../core/auth/auth.service";
import { DialogsService } from "../../dialogs/shared/dialogs.service";
import { RequireMaintenService, RequireMaintenCommunicateService } from "../shared/require-mainten.service";
// timezone
import * as moment from "moment-timezone";
import { BranchService } from "../../branchs/shared/branch.service";

@Component({
  selector: 'app-require-mainten-master',
  templateUrl: './require-mainten-master.component.html',
  styleUrls: ['./require-mainten-master.component.scss'],
  providers: [BranchService]
})
export class RequireMaintenMasterComponent
  extends BaseMasterComponent<RequireMaintenance, RequireMaintenService> {
  constructor(
    service: RequireMaintenService,
    serviceCom: RequireMaintenCommunicateService,
    authService: AuthService,
    dialogsService: DialogsService,
    viewContainerRef: ViewContainerRef,
  ) {
    super(
      service,
      serviceCom,
      authService,
      dialogsService,
      viewContainerRef
    );
  }

  //Parameter

  @ViewChild(RequireMaintenTableComponent)
  private tableComponent: RequireMaintenTableComponent;
  // on change time zone befor update to webapi
  changeTimezone(value: RequireMaintenance): RequireMaintenance {
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

  // onReload
  onReloadData(): void {
    this.tableComponent.reloadData();
  }

  // on detail edit override
  onDetailEdit(editValue?: RequireMaintenance): void {
    if (editValue) {
      if (editValue.RequireStatus !== RequireStatus.Waiting) {
        this.dialogsService.error("Access Deny", "คำขอซ่อมบำรุง อยู่ขณะดำเนินการไม่สามารถแก้ไขได้ !!!", this.viewContainerRef);
        return;
      }
    }
    super.onDetailEdit(editValue);
  }
}
