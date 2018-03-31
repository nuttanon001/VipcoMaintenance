import { Injectable } from "@angular/core";
import { HttpClient, HttpParams, HttpHeaders } from "@angular/common/http";
// services
import { HttpErrorHandler } from "../../shared/http-error-handler.service";
// models
import { ItemMaintenance } from "../shared/item-maintenance.model";
// Base-Services
import { BaseRestService } from "../../shared/base-rest.service";
import { BaseCommunicateService } from "../../shared/base-communicate.service";
// rxjs
import { Observable } from "rxjs/Observable";
import { catchError } from "rxjs/operators";

@Injectable()
export class ItemMaintenService extends BaseRestService<ItemMaintenance> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/ItemMaintenance/",
      "ItemMaintenService",
      "ItemMaintenanceId",
      httpErrorHandler
    )
  }
}

@Injectable()
export class ItemMaintenCommunicateService extends BaseCommunicateService<ItemMaintenance> {
  constructor() { super(); }
}
