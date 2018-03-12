// angular core
import {
  OnInit, OnDestroy,
  ElementRef, ViewChild, ViewContainerRef
} from "@angular/core";
// rxjs
import "rxjs/Rx";
import { Observable } from "rxjs/Observable";
import { Subscription } from "rxjs/Subscription";
// classes
import { Scroll } from "../models/model.index";
// services
import { DialogsService } from "../services/dialog/dialogs.service";
import { DataTableServiceCommunicate } from "../services/base/data-table.service";

export abstract class BaseMasterComponent<Model, Service>
  implements OnInit, OnDestroy {
  /*
   * constructor
   */
  constructor(
    protected service: Service,
    protected serviceCom: any,
    protected serviceDataTableCom: DataTableServiceCommunicate<Model>,
    protected serviceDialogs: DialogsService,
    protected viewContainerRef: ViewContainerRef,
  ) { }
  /*
   * Parameter
   */
  displayValue: Model | undefined;
  editValue: any;
  columns: any;
  subscription1: Subscription;
  subscription2: Subscription;
  // boolean event
  _showEdit: boolean;
  canSave: boolean;
  hideleft: boolean;
  scroll: Scroll;

  /*
   * Property
   */
  get DisplayDataNull(): boolean {
    return this.displayValue === undefined;
  }

  get ShowEdit(): boolean {
    if (this._showEdit) {
      return this._showEdit;
    } else {
      return false;
    }
  }
  set ShowEdit(showEdit: boolean) {
    if (showEdit !== this._showEdit) {
      this.hideleft = !showEdit;
      this._showEdit = showEdit;
    }
  }

  /*
   * Methods
   */
  // angular hook
  ngOnInit(): void {
    this.ShowEdit = false;
    this.canSave = false;

    this.subscription1 = this.serviceCom.ToParent$.subscribe(
      (TypeValue: [Model, boolean]) => {
        this.editValue = TypeValue[0];
        this.canSave = TypeValue[1];
      });

    this.subscription2 = this.serviceDataTableCom.ToParent$
      .subscribe((scroll: Scroll) => this.loadPagedData(scroll));
  }
  // angular hook
  ngOnDestroy(): void {
    if (this.subscription1) {
      this.subscription1.unsubscribe;
    }

    if (this.subscription2) {
      this.subscription2.unsubscribe;
    }
  }
  // on detail edit
  onDetailEdit(editValue?: Model): void {
    this.displayValue = editValue;
    this.ShowEdit = true;
    setTimeout(() => this.serviceCom.toChildEdit(this.displayValue), 1000);
  }
  // on cancel edit
  onCancelEdit(): void {
    this.editValue = undefined;
    this.displayValue = undefined;
    this.canSave = false;
    this.ShowEdit = false;
    this.onDetailView(undefined);
  }
  // on submit
  onSubmit(): void {
    this.canSave = false;
    if (this.editValue.Creator) {
      this.onUpdateToDataBase(this.editValue);
    } else {
      this.onInsertToDataBase(this.editValue);
    }
  }
  // on save complete
  onSaveComplete(): void {
    this.serviceDialogs.context("System message", "Save completed.", this.viewContainerRef)
      .subscribe(result => {
        this.canSave = false;
        this.ShowEdit = false;
        this.editValue = undefined;
        this.onDetailView(undefined);
        setTimeout(() => {
          this.serviceDataTableCom.toReload(true);
        }, 150);
      });
  }
  // on detail view
  abstract onDetailView(value: any): void;
  // on get all with scroll
  abstract loadPagedData(scroll: Scroll): void;
  // change data to timezone
  abstract changeTimezone(value: Model): Model;
  // on insert data
  abstract onInsertToDataBase(value: Model): void;
  // on update data
  abstract onUpdateToDataBase(value: Model): void;
}