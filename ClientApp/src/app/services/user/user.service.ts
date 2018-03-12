import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
// services
import { HttpErrorHandler } from "../base/http-error-handler.service";
// models
import { User } from "../../models/model.index";
// Base-Services
import { BaseRestService } from "../base/base-rest.service";
// rxjs
import { Observable } from "rxjs/Observable";
import { catchError } from "rxjs/operators";

@Injectable()
export class UserService extends BaseRestService<User> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http, "http://192.168.2.33/api/User/",
      "UserService", httpErrorHandler
    )
  }

  // EmployeeAlready
  // get check employee already username
  getEmployeeAlready(empCode: string): Observable<any> {
    // Add safe, URL encoded search parameter if there is a search term
    const options = empCode ? { params: new HttpParams().set('key', empCode) } : {};

    let url: string = `${this.baseUrl}EmployeeAlready/`;
    return this.http.get<any>(url, options)
      .pipe(catchError(this.handleError(this.serviceName + "/get by employee already", <any>{})));
  }
}
