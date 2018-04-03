// angular
import { Component, ViewContainerRef } from "@angular/core";
import { FormBuilder, FormControl, Validators } from "@angular/forms";
// models
import { Branch } from "../../branchs/shared/branch.model";
import { RequireMaintenance,RequireStatus } from "../../require-maintenances/shared/require-maintenance.model";
import { Item } from "../../items/shared/item.model";
// components
import { BaseEditComponent } from "../../shared/base-edit-component";
// 3rd party
import { SelectItem } from "primeng/primeng";
// services
import { AuthService } from "../../core/auth/auth.service";
import { DialogsService } from "../../dialogs/shared/dialogs.service";
import { ShareService } from "../../shared/share.service";
import {
  RequireMaintenService, RequireMaintenCommunicateService
} from "../shared/require-mainten.service";
import { BranchService } from "../../branchs/shared/branch.service";

@Component({
  selector: 'app-require-mainten-edit',
  templateUrl: './require-mainten-edit.component.html',
  styleUrls: ['./require-mainten-edit.component.scss']
})
export class RequireMaintenEditComponent extends BaseEditComponent<RequireMaintenance, RequireMaintenService> {
  constructor(
    service: RequireMaintenService,
    serviceCom: RequireMaintenCommunicateService,
    private serviceBranch: BranchService,
    private serviceShare: ShareService,
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
  branchs: Array<Branch>;
  // on get data by key
  onGetDataByKey(value?: RequireMaintenance): void {
    if (value) {
      this.service.getOneKeyNumber(value)
        .subscribe(dbData => {
          this.editValue = dbData;
        }, error => console.error(error), () => this.buildForm());
    } else {
      this.editValue = {
        RequireMaintenanceId: 0,
        RequireDate: new Date,
        RequireStatus: RequireStatus.Waiting
      };
      this.buildForm();
    }
  }

  // build form
  buildForm(): void {
    //GetData
    this.getBranchs();

    this.editValueForm = this.fb.group({
      RequireMaintenanceId: [this.editValue.RequireMaintenanceId],
      RequireNo: [this.editValue.RequireNo],
      RequireDate: [this.editValue.RequireDate,
        [
          Validators.required,
        ]
      ],
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
      RequireStatus: [this.editValue.RequireStatus],
      GroupMIS: [this.editValue.GroupMIS],
      RequireEmp: [this.editValue.RequireEmp],
      ItemId: [this.editValue.ItemId],
      BranchId: [this.editValue.BranchId],
      ProjectCodeMasterId: [this.editValue.ProjectCodeMasterId],
      MaintenanceApply: [this.editValue.MaintenanceApply],
      // BaseModel
      Creator: [this.editValue.Creator],
      CreateDate: [this.editValue.CreateDate],
      Modifyer: [this.editValue.Modifyer],
      ModifyDate: [this.editValue.ModifyDate],
      // ViewModel
      ItemCode: [this.editValue.ItemCode],
      RequireEmpString: [this.editValue.RequireEmpString],
      ProjectCodeMasterString: [this.editValue.ProjectCodeMasterString],
      GroupMISString: [this.editValue.GroupMISString],
      BranchString: [this.editValue.BranchString],
    });
    this.editValueForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
  }

  // get item type
  getBranchs(): void {
    if (!this.branchs) {
      this.branchs = new Array;
    }
    if (this.branchs) {
      this.serviceBranch.getAll()
        .subscribe(dbBranch => {
          this.branchs = [...dbBranch.sort((item1, item2) => {
            if (item1.Name > item2.Name) {
              return 1;
            }
            if (item1.Name < item2.Name) {
              return -1;
            }
            return 0;
          })];

          if (!this.editValue.BranchId) {
            this.editValue.BranchId = this.branchs.find(item => item.Name.toLowerCase() === "vipco2").BranchId || undefined;
            this.editValueForm.patchValue({
              BranchId: this.editValue.BranchId
            });
          }
        })
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
                RequireEmp: emp.EmpCode,
                RequireEmpString: `คุณ${emp.NameThai}`,
              });
            }
          });
      } else if (type === "Project") {
        this.serviceDialogs.dialogSelectProject(this.viewContainerRef)
          .subscribe(project => {
            if (project) {
              this.editValueForm.patchValue({
                ProjectCodeMasterId: project.ProjectCodeMasterId,
                ProjectCodeMasterString: `${project.ProjectCode}/${project.ProjectName}`,
              });
            }
          });
      } else if (type === "Item") {
        this.serviceDialogs.dialogSelectItem(this.viewContainerRef)
          .subscribe(item => {
            if (item) {
              this.editValueForm.patchValue({
                ItemId: item.ItemId,
                ItemCode: `${item.ItemCode}/${item.Name}`,
              });
            }
          });
      } else if (type === "GroupMis") {
        this.serviceDialogs.dialogSelectGroupMis(this.viewContainerRef)
          .subscribe(groupMis => {
            if (groupMis) {
              this.editValueForm.patchValue({
                GroupMIS: groupMis.GroupMis,
                GroupMISString: `${groupMis.GroupDesc}`,
              });
            }
          });
      }
    }
  }
}
