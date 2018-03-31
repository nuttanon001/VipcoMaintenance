import { BaseModel } from "../../shared/base-model.model";

export interface SparePart extends BaseModel {
  SparePartId: number;
  Name?: string;
  Description?: string;
  Remark?: string;
  Model?: string;
  Size?: string;
  Property?: string;
  SparePartImage?: string;
  MinStock?: number;
  MaxStock?: number;
  //FK
  // WorkGroup
  WorkGroupId?: number;
  //ViewModel
  OnHand?: number;
}
