// core modules
import {
  HttpClient, HttpHeaders,
  HttpParams
} from "@angular/common/http";
// rxjs
import { Observable } from "rxjs/Observable";
import { of } from "rxjs/observable/of";
import { catchError, retry, tap } from "rxjs/operators";
// models
import { Scroll, ScrollData } from "../../models/model.index";
// services
import { HttpErrorHandler, HandleError } from "./http-error-handler.service";

const httpOptions = {
  headers: new HttpHeaders ({
    "Content-Type" : "application/json",
    // "Authorization": "my-auth-token"
  })
};

export abstract class BaseRestService<Model>{
  constructor(
    protected http: HttpClient,
    protected baseUrl: string,
    protected serviceName: string,
    protected httpErrorHandler: HttpErrorHandler
  ) {
    this.handleError = httpErrorHandler.createHandleError(serviceName);
  }
  /* Parameter */
  public handleError: HandleError;

  //===================== Privete Members =======================\\
  // extract data
  public extractData(r: Response) { // for extractdata
    let body = r.json();
    // console.log(body);
    return body || {};
  }
  // extract data for result code
  private extractResultCode(res: Response) {
    if (res) {
      if (res.status === 201) {
        return [{ status: res.status, json: res }]
      }
      else if (res.status === 200) {
        return [{ status: res.status, json: res }]
      }
    }
  }
  // set log
  private log(filename: string, data: string) {
    const message = `DownloaderService downloaded "${filename}" and got "${data}".`;
    console.log(message);
  }
  // set log error
  private logError(filename: string, error: any) {
    const message = `DownloaderService failed to download "${filename}"; got error "${error.message}".`;
    console.error(message);
  }

  //===================== HTTP-Rest =============================\\
  /** GET Models from the server */
  getAll(): Observable<Array<Model>> {
    return this.http.get<Array<Model>>(this.baseUrl)
      .pipe(catchError(this.handleError(this.serviceName + "/get all model.", new Array<Model>())));
  }
  /** get one with key number */
  getOneKeyNumber(key: number): Observable<Model> {
    // Add safe, URL encoded search parameter if there is a search term
    const options = key ? { params: new HttpParams().set("key", key.toString()) } : {};

    return this.http.get<Model>(this.baseUrl, options)
      .pipe(catchError(this.handleError(this.serviceName + "/get one model", <Model>{})));
  }
  /** get one with key string */
  getOneKeyString(key: string): Observable<Model> {
    // Add safe, URL encoded search parameter if there is a search term
    const options = key ? { params: new HttpParams().set("key", key) } : {};

    return this.http.get<Model>(this.baseUrl, options)
      .pipe(catchError(this.handleError(this.serviceName + "/get one model", <Model>{})));
  }

  /** get auto complate */
  getAutoComplate(): Observable<Array<string>> {
    let url: string = `${this.baseUrl}GetAutoComplate/`;
    return this.http.get<Array<string>>(url)
      .pipe(catchError(this.handleError(this.serviceName + "/get auto complate", new Array<string>())));
  }

  /** get by master id */
  getByMasterId(masterId: number, subAction: string = "GetByMaster/"): Observable<Array<Model>> {
    // Add safe, URL encoded search parameter if there is a search term
    const options = masterId ? { params: new HttpParams().set('key', masterId.toString()) } : {};

    let url: string = this.baseUrl + subAction;
    return this.http.get<Array<Model>>(url, options)
      .pipe(catchError(this.handleError(this.serviceName + "/get by master", new Array<Model>())));
  }

  /** get by master id */
  getByMasterCode(masterCode: string, subAction: string = "GetByMaster/"): Observable<Array<Model>> {
    // Add safe, URL encoded search parameter if there is a search term
    const options = masterCode ? { params: new HttpParams().set("key", masterCode) } : {};

    let url: string = this.baseUrl + subAction;
    return this.http.get<Array<Model>>(url)
      .pipe(catchError(this.handleError(this.serviceName + "/get by master", new Array<Model>())));
  }

  /** get all with scroll data */
  getAllWithScroll(scroll: Scroll, subAction: string = "GetScroll/"): Observable<ScrollData<Model>> {
    return this.http.post<ScrollData<Model>>(this.baseUrl + subAction, JSON.stringify(scroll), httpOptions)
      .pipe(catchError(this.handleError(this.serviceName + "/get scroll", <ScrollData<Model>>{})));
  }

  // get all with page
  //getAllWithPage(page: Page, subAction: string = "GetPage/"): Observable<PageData<Model>> {
  //    return this.http.post(this.baseUrl + subAction, JSON.stringify(page), this.getRequestOption())
  //        .map(this.extractData).catch(this.handleError);
  //}

  /** add Model @param nObject */
  addModel(nObject: Model): Observable<Model> {
    return this.http.post<Model>(this.baseUrl, JSON.stringify(nObject), httpOptions)
      .pipe(catchError(this.handleError(this.serviceName + "/post model", nObject)));
  }

  /** update with key number */
  updateModelWithKey(uObject: Model, key: number): Observable<Model> {
    return this.http.put<Model>(this.baseUrl + key + "/", JSON.stringify(uObject), {
      headers: httpOptions.headers,
      params: new HttpParams().set("key", key.toString())
    }).pipe(catchError(this.handleError(this.serviceName + "/put update model", uObject)));
  }

  /** update with key string */
  updateModelWithString(uObject: Model, key: string): Observable<Model> {
    return this.http.put<Model>(this.baseUrl + key + "/", JSON.stringify(uObject), {
      headers: httpOptions.headers,
      params: new HttpParams().set("key", key)
    }).pipe(catchError(this.handleError(this.serviceName + "/put update model", uObject)));
  }

  /** put update model */
  updateModel(uObject: Model): Observable<Model> {
    return this.http.put<Model>(this.baseUrl, JSON.stringify(uObject), httpOptions)
      .pipe(catchError(this.handleError(this.serviceName + "/put update model", uObject)));
  }

  /** delete with key number*/
  deleteKeyNumber(key: number): Observable<any> {
    const url = `${this.baseUrl}/${key}`; // DELETE api/heroes/42
    return this.http.delete(url, httpOptions)
      .pipe(catchError(this.handleError(this.serviceName + "/delete model")));
  }

  getTextFile(filename: string) {
    // The Observable returned by get() is of type Observable<string>
    // because a text response was specified.
    // There's no need to pass a <string> type parameter to get().
    return this.http.get(filename, { responseType: "text" })
      .pipe(
        tap( // Log the result or error
          data => this.log(filename, data),
          error => this.logError(filename, error)
      ));
  }
}
