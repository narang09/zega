import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { ZegaRoutingModule, RouteComponents } from './modules/zega-routing.module';
import { ZegaComponentsModule } from './modules/zega-components.module';
import { ZegaComponent } from './zega.component';

import { AllowNumbersDirective } from './directives/allowNumbers/allow-numbers.directive';
import { AllowPositiveDecimalsDirective } from './directives/allowPositiveDecimals/allow-positive-decimals.directive';
import { AllowDecimalsDirective } from './directives/allowDecimals/allow-decimals.directive';
import { PercentageDirective } from './directives/percentage/percentage.directive';
import { JsonDatePipe } from './pipes/jssonDate/json-date.pipe';
import { ZegaAPIInterceptor } from './support/interceptors/zegaAPI/zega-api.interceptor';
import { ZegaLoaderInterceptor } from './support/interceptors/zegaLoader/zega-loader.interceptor';

import { LoaderComponent } from './components/loader/loader.component';
import { AuthMessageComponent } from './components/auth-message/auth-message.component';
import { DataGridComponent } from './components/data-grid/data-grid.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { DragDropModule } from '@angular/cdk/drag-drop';

import { ZegaRouteGuard } from './support/guards/zega-route.guard';
import { AuthenticationService } from './services/AuthenticationService/authentication.service';
import { SettingLeftPanelComponent } from './components/setting-left-panel/setting-left-panel.component';
import { EnumsToArrayPipe } from './pipes/EnumsToArray/enums-to-array.pipe';
import { ModelListingSidebarComponent } from './components/sidebars/model-listing-sidebar/model-listing-sidebar.component';
import { AdvancedFilterComponent } from './components/advanced-filter/advanced-filter.component';
import { StrategySidebarComponent } from './components/sidebars/strategy-sidebar/strategy-sidebar.component';
import { SleeveSidebarComponent } from './components/sidebars/sleeve-sidebar/sleeve-sidebar.component';
import { ModelDetailsSidebarComponent } from './components/sidebars/model-details-sidebar/model-details-sidebar.component';
import { BlendModelSidebarComponent } from './components/sidebars/blend-model-sidebar/blend-model-sidebar.component';
import { BackendEnumToArrayPipe } from './pipes/EnumsToArray/backend-enum-to-array.pipe';
import { BulkEditSidebarComponent } from './components/sidebars/bulk-edit-sidebar/bulk-edit-sidebar.component';
import { ConfirmationPopupComponent } from './components/popups/confirmation-popup/confirmation-popup.component';
import { FooterComponent } from './components/footer/footer.component';
import { RepCodeBatchPopupComponent } from './components/popups/rep-code-batch-popup/rep-code-batch-popup.component';
import { RepCodesSidebarComponent } from './components/sidebars/rep-codes-sidebar/rep-codes-sidebar.component';
import { RepCodesListingSidebarComponent } from './components/sidebars/rep-codes-listing-sidebar/rep-codes-listing-sidebar.component';

@NgModule({
  declarations: [
    ZegaComponent,
    RouteComponents,
    AllowNumbersDirective,
    AllowPositiveDecimalsDirective,
    AllowDecimalsDirective,
    PercentageDirective,
    JsonDatePipe,
    EnumsToArrayPipe,
    LoaderComponent,
    AuthMessageComponent,
    DataGridComponent,
    NavMenuComponent,
    SettingLeftPanelComponent,
    ModelListingSidebarComponent,
    AdvancedFilterComponent,
    StrategySidebarComponent,
    SleeveSidebarComponent,
    ModelDetailsSidebarComponent,
    BlendModelSidebarComponent,
    BackendEnumToArrayPipe,
    BulkEditSidebarComponent,
    FooterComponent,
    ConfirmationPopupComponent,
    RepCodesSidebarComponent,
    RepCodeBatchPopupComponent,
    RepCodesListingSidebarComponent,
  ],
  imports: [
    BrowserModule,
    ZegaComponentsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ZegaRoutingModule,
    MatProgressSpinnerModule,
    FormsModule,
    NgbModule,
    MatCardModule,
    MatChipsModule,
    MatExpansionModule,
    MatCheckboxModule,
    DragDropModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ZegaAPIInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ZegaLoaderInterceptor,
      multi: true
    },
    ZegaRouteGuard,
    AuthenticationService,
  ],
  bootstrap: [ZegaComponent]
})
export class ZegaBaseModule { }
