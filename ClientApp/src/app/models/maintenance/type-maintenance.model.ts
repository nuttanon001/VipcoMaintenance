import { BaseModel } from "../basemodel.model";

export interface TypeMaintenance extends BaseModel {
  TypeMaintenanceId: number;
  Name?: string;
  Description?: string;
}
