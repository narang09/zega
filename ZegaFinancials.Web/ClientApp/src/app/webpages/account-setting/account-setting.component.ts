import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Constants } from '../../support/constants/constants';
import { UserSettings } from '../../models/UserSettings/user-settings';
import { UserService } from '../../services/User/user.service';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { UserCountries} from '../../support/enums/user.enum';
import { Utility } from '../../support/utility/utility';

@Component({
  selector: 'zega-account-setting',
  templateUrl: './account-setting.component.html',
  styleUrls: ['./account-setting.component.less']
})
export class AccountSettingComponent implements OnInit {
  countriesDropdown = UserCountries;
  accountSettingForm: FormGroup;
  ngOnInit(): void {
    this.getUserSettingsData();
  }

  constructor(private fb: FormBuilder, private userService: UserService, private router: Router, private messageService: AuthMessageService) {
    this.accountSettingForm = this.fb.group({
      FirstName: new FormControl(null, [Validators.required]),
      LastName: new FormControl(null, [Validators.required]),
      Email: new FormControl(null, [Validators.required, Validators.email]),
      CountryCode: new FormControl(UserCountries.US, [Validators.required]),
      PhoneNo: new FormControl(null, [Validators.required, Validators.pattern('[1-9][0-9]{9}')]),
      Company: new FormControl(null, []),
      Designation: new FormControl(null, []),
    });
  }

  submitAccountSettingForm() {
    Utility.cleanForm(this.accountSettingForm);
    if (this.accountSettingForm.valid) {
      let accountSettingModel: UserSettings = Object.assign({}, this.accountSettingForm.value);
      accountSettingModel.CountryCode = '+' + accountSettingModel.CountryCode;
      this.userService.userSettings(accountSettingModel)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.navigation();
          }
        });
    }
  }

  private getUserSettingsData() {
    this.userService.getUserSettingsData()
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) 
          this.setUserSettingData(response['response']);
      });
  }

  private setUserSettingData(userSetting: any) {
    if (userSetting) {
      this.accountSettingForm.patchValue({
        FirstName: userSetting.firstName,
        LastName: userSetting.lastname,
        Email: userSetting.email,
        CountryCode: parseInt(userSetting.countryCode.startsWith('+') ? userSetting.countryCode.slice(1) : userSetting.countryCode),
        PhoneNo: userSetting.phoneNo,
        Company: userSetting.company,
        Designation: userSetting.designation,
      });
     
    }
  }

  cancel() {
    this.navigation();
  }


  private navigation() {
    this.router.navigate(['/dashboard']);
  }

}
