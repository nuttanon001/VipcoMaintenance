// Angular Core
import { NgModule } from "@angular/core";
import { HttpModule } from "@angular/http";
import { RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// Components
import { AppComponent } from "./components/app/app.component";
import { NavMenuComponent } from "./components/nav-menu/nav-menu.component";
import { HomeComponent } from "./components/home/home.component";
// Modules
import { CustomMaterialModule,DialogsModule } from "./modules/module.index";
// Components
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
// Serices
import { AuthService } from "./services/auth/auth.service";
import { AuthGuard } from "./services/auth/auth-guard.service";
import { MessageService } from "./services/base/message.service";
import { HttpErrorHandler } from "./services/base/http-error-handler.service";

import "hammerjs";
import "popper.js";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    LoginComponent,
    RegisterComponent,
  ],
  imports: [
    // Angular Core
    HttpModule,
    FormsModule,
    CommonModule,
    HttpClientModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    // Modules
    DialogsModule,
    CustomMaterialModule,
    // Router
    RouterModule.forRoot([
      { path: "", redirectTo: "home", pathMatch: "full" },
      { path: "home", component: HomeComponent },
      { path: "login", component: LoginComponent },
      { path: "register/:condition", component: RegisterComponent },
      { path: "register", component: RegisterComponent },
      {
        path: "branch",
        loadChildren: './modules/branch/branch.module#BranchModule'
      },
      { path: "**", redirectTo: "home" },
    ]),
  ],
  providers: [
    AuthGuard,
    AuthService,
    MessageService,
    HttpErrorHandler,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
