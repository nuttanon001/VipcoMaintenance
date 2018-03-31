// angular
import { Component, Inject, ViewChild, OnDestroy } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material";
// models
import { Workgroup } from "../../work-groups/shared/workgroup.model";
// service
import { WorkGroupService } from "../../work-groups/shared/work-group.service";
// rxjs
import { Observable } from "rxjs/Observable";
import { Subscription } from "rxjs/Subscription";
// base-component
import { BaseDialogComponent } from "../../shared/base-dialog.component";

@Component({
  selector: "dialog-workgroup",
  templateUrl: "./workgroup-dialog.component.html",
  styleUrls: ["./workgroup-dialog.component.scss"]
})
export class WorkgroupDialogComponent extends BaseDialogComponent<Workgroup, WorkGroupService> {
  /** employee-dialog ctor */
  constructor(
    public service: WorkGroupService,
    public dialogRef: MatDialogRef<WorkgroupDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public mode: number
  ) {
    super(
      service,
      dialogRef
    );
  }
  // Parameter
  columns: Array<string> = ["select", "Name", "Description"];

  // on init
  onInit(): void {
    this.fastSelectd = true;
  }
}
