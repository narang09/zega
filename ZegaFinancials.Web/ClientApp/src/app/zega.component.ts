import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/internal/operators/filter';
import { NavItems } from './support/navHeaders/nav-items';
import { UserService } from './services/User/user.service';
import { Constants } from './support/constants/constants';
import { AuthenticationService } from './services/AuthenticationService/authentication.service';

@Component({
  selector: 'zega-root',
  templateUrl: './zega.component.html',
  styleUrls: ['./zega.component.less']
})
export class ZegaComponent implements OnInit {
  pageTitle: string = '';
  isLoginView: boolean = true;
  userName: string = '';
  imageBase64URL: string = '';
  loadUserImage: boolean = false;

  constructor(private router: Router, private userService: UserService, private authService: AuthenticationService) {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        var urls = event.urlAfterRedirects.split('/');
        urls.splice(0, 1);
        if (urls.length) {
          this.isLoginView = this.checkForLoginPage(urls[0]);
          this.pageTitle = NavItems.getBreadcrumbTitle(urls);
          if (!this.isLoginView) {
            this.getUserSettingsData();
            this.getUserProfileImage();
          } else {
            this.imageBase64URL = '';
            this.loadUserImage = false;
          }
        }
      });


  }
  ngOnInit(): void {
  }

  private checkForLoginPage(url: string) {
    if (url === 'login') {
      return true;
    }
    else {
      let urlparts = url.split('?');
      if (urlparts[0])
        return urlparts[0] === 'resetpassword'
      return false;
    }
  }

  private getUserSettingsData() {
    this.userService.getUserSettingsData()
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          var retData = response['response'];
          this.userName = retData.firstName;
        }
      });
  }

  private getUserProfileImage() {
    if (!this.imageBase64URL) {
      this.userService.getUserImage().
        subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            var retData = response['response'];
            this.imageBase64URL = retData.imageBytes;
            this.loadUserImage = true;
          }
        });
    }
  }

  navigationToSettings() {
    this.router.navigate(['/settingpanel']);
  }
}
