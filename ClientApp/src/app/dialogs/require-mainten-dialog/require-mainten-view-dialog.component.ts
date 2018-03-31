import { Component } from "@angular/core";
// Components
import { RequireMaintenViewComponent } from "../../require-maintenances/require-mainten-view/require-mainten-view.component";

@Component({
  selector: "app-require-mainten-view-dialog",
  templateUrl: "../../require-maintenances/require-mainten-view/require-mainten-view.component.html",
  styleUrls: ["../../require-maintenances/require-mainten-view/require-mainten-view.component.scss"]
})
export class RequireMaintenViewDialogComponent extends RequireMaintenViewComponent {
  constructor() {
    super();
  }
}
