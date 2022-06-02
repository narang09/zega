import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from '../webpages/login/login.component';
import { DashboardComponent } from '../webpages/dashboard/dashboard.component';
import { StrategyListingComponent } from '../webpages/strategy-listing/strategy-listing.component';
import { AccountListingComponent } from '../webpages/account-listing/account-listing.component';
import { ModelListingComponent } from '../webpages/model-listing/model-listing.component';
import { SleeveListingComponent } from '../webpages/sleeve-listing/sleeve-listing.component';
import { UserListingComponent } from '../webpages/user-listing/user-listing.component';
import { UserDetailsComponent } from '../webpages/user-details/user-details.component';
import { ImportHistoryComponent } from '../webpages/import-history/import-history.component';
import { ImportProfilesComponent } from '../webpages/import-profiles/import-profiles.component';
import { AuditLogListingComponent } from '../webpages/audit-log-listing/audit-log-listing.component';
import { PageNotFoundComponent } from '../webpages/page-not-found/page-not-found.component';
import { AddAccountComponent } from '../webpages/add-account/add-account.component';
import { SettingLeftPanelComponent } from '../components/setting-left-panel/setting-left-panel.component';
import { AccountSettingComponent } from '../webpages/account-setting/account-setting.component';
import { PasswordSettingComponent } from '../webpages/password-setting/password-setting.component';
import { ZegaRouteGuard } from '../support/guards/zega-route.guard';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  {
    path: 'settingpanel', component: SettingLeftPanelComponent},
  { path: 'resetpassword', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [ZegaRouteGuard] },
  {
    path: 'accounts', canActivate: [ZegaRouteGuard], children: [
      { path: '', component: AccountListingComponent, canActivateChild: [ZegaRouteGuard] },
      { path: ':accountId', component: AddAccountComponent, canActivateChild: [ZegaRouteGuard] },
    ]
  },
  { path: 'strategies', component: StrategyListingComponent, canActivate: [ZegaRouteGuard] },
  { path: 'models', component: ModelListingComponent, canActivate: [ZegaRouteGuard] },
  { path: 'sleeves', component: SleeveListingComponent, canActivate: [ZegaRouteGuard] },
  {
    path: 'users', canActivate: [ZegaRouteGuard], children: [
      { path: '', component: UserListingComponent, canActivateChild: [ZegaRouteGuard] },
      { path: ':userId', component: UserDetailsComponent, canActivateChild: [ZegaRouteGuard] },
    ]
  },
  { path: 'import', component: ImportProfilesComponent, canActivate: [ZegaRouteGuard] },
  { path: 'importhistory', component: ImportHistoryComponent, canActivate: [ZegaRouteGuard] },
  { path: 'audit', component: AuditLogListingComponent, canActivate: [ZegaRouteGuard] },
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class ZegaRoutingModule { }

export const RouteComponents = [
  LoginComponent,
  DashboardComponent,
  StrategyListingComponent,
  AccountListingComponent,
  ModelListingComponent,
  SleeveListingComponent,
  ImportHistoryComponent,
  ImportProfilesComponent,
  AuditLogListingComponent,
  UserListingComponent,
  PageNotFoundComponent,
  AddAccountComponent,
  AccountSettingComponent,
  PasswordSettingComponent,
  UserDetailsComponent,
  PageNotFoundComponent,
]
