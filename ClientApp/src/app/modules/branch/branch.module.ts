// AngulerCore
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common"
import { ReactiveFormsModule } from "@angular/forms";
// Modules
import { CustomMaterialModule } from "../customer-material/customer-material.module";
import { BranchRoutingModule } from "./branch-routing.module";
// Services
import { BranchService,BranchCommunicateService } from '../../services/branch/branch.service';
// Components
import { BranchCenterComponent } from "../../components/branch/branch-center.component";
import { BranchMasterComponent } from '../../components/branch/branch-master/branch-master.component';
import { BranchViewComponent } from '../../components/branch/branch-view/branch-view.component';
import { BranchEditComponent } from '../../components/branch/branch-edit/branch-edit.component';

@NgModule({
  imports: [
    CommonModule,
    BranchRoutingModule,
    CustomMaterialModule,
    ReactiveFormsModule
  ],
  declarations: [
    BranchMasterComponent,
    BranchViewComponent,
    BranchEditComponent,
    BranchCenterComponent
  ],
  providers: [
    BranchService,
    BranchCommunicateService
  ]
})
export class BranchModule { }
