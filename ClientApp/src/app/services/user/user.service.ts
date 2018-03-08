import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
// services
import { HttpErrorHandler } from "../base/http-error-handler.service";
// models
import { User } from "../../models/model.index";
// Base-Services
import { BaseRestService } from "../base/base-rest.service";
import { Observable } from "rxjs/Observable";

@Injectable()
export class UserService extends BaseRestService<User> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http, "http://192.168.2.31/machine/api/User/",
      "UserService", httpErrorHandler
    )
  }
}
