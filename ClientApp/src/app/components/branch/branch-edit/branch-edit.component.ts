// angular
import { Component, ViewContainerRef } from "@angular/core";
import { FormBuilder, FormControl, Validators } from "@angular/forms";
// models
import { Branch } from "../../../models/model.index";
// components
import { BaseEditComponent } from "../../../classes/base-edit-component";
// 3rd party
import { SelectItem } from "primeng/primeng";
import { TableColumn } from "@swimlane/ngx-datatable";
// pipe
import { DateOnlyPipe } from "../../../pipes/date-only.pipe";
// services
import { AuthService } from "../../../services/auth/auth.service";
import { DialogsService } from "../../../services/dialog/dialogs.service";
import { BranchService, BranchCommunicateService } from "../../../services/branch/branch.service";

@Component({
  selector: 'app-branch-edit',
  templateUrl: './branch-edit.component.html',
  styleUrls: ["../../../styles/edit.style.scss"],
})
export class BranchEditComponent extends BaseEditComponent<Branch, BranchService> {
  constructor(
    service: BranchService,
    serviceCom: BranchCommunicateService,
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

  // on get data by key
  onGetDataByKey(value?: Branch): void {
    if (value) {
      this.service.getOneKeyNumber(value.BranchId)
        .subscribe(dbData => {
          this.editValue = dbData;
        }, error => console.error(error), () => this.buildForm());
    } else {
      this.editValue = {
        BranchId: 0,
      };
      this.buildForm();
    }
  }

  // build form
  buildForm(): void {
    this.editValueForm = this.fb.group({
      BranchId: [this.editValue.BranchId],
      Name: [this.editValue.Name,
      [
        Validators.required,
        Validators.maxLength(50)
      ]
      ],
      Address: [this.editValue.Address,
      [
        Validators.maxLength(250),
      ]
      ],
      Creator: [this.editValue.Creator],
      CreateDate: [this.editValue.CreateDate],
      Modifyer: [this.editValue.Modifyer],
      ModifyDate: [this.editValue.ModifyDate],
    });

    this.editValueForm.valueChanges.subscribe((data: any) => this.onValueChanged(data));
  }
}
