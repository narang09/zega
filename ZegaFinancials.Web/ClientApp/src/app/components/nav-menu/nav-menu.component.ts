import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginUser } from '../../models/login/login-user';
import { AuthenticationService } from '../../services/AuthenticationService/authentication.service';
import { Constants } from '../../support/constants/constants';
import { NavItems } from '../../support/navHeaders/nav-items';

@Component({
  selector: 'zega-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.less']
})
export class NavMenuComponent implements OnInit {

  menuList: Array<any>

  constructor(private authService: AuthenticationService, private router: Router) {
    this.menuList = [];
  }

  ngOnInit(): void {
    let loggedInUser: LoginUser = this.authService.getLoggedInUserInfo;
    if (loggedInUser.loginId)
      this.menuList = NavItems.getPermittedModules(loggedInUser.isAdmin);
  }

  logOutUser() {
    this.authService.logout()
    .subscribe((response: any) => {
      if (response['success'] === Constants.SuccessResponse) {
        this.authService.logoutUserUI();
        this.router.navigate(['/login']);
      }
    });
  }

}
