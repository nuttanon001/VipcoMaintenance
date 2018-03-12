import { BaseModel } from "../basemodel.model";

export interface Item extends BaseModel {
  ItemId: number;
  ItemCode?: string;
  Name?: string;
  Description?: string;
  Model?: string;
  Brand?: string;
  Property?: string;
  Property2?: string;
  Property3?: string;
  ItemStatus?: ItemStatus;
  ItemImage?: string;
  // Fk
  // ItemType
  ItemTypeId?: number;
  // Employee
  EmpResponsible?: string;
  // Branch
  BranchId?: number;
}

enum ItemStatus {
  Use = 1,
  Repair,
  Cancel,
}
