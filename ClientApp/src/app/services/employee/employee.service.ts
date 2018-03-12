import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
// service
import { HttpErrorHandler } from "../base/http-error-handler.service";
// models
import { Employee } from "../../models/model.index";
// component
import { BaseRestService } from "../base/base-rest.service";
// rxjs
import { Observable } from "rxjs/Observable";

@Injectable()
export class EmployeeService extends BaseRestService<Employee> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http, "http://192.168.2.33/api/Employee/",
      "EmployeeService",httpErrorHandler
    )
  }
}
