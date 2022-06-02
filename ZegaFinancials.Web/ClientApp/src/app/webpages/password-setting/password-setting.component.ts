import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ZegaValidators } from '../../support/validators/validators';
import { Constants } from '../../support/constants/constants';
import { UserService } from '../../services/User/user.service';
import { UserSettings } from '../../models/UserSettings/user-settings';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';

@Component({
  selector: 'zega-password-setting',
  templateUrl: './password-setting.component.html',
  styleUrls: ['./password-setting.component.less']
})
export class PasswordSettingComponent implements OnInit {
  hideC = true;
  hideN = true;
  hideR = true;
  passwordSettingForm: FormGroup;
  isPasswordChanged: boolean = true;
  ngOnInit(): void { }

  constructor(private fb: FormBuilder, private userService: UserService, private router: Router, private messageService: AuthMessageService) {
    this.passwordSettingForm = this.fb.group({
      CurrentPassword: new FormControl(null, [Validators.required]),
      Password: new FormControl(null, [Validators.required, Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{7,}')]),
      ReNewPassword: new FormControl(null, [Validators.required]),
    }, {
        validator: [ZegaValidators.notMatched('CurrentPassword', 'Password'),
          ZegaValidators.matchConfirmPassword('Password', 'ReNewPassword'),
        
        ]
    });
  }

  submitPasswordSettingForm() {
    if (this.passwordSettingForm.valid) {
      let passwordSettingModel: UserSettings = Object.assign({}, this.passwordSettingForm.value);
      passwordSettingModel.isPasswordChanged = this.isPasswordChanged;
      this.userService.passwordSettings(passwordSettingModel)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.navigation();
          }
        });
    }
  }

  navigation() {
    this.router.navigate(['/dashboard']);
  }
}
