import { Component, OnInit, Inject, ViewChild } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
// models
import { ItemMaintenance } from "../../item-maintenances/shared/item-maintenance.model";
//Services
import { RequireMaintenService } from '../../require-maintenances/shared/require-mainten.service';
import { ItemMaintenHasEmpService } from '../../item-maintenances/shared/item-mainten-has-emp.service';
import { RequisitionStockService } from '../../inventories/shared/requisition-stock.service';
import { ItemMaintenService } from "../../item-maintenances/shared/item-mainten.service";

@Component({
  selector: 'app-item-mainten-dialog',
  templateUrl: './item-mainten-dialog.component.html',
  styleUrls: ['./item-mainten-dialog.component.scss'],
  providers: [
    RequireMaintenService,
    ItemMaintenHasEmpService,
    RequisitionStockService,
    ItemMaintenService
  ]
})
export class ItemMaintenDialogComponent implements OnInit {
  /** require-painting-view-dialog ctor */
  constructor(
    private service: ItemMaintenService,
    @Inject(MAT_DIALOG_DATA) public data : number,
    private dialogRef: MatDialogRef<number>) { }

  // Parameter
  itemMainten: ItemMaintenance;
  canClose: boolean;

  /** Called by Angular after cutting-plan-dialog component initialized */
  ngOnInit(): void {
    this.canClose = false;
    if (this.data) {
      this.service.getOneKeyNumber({
        ItemMaintenanceId: this.data,
        PlanEndDate: new Date,
        PlanStartDate: new Date
      })
        .subscribe(dbData => {
          if (dbData) {
            this.itemMainten = dbData;
          } else {
            this.onCancelClick();
          }
        });
    } else {
      this.onCancelClick();
    }
  }
  // No Click
  onCancelClick(): void {
      this.dialogRef.close();
  }
}
