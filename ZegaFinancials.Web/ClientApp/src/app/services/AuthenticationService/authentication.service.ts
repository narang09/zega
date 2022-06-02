import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Login } from '../../models/login/login';
import { LoginUser } from '../../models/login/login-user';
import { ApiURLs } from '../../support/apiURLs/api-urls';
import { NavItems } from '../../support/navHeaders/nav-items';
import { Utility } from '../../support/utility/utility';


@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private loggedIn: BehaviorSubject<boolean>;
  private loggedInUser: LoginUser;

  constructor(private http: HttpClient) {
    this.loggedIn = new BehaviorSubject<boolean>(false);
    this.loggedInUser = new LoginUser;
  }

  // Logged in User
  get isUserLoggedIn() {
    return this.loggedIn.asObservable();
  }

  get getLoggedInUserInfo() {
    if (this.loggedInUser === null || !this.loggedInUser.loginId) {
      let plainText = Utility.decompressData(String(localStorage.getItem('ZF')));
      this.loggedInUser = plainText ? JSON.parse(plainText) : null;
    }
    return this.loggedInUser;
  }

  setLoggedInUser(isLoggedIn: boolean) {
    this.loggedIn.next(isLoggedIn);
  }

  setLoggedInUserDetails(info: LoginUser) {
    this.loggedInUser = info;
    localStorage.setItem('ZF', Utility.compressData(JSON.stringify(info)));
  }

  logoutUserUI() {
    this.loggedInUser = new LoginUser;
    this.loggedIn.next(false);
    localStorage.clear();
  }


  checkUserModulePermission(url: string) {
    var user = this.getLoggedInUserInfo;
    return user && user.loginId ? NavItems.checkModulePermissions('/' + url, user.isAdmin) : false;
  }

  // Backend APIs

  login(loginInfo: Login) {
    return this.http.post(ApiURLs.loginAPI, loginInfo)
      .pipe(response => response);
  }

  forgortPassord(email: string) {
    return this.http.post(ApiURLs.forgotPasswordAPI, email)
      .pipe(response => response);
  }

  resetPassword(resetPassModel: any) {
    return this.http.post(ApiURLs.resetPasswordAPI, resetPassModel)
      .pipe(response => response);
  }

  logout() {
    return this.http.post(ApiURLs.logoutAPI, null)
      .pipe(response => response);
  }
}
