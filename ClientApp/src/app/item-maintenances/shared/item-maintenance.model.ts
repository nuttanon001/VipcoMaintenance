import { BaseModel } from "../../shared/base-model.model";

export interface ItemMaintenance extends BaseModel {
  ItemMaintenanceId: number;
  ItemMaintenanceNo?: string;
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
  //ViewModel
  ItemCode?: string;
  MaintenanceEmpString?: string;
  TypeMaintenanceString?: string;
  StatusMaintenanceString?: string;
}

export enum StatusMaintenance {
  TakeAction = 1,
  InProcess,
  WaitSpare,
  OutSouce,
  Complate,
  Cancel
}
