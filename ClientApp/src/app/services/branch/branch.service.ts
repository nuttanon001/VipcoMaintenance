import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
// services
import { HttpErrorHandler } from "../base/http-error-handler.service";
// models
import { Branch } from "../../models/model.index";
// Base-Services
import { BaseRestService } from "../base/base-rest.service";
import { BaseCommunicateService } from "../base/base-communicate.service";
// rxjs
import { Observable } from "rxjs/Observable";
import { catchError } from "rxjs/operators";

@Injectable()
export class BranchService extends BaseRestService<Branch> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http, "api/Branch/",
      "BranchService", httpErrorHandler
    )
  }
}

export class BranchCommunicateService extends BaseCommunicateService<Branch> {
  constructor() { super(); }
}
