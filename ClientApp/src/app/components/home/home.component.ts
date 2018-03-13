import { Component,OnInit } from "@angular/core";
// Model
import { User } from "../../models/model.index";
// Service
import { UserService } from "../../services/user/user.service";
@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  providers: [UserService]
})
export class HomeComponent implements OnInit {
  constructor(
    private service:UserService
  ) { }

  /**
   * Parameter
   */
  /**
   * On angular core Init
   */
  ngOnInit(): void {
  }
}
