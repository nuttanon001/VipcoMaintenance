import { BaseModel } from "../../shared/base-model.model";

export interface RequireMaintenance extends BaseModel {
  RequireMaintenanceId: number;
  RequireNo?: string;
  RequireDate?: Date;
  Description?: string;
  Remark?: string;
  RequireStatus?: RequireStatus;
  MailApply?: string;
  MaintenanceApply?: Date;
  // FK
  // GroupMis
  GroupMIS?: string;
  // Employee
  RequireEmp?: string;
  // Item
  ItemId?: number;
  // Branch
  BranchId?: number;
  // ProjectCodeMaster
  ProjectCodeMasterId?: number;
  // ViewModel
  ItemCode?: string;
  RequireEmpString?: string;
  ProjectCodeMasterString?: string;
  GroupMISString?: string;
  BranchString?: string;
  RequireStatusString?: string;
}

export enum RequireStatus {
  Waiting = 1,
  InProcess,
  Complate,
  Cancel,
  MaintenanceResponse
}
