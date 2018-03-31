import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
// service
import { HttpErrorHandler } from "../../shared/http-error-handler.service";
// models
import { EmployeeGroupMis } from "../../employees/shared/employee-group-mis.model";
// component
import { BaseRestService } from "../../shared/base-rest.service";
// rxjs
import { Observable } from "rxjs/Observable";
@Injectable()
export class EmployeeGroupMisService extends BaseRestService<EmployeeGroupMis> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/EmployeeGroupMis/",
      "EmployeeGroupMisService",
      "GroupMIS",
      httpErrorHandler
    )
  }
}
