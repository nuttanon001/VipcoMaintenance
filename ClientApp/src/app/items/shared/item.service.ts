import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
// services
import { HttpErrorHandler } from "../../shared/http-error-handler.service";
// models
import { Item } from "../../items/shared/item.model";
// Base-Services
import { BaseRestService } from "../../shared/base-rest.service";
import { BaseCommunicateService } from "../../shared/base-communicate.service";
// rxjs
import { Observable } from "rxjs/Observable";
import { catchError } from "rxjs/operators";

@Injectable()
export class ItemService extends BaseRestService<Item> {
  constructor(
    http: HttpClient,
    httpErrorHandler: HttpErrorHandler
  ) {
    super(
      http,
      "api/Item/",
      "ItemService",
      "ItemId",
      httpErrorHandler
    )
  }
}

@Injectable()
export class ItemCommunicateService extends BaseCommunicateService<Item> {
  constructor() { super(); }
}
