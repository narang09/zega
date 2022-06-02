import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';

import { Login } from '../../models/login/login';
import { AuthenticationService } from '../../services/AuthenticationService/authentication.service';
import { Constants } from '../../support/constants/constants';
import { ZegaValidators } from '../../support/validators/validators';
import { LoginUser } from '../../models/login/login-user';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { ConfirmationPopupComponent } from '../../components/popups/confirmation-popup/confirmation-popup.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'zega-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less']
})
export class LoginComponent implements OnInit {
  hideC = true;
  hideN = true;
  hideR = true;
  ngOnInit() {
    var urlSegs = this.route.snapshot.url;
    if (urlSegs.length && urlSegs[0].path === 'resetpassword') {
      this.route.queryParams.subscribe(params => {
        this.authTokenResetPass = params['token'];
        this.loginResetPass = params['login'];
        if (this.loginResetPass && this.authTokenResetPass)
          this.activeForm = "ResetPassword";
        else
          this.router.navigate(['/login']);
      });
    } else
      this.activeForm = "Login";
  }

  private authTokenResetPass: string = '';
  private loginResetPass: string = '';
  activeForm: string = '';
  loginForm: FormGroup;
  forgotForm: FormGroup;
  resetPasswordForm: FormGroup;

  constructor(private fb: FormBuilder, private authenticationService: AuthenticationService, private router: Router, private route: ActivatedRoute, private messageService: AuthMessageService, private matDialog: MatDialog) {

    this.loginForm = this.fb.group({
      Login: new FormControl(null, [Validators.required]),
      Password: new FormControl(null, [Validators.required])
    });

    this.forgotForm = this.fb.group({
      Email: new FormControl(null, [Validators.required, Validators.pattern('[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}')])
    });

    this.resetPasswordForm = this.fb.group({
      Password: new FormControl(null, [Validators.required, Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{7,}')]),
      ConfirmPassword: new FormControl(null, [Validators.required])
    }, {
      validator: ZegaValidators.matchConfirmPassword('Password', 'ConfirmPassword')
    });
  }


  forgetPassword() {
    this.loginForm.reset();
    this.activeForm = "ChangePassword";
  }
  backToLogin() {
    this.forgotForm.reset();
    this.activeForm = "Login";
  }


  submitLoginForm(isForceLogin: boolean) {
    if (this.loginForm.valid) {
      let loginModel: Login = Object.assign({}, this.loginForm.value);
      loginModel.IsForceLogin = isForceLogin;
      this.authenticationService.login(loginModel)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            if (response['responseCode'] === Constants.ForceLoginPopupResponseCode)
              this.openForceLoginDialog(response['message'])
            else
              this.setLoginResponse(response['response']);
          }
        });
    }
  }

  private openForceLoginDialog(message: string) {
    const dialogRef = this.matDialog.open(ConfirmationPopupComponent, {
      data: { Header: "Force Login Confirmation!", Message: message }
    });

    dialogRef.afterClosed().subscribe((isSuccess: any) => {
      if (isSuccess)
        this.submitLoginForm(true);
    });
  }

  submitForgotForm() {
    if (this.forgotForm.valid) {
      let email: string = this.forgotForm.get('Email')?.value;
      this.authenticationService.forgortPassord(JSON.stringify(email))
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.backToLogin();
          }
        });
    }
  }

  submitResetPassword() {
    if (this.resetPasswordForm.valid) {
      let resetPassModel = {
        Password: this.resetPasswordForm.get('Password')?.value,
        Login: this.loginResetPass,
        AuthToken: this.authTokenResetPass
      }
      this.authenticationService.resetPassword(resetPassModel)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.router.navigate(['/login']);
          }
        });
    }
  }

  private setLoginResponse(loginResp: LoginUser) {
    this.authenticationService.setLoggedInUser(true);
    this.authenticationService.setLoggedInUserDetails(loginResp);
    this.router.navigate(['/dashboard']);
  }

}
