import { BaseModel } from "../basemodel.model";

export interface MovementStock extends BaseModel {
  MovementStockSpId: number;
  MovementDate ?: Date;
  Quantity?: number;
  MovementStatus?: MovementStatus;
}

enum MovementStatus {
  ReceiveStock = 1,
  RequisitionStock,
  AdjustIncrement,
  AdjustDecrement,
  Cancel
}
