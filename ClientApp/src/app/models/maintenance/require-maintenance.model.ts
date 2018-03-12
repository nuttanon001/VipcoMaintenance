import { BaseModel } from "../basemodel.model";

export interface RequireMaintenance extends BaseModel {
  RequireMaintenanceId: number;
  RequireNo?: string;
  RequireDate?: Date;
  Description?: string;
  Remark?: string;
  RequireStatus?: RequireStatus;
  // FK
  // GroupMis
  GroupMIS?: string;
  // Employee
  RequireEmp?: string;
  // Item
  ItemId?: number;
  // Branch
  BranchId?: number;
}

enum RequireStatus {
  Waiting = 1,
  InProcess,
  Complate,
  Cancel
}
