// Angular Core
import { Component,Input } from "@angular/core";
// Components
import { CustomMatTableComponent } from "../../shared/custom-mat-table/custom-mat-table.component";
// Models
import { Item } from "../shared/item.model";
// Services
import { ItemService } from "../shared/item.service";
import { AuthService } from "../../core/auth/auth.service";

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
  ) {
    super(service, authService);
    this.displayedColumns = ["select", "ItemCode", "Name", "ItemTypeString"];
  }

  @Input() isDialog: boolean = false;
}
