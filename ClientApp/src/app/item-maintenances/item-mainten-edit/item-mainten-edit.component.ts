// angular
import { Component, ViewContainerRef } from "@angular/core";
import { FormBuilder, FormControl, Validators } from "@angular/forms";
// models
import { ItemMaintenance, StatusMaintenance } from "../shared/item-maintenance.model";
import { RequireMaintenance, RequireStatus } from "../../require-maintenances/shared/require-maintenance.model";
import { TypeMaintenance } from "../../type-maintenances/shared/type-maintenance.model";
import { RequisitionStock } from "../../inventories/shared/requisition-stock.model";
import { WorkGroupMaintenance } from "../../work-group-maintenances/shared/work-group-maintenance";
// components
import { BaseEditComponent } from "../../shared/base-edit-component";
// services
import { AuthService } from "../../core/auth/auth.service";
import { DialogsService } from "../../dialogs/shared/dialogs.service";
import { TypeMaintenService } from "../../type-maintenances/shared/type-mainten.service";
import { RequisitionStockService } from "../../inventories/shared/requisition-stock.service";
import { RequireMaintenService } from "../../require-maintenances/shared/require-mainten.service";
import { ItemMaintenService, ItemMaintenCommunicateService } from "../shared/item-mainten.service";
import { WorkGroupMaintenService } from "../../work-group-maintenances/shared/work-group-mainten.service";
import { ItemMaintenHasEmpService } from "../shared/item-mainten-has-emp.service";
import { ItemMaintenanceHasEmp } from "../shared/item-maintenance-has-emp.model";

@Component({
  selector: 'app-item-mainten-edit',
  templateUrl: './item-mainten-edit.component.html',
  styleUrls: ['./item-mainten-edit.component.scss']
})

export class ItemMaintenEditComponent extends BaseEditComponent<ItemMaintenance, ItemMaintenService> {
  constructor(
    service: ItemMaintenService,
    serviceCom: ItemMaintenCommunicateService,
    private serviceItemMainEmp: ItemMaintenHasEmpService,
    private serviceGroupMainten: WorkGroupMaintenService,
    private serviceRequireMainten: RequireMaintenService,
    private serviceRequisitionStock: RequisitionStockService,
    private serviceTypeMainten: TypeMaintenService,
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
  groupMaintenances: Array<WorkGroupMaintenance>;
  requisition: RequisitionStock;
  indexItem: number;
  // on get data by key
  onGetDataByKey(value?: ItemMaintenance): void {
    if (value && value.ItemMaintenanceId) {
      this.service.getOneKeyNumber(value)
        .subscribe(dbData => {
          if (dbData) {
            this.editValue = dbData;
            //Requistion
            this.serviceRequisitionStock.getRequisitionByItemMaintenance(dbData.ItemMaintenanceId)
              .subscribe(dbRequisition => {
                this.editValue.RequisitionStockSps = new Array;
                if (dbRequisition) {
                  dbRequisition.forEach((item, index) => {
                    this.editValue.RequisitionStockSps.push({
                      CreateDate: item.CreateDate,
                      Creator: item.Creator,
                      ItemMaintenanceId: item.ItemMaintenanceId,
                      ModifyDate: item.ModifyDate,
                      Modifyer: item.Modifyer,
                      MovementStockSpId: item.MovementStockSpId,
                      PaperNo: item.PaperNo,
                      Quantity: item.Quantity,
                      Remark: item.Remark,
                      RequisitionDate: item.RequisitionDate,
                      RequisitionEmp: item.RequisitionEmp,
                      RequisitionEmpString: item.RequisitionEmpString,
                      RequisitionStockSpId: item.RequisitionStockSpId,
                      SparePartId: item.SparePartId,
                      SparePartName: item.SparePartName,
                      TotalPrice: item.TotalPrice,
                      UnitPrice: item.UnitPrice
                    });
                  });
                }
                this.editValueForm.patchValue({
                  RequisitionStockSps: this.editValue.RequisitionStockSps
                });
              });
            //Employee
            this.serviceItemMainEmp.actionItemMaintenanceHasEmployee(dbData.ItemMaintenanceId)
              .subscribe(ItemMainHasEmp => {
                this.editValue.ItemMainHasEmployees = new Array;
                if (ItemMainHasEmp) {
                  ItemMainHasEmp.forEach((item, index) => {
                    this.editValue.ItemMainHasEmployees.push({
                      CreateDate: item.CreateDate,
                      Creator: item.Creator,
                      EmpCode: item.EmpCode,
                      ItemMainEmpString: item.ItemMainEmpString,
                      ItemMainHasEmployeeId: item.ItemMainHasEmployeeId,
                      ItemMaintenanceId: item.ItemMaintenanceId,
                      ModifyDate: item.ModifyDate,
                      Modifyer: item.Modifyer,
                      Remark:item.Remark
                    });
                  });
                  //Patch value to Form
                  this.editValueForm.patchValue({
                    ItemMainHasEmployees: this.editValue.ItemMainHasEmployees
                  });
                }
              });
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
        RequisitionStockSps: new Array,
        ItemMainHasEmployees: new Array,
        StatusMaintenance: StatusMaintenance.InProcess,
        Description:"-"
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
    // Get group maintenances
    this.getGroupMaintenances();
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
      TypeMaintenanceId: [this.editValue.TypeMaintenanceId,
      [
        Validators.required,
      ]
      ],
      RequisitionStockSps: [this.editValue.RequisitionStockSps],
      ItemMainHasEmployees: [this.editValue.ItemMainHasEmployees],
      WorkGroupMaintenanceId: [this.editValue.WorkGroupMaintenanceId,
      [
        Validators.required,
      ]
      ],
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

  // get group maintenance
  getGroupMaintenances(): void {
    if (!this.groupMaintenances) {
      this.groupMaintenances = new Array;
    }

    this.serviceGroupMainten.getAll()
      .subscribe(dbData => {
        if (dbData) {
          this.groupMaintenances = [...dbData];
        }
      })
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
      } else if (type === "RequireMaintenance") {
        this.serviceDialogs.dialogSelectRequireMaintenance(this.requireMainten.RequireMaintenanceId, this.viewContainerRef, false);
      } else if (type === "Employees") {
        // New Array
        if (!this.editValue.ItemMainHasEmployees) {
          this.editValue.ItemMainHasEmployees = new Array;
        }
        this.serviceDialogs.dialogSelectEmployees(this.viewContainerRef, 1)
          .subscribe(Employees => {
            if (Employees) {

              Employees.forEach((item, index) => {
                if (!this.editValue.ItemMainHasEmployees.find(itemEmp => itemEmp.EmpCode === item.EmpCode)) {
                  this.editValue.ItemMainHasEmployees.push({
                    EmpCode: item.EmpCode,
                    ItemMainEmpString: item.NameThai,
                    ItemMaintenanceId: this.editValue.ItemMaintenanceId,
                    ItemMainHasEmployeeId:0,
                  });
                }
              });

              this.editValue.ItemMainHasEmployees = this.editValue.ItemMainHasEmployees.slice();
              // Update to form
              this.editValueForm.patchValue({
                ItemMainHasEmployees: this.editValue.ItemMainHasEmployees
              });
              this.onValueChanged();
            }
          });
      }
    }
  }

  // action Requisition add | edit
  actionRequisition(anyData?: { data: RequisitionStock, mode: number }): void {
    if (!anyData) {
      this.requisition = {
        RequisitionStockSpId: 0,
        RequisitionDate: new Date,
        Quantity: 1,
        ItemMaintenanceId: this.editValue.ItemMaintenanceId,
      };
      this.indexItem = -1;
    } else {
      this.indexItem = this.editValue.RequisitionStockSps.indexOf(anyData.data);
      if (anyData.mode === 1) {
        this.requisition = anyData.data;
      } else {
        this.editValue.RequisitionStockSps.splice(this.indexItem, 1);
        this.editValue.RequisitionStockSps = this.editValue.RequisitionStockSps.slice();
        // Update to form
        this.editValueForm.patchValue({
          RequisitionStockSps: this.editValue.RequisitionStockSps
        });
      }
    }
  }

  // Requisition complate or cancel
  onComplateOrCancel(requisition?: RequisitionStock): void {
    if (requisition) {
      if (this.indexItem > -1) {
        // remove item
        this.editValue.RequisitionStockSps.splice(this.indexItem, 1);
      }
      // cloning an object
      this.editValue.RequisitionStockSps.push(Object.assign({}, requisition));
      this.editValue.RequisitionStockSps = this.editValue.RequisitionStockSps.slice();
      // Update to form
      this.editValueForm.patchValue({
        RequisitionStockSps: this.editValue.RequisitionStockSps
      });
      this.onValueChanged();
    }
    this.requisition = undefined;
    this.onValueChanged();
  }

  // ItemEmployee remove
  onItemMaintenanceEmployeeRemove(anyData?: { data: ItemMaintenanceHasEmp, option: number }): void {
    // console.log("Data is", JSON.stringify(anyData));
    if (anyData) {
      if (anyData.option === 0) {
        // Found Index
        let indexItem: number = this.editValue.ItemMainHasEmployees.indexOf(anyData.data);
        // Remove at Index
        this.editValue.ItemMainHasEmployees.splice(indexItem, 1);
        // Angular need change data for update view
        this.editValue.ItemMainHasEmployees = this.editValue.ItemMainHasEmployees.slice();
        // Update to form
        this.editValueForm.patchValue({
          ItemMainHasEmployees: this.editValue.ItemMainHasEmployees
        });
      }
    }
  }
}
