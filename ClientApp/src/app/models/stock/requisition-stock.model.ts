import { BaseModel } from "../basemodel.model";

export interface RequisitionStock extends BaseModel {
  RequisitionStockSpId: number;
  Remark?: string;
  // Fk
  // Employee
  RequisitionEmp?: string;
  // SparePart
  SparePartId?: number;
  // ItemMaintenance
  ItemMaintenanceId?: number;
  // MovementStockSp
  MovementStockSpId?: number;
}
