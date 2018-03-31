import { Component } from '@angular/core';
// Components
import { RequireMaintenViewComponent } from "../../require-maintenances/require-mainten-view/require-mainten-view.component";

@Component({
  selector: 'app-item-mainten-has-require',
  templateUrl: "../../require-maintenances/require-mainten-view/require-mainten-view.component.html",
  styleUrls: ["../../require-maintenances/require-mainten-view/require-mainten-view.component.scss"]
})
export class ItemMaintenHasRequireComponent extends RequireMaintenViewComponent {
  constructor() {
    super();
  }
}
