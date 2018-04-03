import { Injectable } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders } from "@angular/common/http";
// services
import { HttpErrorHandler } from "../../shared/http-error-handler.service";
// models
import { RequireMaintenance } from "./require-maintenance.model";
import { OptionRequireMaintenance } from "./option-require-maintenance.model";
// Base-Services
import { BaseRestService } from "../../shared/base-rest.service";
import { BaseCommunicateService } from "../../shared/base-communicate.service";
// rxjs
import { Observable } from "rxjs/Observable";
import { catchError } from "rxjs/operators";

const httpOptions = {
  headers: new HttpHeaders({
    "Content-Type": "application/json",
    // "Authorization": "my-auth-token"
  })
};

@Injectable()
export class RequireMaintenService extends BaseRestService<RequireMaintenance> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/RequireMaintenance/",
      "RequireMaintenService",
      "RequireMaintenanceId",
      httpErrorHandler
    )
  }

  // ===================== Action Require Maintenance ===========================\\
  // action require maintenance
  actionRequireMaintenance(RequireMaintenaceId: number,ByEmployee:string): Observable<RequireMaintenance> {
    const options = {
      params: new HttpParams().set("key", RequireMaintenaceId.toString()).set("byEmp",ByEmployee)
    };
    return this.http.get<RequireMaintenance>(this.baseUrl + "ActionRequireMaintenance/", options)
      .pipe(catchError(this.handleError(this.serviceName + "/action require maintenance model", <RequireMaintenance>{})));
  }

  // ===================== Require Maintenance Schedule ===========================\\
  // get Require Maintenance Schedule
  getRequireMaintenanceSchedule(option: OptionRequireMaintenance): Observable<any> {
    let url: string = `${this.baseUrl}MaintenanceWaiting/`;
    
    return this.http.post<any>(url, JSON.stringify(option), httpOptions)
      .pipe(catchError(this.handleError(this.serviceName + "/maintenance waiting", <any>{})));
  }
}

@Injectable()
export class RequireMaintenCommunicateService extends BaseCommunicateService<RequireMaintenance> {
  constructor() { super(); }
}
