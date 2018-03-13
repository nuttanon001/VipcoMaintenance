// angular
import { Component, Output, EventEmitter, Input } from "@angular/core";
// models
import { Branch } from "../../../models/model.index";
// components
import { BaseViewComponent } from "../../../classes/base-view-component";

@Component({
  selector: 'app-branch-view',
  templateUrl: './branch-view.component.html',
  styleUrls: ["../../../styles/view.style.scss"],
})
export class BranchViewComponent extends BaseViewComponent<Branch> {
  constructor() {
    super();
  }
  // load more data
  onLoadMoreData() {}
}
