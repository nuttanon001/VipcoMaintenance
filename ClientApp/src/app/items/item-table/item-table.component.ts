// Angular Core
import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { SelectionModel } from '@angular/cdk/collections';
// Components
import { CustomMatTableComponent } from "../../shared/custom-mat-table/custom-mat-table.component";
// Rxjs
import { map } from "rxjs/operators/map";
import { Observable } from "rxjs/Observable";
import { merge } from "rxjs/observable/merge";
import { startWith } from "rxjs/operators/startWith";
import { switchMap } from "rxjs/operators/switchMap";
import { catchError } from "rxjs/operators/catchError";
import { of as observableOf } from "rxjs/observable/of";
// Models
import { Item } from "../shared/item.model";
// Services
import { ItemService } from "../shared/item.service";
import { AuthService } from "../../core/auth/auth.service";
import { ItemTypeService } from "../../item-types/shared/item-type.service";
import { Scroll } from "../../shared/scroll.model";
import { MatSelect } from "@angular/material";
import { ItemType } from "../../item-types/shared/item-type.model";

@Component({
  selector: 'app-item-table',
  templateUrl: './item-table.component.html',
  styleUrls: ["../../shared/custom-mat-table/custom-mat-table.component.scss"]
})
export class ItemTableComponent extends CustomMatTableComponent<Item, ItemService>{
  // Constructor
  constructor(
    service: ItemService,
    authService: AuthService,
    private serviceItemType:ItemTypeService
  ) {
    super(service, authService);
    this.displayedColumns = ["select", "ItemCode", "Name", "ItemTypeString","GroupMisString"];
  }
  // Parameter
  @ViewChild(MatSelect) selectItemType: MatSelect;
  @Input() isDialog: boolean = false;

  itemTypes: Array<ItemType>;
  // Override
  ngOnInit(): void {
    // Get ItemType
    if (!this.itemTypes) {
      this.itemTypes = new Array;
    }
    this.serviceItemType.getAll()
      .subscribe(dbItemType => {
        this.itemTypes = [...dbItemType];
      });

    // If the user changes the sort order, reset back to the first page.
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    // Merge
    merge(this.sort.sortChange, this.paginator.page,
      this.searchBox.search, this.searchBox.onlyCreate,
      this.selectItemType.selectionChange)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          let scroll: Scroll = {
            Skip: this.paginator.pageIndex * this.paginator.pageSize,
            Take: this.paginator.pageSize,
            Filter: this.searchBox.search2,
            SortField: this.sort.active,
            SortOrder: this.sort.direction === "desc" ? 1 : -1,
            WhereId: this.selectItemType.value || -1,
            Where: this.searchBox.onlyCreate2 ? this.authService.getAuth.UserName || "" : ""
          };
          return this.service.getAllWithScroll(scroll);
        }),
        map(data => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.isRateLimitReached = false;
          this.resultsLength = data.Scroll.TotalRow;
          return data.Data;
        }),
        catchError(() => {
          this.isLoadingResults = false;
          // Catch if the GitHub API has reached its rate limit. Return empty data.
          this.isRateLimitReached = true;
          return observableOf([]);
        })
      ).subscribe(data => this.dataSource.data = data);
    // Selection
    this.selection = new SelectionModel<Item>(this.isMultiple, [], true)
    this.selection.onChange.subscribe(selected => {
      if (this.isMultiple) {
        if (selected.source) {
          // this.selectedRow = selected.source.selected[0];
          this.returnSelectesd.emit(selected.source.selected);
        }
      } else {
        if (selected.source.selected[0]) {
          this.selectedRow = selected.source.selected[0];
          this.returnSelected.emit(selected.source.selected[0]);
        }
      }
    });

   
  }
}
