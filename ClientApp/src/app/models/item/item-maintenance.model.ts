import { BaseModel } from "../basemodel.model";

export interface ItemMaintenance extends BaseModel {
  ItemMaintenanceId: number;
  ItemMaintenanceCode?: string;
  PlanStartDate: Date;
  PlanEndDate: Date;
  ActualStartDate?: Date;
  ActualEndDate?: Date;
  StatusMaintenance?: StatusMaintenance;
  Description?: string;
  Remark?: string;
  // FK
  // Employee
  MaintenanceEmp?: string;
  // RequireMaintenance
  RequireMaintenanceId?: number;
  // TypeMaintenance
  TypeMaintenanceId?: number;
}

enum StatusMaintenance {
  TakeAction = 1,
  InProcess,
  WaitSpare,
  OutSouce,
  Complate,
  Cancel
}
