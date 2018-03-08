// Angular Core
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { RouterModule } from "@angular/router";
// Components
import { AppComponent } from "./components/app/app.component";
import { NavMenuComponent } from "./components/nav-menu/nav-menu.component";
import { HomeComponent } from "./components/home/home.component";
// Modules
import { CustomMaterialModule } from "./modules/module.index";
// Serices
import { HttpErrorHandler } from "./services/base/http-error-handler.service";
import { MessageService } from "./services/base/message.service";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
  ],
  imports: [
    // Angular Core
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    // Modules
    CustomMaterialModule,
    // Router
    RouterModule.forRoot([
      { path: "", component: HomeComponent, pathMatch: "full" },
    ])
  ],
  providers: [
    HttpErrorHandler,
    MessageService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
