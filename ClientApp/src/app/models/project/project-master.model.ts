import { BaseModel } from "../basemodel.model";

export interface ProjectMaster extends BaseModel {
  ProjectCodeMasterId: number;
  ProjectCode?: string;
  ProjectName?: string;
  StartDate?: Date;
  EndDate?: Date;
}
