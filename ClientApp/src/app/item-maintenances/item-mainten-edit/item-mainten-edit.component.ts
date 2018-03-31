// angular
import { Component, ViewContainerRef } from "@angular/core";
import { FormBuilder, FormControl, Validators } from "@angular/forms";
// models
import { ItemMaintenance } from "../shared/item-maintenance.model";
import { RequireMaintenance,RequireStatus } from "../../require-maintenances/shared/require-maintenance.model";
import { TypeMaintenance } from "../../type-maintenances/shared/type-maintenance.model";
// components
import { BaseEditComponent } from "../../shared/base-edit-component";
// services
import { AuthService } from "../../core/auth/auth.service";
import { DialogsService } from "../../dialogs/shared/dialogs.service";
import { ItemMaintenService, ItemMaintenCommunicateService } from "../shared/item-mainten.service";
import { RequireMaintenService } from "../../require-maintenances/shared/require-mainten.service";
import { TypeMaintenService } from "../../type-maintenances/shared/type-mainten.service";

@Component({
  selector: 'app-item-mainten-edit',
  templateUrl: './item-mainten-edit.component.html',
  styleUrls: ['./item-mainten-edit.component.scss']
})
export class ItemMaintenEditComponent extends BaseEditComponent<ItemMaintenance, ItemMaintenService> {
  constructor(
    service: ItemMaintenService,
    serviceCom: ItemMaintenCommunicateService,
    private serviceRequireMainten: RequireMaintenService,
    private serviceTypeMainten:TypeMaintenService,
    private serviceAuth: AuthService,
    private serviceDialogs: DialogsService,
    private viewContainerRef: ViewContainerRef,
    private fb: FormBuilder
  ) {
    super(
      service,
      serviceCom,
    );
  }

  // Parameter
  requireMainten: RequireMaintenance;
  typeMaintenances: Array<TypeMaintenance>;
  // on get data by key
  onGetDataByKey(value?: ItemMaintenance): void {
    if (value && value.ItemMaintenanceId) {
      this.service.getOneKeyNumber(value)
        .subscribe(dbData => {
          if (dbData) {
            this.editValue = dbData;
          }
        }, error => console.error(error), () => {
          this.buildForm();
          if (this.editValue.RequireMaintenanceId) {
            this.serviceRequireMainten.getOneKeyNumber({ RequireMaintenanceId: this.editValue.RequireMaintenanceId })
              .subscribe(dbDataReq => {
                if (dbDataReq) {
                  this.requireMainten = dbDataReq;
                }
              })
          }
        });
    } else {
      this.editValue = {
        ItemMaintenanceId: 0,
        PlanStartDate: new Date,
        PlanEndDate: new Date,
      };

      if (value) {
        if (value.RequireMaintenanceId) {
          this.editValue.RequireMaintenanceId = value.RequireMaintenanceId;
          this.serviceRequireMainten.getOneKeyNumber({ RequireMaintenanceId: this.editValue.RequireMaintenanceId })
            .subscribe(dbData => {
              if (dbData) {
                this.requireMainten = dbData;
                if (!this.typeMaintenances) {
                  this.getTypeMaintenances();
                } else if (this.typeMaintenances.length < 1) {
                  this.getTypeMaintenances();
                }
              }
            });
        }
      }
      this.buildForm();
    }
  }

  // build form
  buildForm(): void {
    // Get type maintenances
    this.getTypeMaintenances();
    // New form
    this.editValueForm = this.fb.group({
      ItemMaintenanceId: [this.editValue.ItemMaintenanceId],
      ItemMaintenanceNo: [this.editValue.ItemMaintenanceNo],
      PlanStartDate: [this.editValue.PlanStartDate,
        [
          Validators.required,
        ]
      ],
      PlanEndDate: [this.editValue.PlanEndDate,
        [
          Validators.required,
        ]
      ],
      ActualStartDate: [this.editValue.ActualStartDate],
      ActualEndDate: [this.editValue.ActualEndDate],
      StatusMaintenance: [this.editValue.StatusMaintenance],
      Description: [this.editValue.Description,
        [
          Validators.required,
          Validators.maxLength(250)
        ]
      ],
      Remark: [this.editValue.Remark,
        [
          Validators.maxLength(250)
        ]
      ],
      MaintenanceEmp: [this.editValue.MaintenanceEmp],
      RequireMaintenanceId: [this.editValue.RequireMaintenanceId],
      TypeMaintenanceId: [this.editValue.TypeMaintenanceId],
      // BaseModel
      Creator: [this.editValue.Creator],
      CreateDate: [this.editValue.CreateDate],
      Modifyer: [this.editValue.Modifyer],
      ModifyDate: [this.editValue.ModifyDate],
      // ViewModel
      ItemCode: [this.editValue.ItemCode],
      MaintenanceEmpString: [this.editValue.MaintenanceEmpString],
      TypeMaintenanceString: [this.editValue.TypeMaintenanceString],
      StatusMaintenanceString: [this.editValue.StatusMaintenanceString],
    });
    this.editValueForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
  }

  // get type maintenance
  getTypeMaintenances(): void {
    if (!this.typeMaintenances) {
      this.typeMaintenances = new Array;
    }

    if (this.requireMainten) {
      this.serviceTypeMainten.getTypeMaintenanceByItem(this.requireMainten.ItemId)
        .subscribe(dbData => {
          if (dbData) {
            this.typeMaintenances = [...dbData];
          }
        });
    }
  }

  // open dialog
  openDialog(type?: string): void {
    if (type) {
      if (type === "Employee") {
        this.serviceDialogs.dialogSelectEmployee(this.viewContainerRef)
          .subscribe(emp => {
            if (emp) {
              this.editValueForm.patchValue({
                MaintenanceEmp: emp.EmpCode,
                MaintenanceEmpString: `คุณ${emp.NameThai}`,
              });
            }
          });
      }  
    }
  }
}
