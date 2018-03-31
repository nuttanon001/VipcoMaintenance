import { Component, ViewContainerRef, ViewChild } from "@angular/core";
import { Router, ActivatedRoute, ParamMap } from "@angular/router";
// components
import { BaseMasterComponent } from "../../shared/base-master-component";
// models
import { ItemMaintenance } from "../shared/item-maintenance.model";
// services
import { ItemMaintenService, ItemMaintenCommunicateService } from "../shared/item-mainten.service";
import { AuthService } from "../../core/auth/auth.service";
import { DialogsService } from "../../dialogs/shared/dialogs.service";
// timezone
import * as moment from "moment-timezone";
import { ItemMaintenTableComponent } from "../item-mainten-table/item-mainten-table.component";

@Component({
  selector: 'app-item-mainten-master',
  templateUrl: './item-mainten-master.component.html',
  styleUrls: ['./item-mainten-master.component.scss']
})
export class ItemMaintenMasterComponent extends BaseMasterComponent<ItemMaintenance, ItemMaintenService> {
  constructor(
    service: ItemMaintenService,
    serviceCom: ItemMaintenCommunicateService,
    authService: AuthService,
    dialogsService: DialogsService,
    viewContainerRef: ViewContainerRef,
    private router: Router,
    private route: ActivatedRoute,
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
  backToSchedule: boolean = false;

  @ViewChild(ItemMaintenTableComponent)
  private tableComponent: ItemMaintenTableComponent;
  // override
  ngOnInit(): void {
    // override class
    super.ngOnInit();

    this.route.paramMap.subscribe((param: ParamMap) => {
      let key: number = Number(param.get("condition") || 0);

      if (key) {
        // can go back to last page
        this.backToSchedule = true;

        let itemMainten: ItemMaintenance = {
          ItemMaintenanceId: 0,
          PlanStartDate: new Date,
          PlanEndDate: new Date,
          RequireMaintenanceId: key
        };
        setTimeout(() => {
          this.onDetailEdit(itemMainten);
        }, 500);
      }
    }, error => console.error(error));
  }

  // on change time zone befor update to webapi
  changeTimezone(value: ItemMaintenance): ItemMaintenance {
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
}
