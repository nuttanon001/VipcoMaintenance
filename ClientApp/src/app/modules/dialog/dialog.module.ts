// angular core
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
// 3rd party
import "rxjs/Rx";
import "hammerjs";
// services
import { DialogsService } from "../../services/dialog/dialogs.service";
// components
import {
  ConfirmDialog, ContextDialog,
  ErrorDialog,
} from "../../components/dialog/dialog.index";
// modules
import { CustomMaterialModule } from "../customer-material/customer-material.module";

@NgModule({
  imports: [
    // angular
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    // customer Module
    CustomMaterialModule,
  ],
  exports: [
    ErrorDialog,
    ConfirmDialog,
    ContextDialog,
  ],
  declarations: [
    ErrorDialog,
    ConfirmDialog,
    ContextDialog,
  ],
  providers: [
    DialogsService,
  ],
  // a list of components that are not referenced in a reachable component template.
  // doc url is :https://angular.io/guide/ngmodule-faq
  entryComponents: [
    ErrorDialog,
    ConfirmDialog,
    ContextDialog,
  ],
})
export class DialogsModule { }
