import { BaseModel } from "../basemodel.model";

export interface Branch extends BaseModel {
  BranchId: number;
  Name?: string;
  Address?: string;
}
