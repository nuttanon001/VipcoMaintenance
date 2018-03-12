import { BaseModel } from "../basemodel.model";

export interface Workgroup extends BaseModel {
  WorkGroupId: number;
  Name?: string;
  Description?: string;
  Remark?: string;
}
