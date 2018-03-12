import { BaseModel } from "../basemodel.model";

export interface ProjectSub extends BaseModel {
  ProjectCodeDetailId: number;
  ProjectCodeDetailCode?: string;
  Description?: string;
  ProjectCodeMasterId?: number;
  //ViewModel
  FullProjectLevelString?: string;
}
