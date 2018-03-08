import { Component,OnInit } from "@angular/core";
// Model
import { User } from "../../models/model.index";
// Services
import { UserService } from "../../services/user/user.service";
@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  providers: [UserService]
})
export class HomeComponent implements OnInit {
  constructor(
    private userService:UserService
  ) { }

  /**
   * Parameter
   */
  allUsers: Array<User>;

  /**
   * On angular core Init
   */
  ngOnInit(): void {
    if (!this.allUsers) {
      this.allUsers = new Array;
    }
    this.userService.getAll().subscribe(dbData => {
      if (dbData) {
        this.allUsers = dbData.slice();
      }
    })
  }
}
