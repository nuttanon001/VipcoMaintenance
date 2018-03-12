import { BaseModel } from "../basemodel.model";

export interface ReceiveStock extends BaseModel {
  ReceiveStockSpId: number;
  PurchaseOrder?: string;
  Remark?: string;
  // FK
  // Employee
  ReceiveEmp?: string;
  // SparePart
  SparePartId?: number;
  // MovementStockSp
  MovementStockSpId?: number;
}
