import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DataGridEditModel } from '../../models/DataGrid/DataGridEditModel/data-grid-edit-model';
import { GridRowSelectionModel } from '../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { UserService } from '../../services/User/user.service';
import { Constants } from '../../support/constants/constants';
import { DataGridNames } from '../../support/enums/data-grid.enum';
import { RepCodeType, UserCountries, UserStatus } from '../../support/enums/user.enum';
import { Utility } from '../../support/utility/utility';

@Component({
  selector: 'zega-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.less'],
})
export class UserDetailsComponent implements OnInit {
  private userId: number = 0;
  isAdmin: boolean = false;
  userDetailsForm: FormGroup;
  statesDropdown = UserStatus;

  countriesDropdown = UserCountries;
  refreshMobileGrid: boolean = false;
  mobileForm: FormGroup;
  mobileGridName: DataGridNames = DataGridNames.UserContactNumbers;
  private userMobiles: Array<any> = []
  public selectedMobileIds: Array<any> = [];
  get PrimaryMobile() {
    let mobile = '';
    if (this.userMobiles.length) {
      let primary = this.userMobiles.find(mob => mob.isPrimary)
      if (primary)
        mobile = primary.countryCode + ' ' + primary.phoneNo;
    }
    return mobile;
  }


  refreshEmailGrid: boolean = false;
  emailForm: FormGroup;
  emailGridName: DataGridNames = DataGridNames.UserEmails;
  private userEmails: Array<any> = []
  public selectedEmailIds: Array<any> = [];
  get PrimaryEmail() {
    let email = '';
    if (this.userEmails.length) {
      let primary = this.userEmails.find(mob => mob.isPrimary)
      if (primary)
        email = primary.email;
    }
    return email;
  }

  userRepCodes: Array<number> = []
  userModels: Array<any> = [];
  showModelListingSidebar: boolean = false;
  showRepCodesListingSidebar: boolean = false;

  constructor(private route: ActivatedRoute, private fb: FormBuilder, private messageService: AuthMessageService, private userService: UserService, private router: Router) {
    this.userDetailsForm = this.fb.group({
      FirstName: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
      LastName: new FormControl(null, [Validators.maxLength(100)]),
      Login: new FormControl(null, [Validators.required, Validators.maxLength(25)]),
      Status: new FormControl(UserStatus.Inactive, [Validators.required]),
    });
    this.mobileForm = this.fb.group({
      Code: new FormControl(UserCountries.US, [Validators.required]),
      Mobile: new FormControl(null, [Validators.required,Validators.pattern('[1-9][0-9]{9}')])
    });
    this.emailForm = this.fb.group({
      Email: new FormControl(null, [Validators.required,Validators.email])
    });
  }

  ngOnInit(): void {
    var encodedParam = this.route.snapshot.paramMap.get('userId')
    var param = Utility.decompressURL(String(encodedParam));
    if (param) {
      this.userId = Number.parseInt(param);
      this.getUserInfo();
    }
  }

  get validateUserDetails() {
    return this.userDetailsForm.valid && this.userEmails.length > 0 && this.userMobiles.length > 0 && (this.isAdmin ? true : this.userRepCodes.length > 0);
  }

  private getUserInfo() {
    if (this.userId) {
      this.userService.getUser(this.userId)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            var retData = response['response'];
            this.setUserData(retData);
          }
        });
    }
  }

  private setUserData(user: any) {
    if (user) {
      this.userDetailsForm.setValue({
        'FirstName': user.details.firstName,
        'LastName': user.details.lastName,
        'Login': user.login,
        'Status': user.status
      });
      this.userDetailsForm.get('Login')?.disable;
      this.isAdmin = user.isAdmin;
      this.userEmails = user.emails;
      user.phoneNumbers.forEach((mob: any) => { mob.countryCodeValue = UserCountries[mob.countryCode.startsWith('+') ? mob.countryCode.slice(1) : mob.countryCode]; })
      this.userMobiles = user.phoneNumbers;
      this.userRepCodes = user.repCodes;
      this.userModels = user.models;
      this.refreshMobileGridData();
      this.refreshEmailGridData();
    }
  }

  private showErrorMessage(message: string) {
    this.messageService.showErrorPopup({ message: message });
  }

  public submitUserForm() {
    Utility.cleanForm(this.userDetailsForm);
    if (this.validateUserDetails) {
      if (!this.PrimaryEmail)
        this.showErrorMessage("Plz set a Primary Email");
      else if (!this.PrimaryMobile)
        this.showErrorMessage("Plz set a Primary Mobile");
      else {
        var formData = this.userDetailsForm.value;
        var User = {
          Id: this.userId,
          Models: this.userModels,
          Emails: this.removeUIRandomId(this.userEmails),
          PhoneNumbers: this.removeUIRandomId(this.userMobiles),
          RepCodes: this.removeUIRandomId(this.userRepCodes),
          Login: formData.Login,
          Status: formData.Status,
          Details: {
            FirstName: formData.FirstName,
            LastName: formData.LastName
          }
        };
        this.userService.saveUser(User)
          .subscribe((response: any) => {
            if (response['success'] == Constants.SuccessResponse) {
              this.messageService.showSuccessPopup({ message: response['message'] });
              this.router.navigate(['../'], { relativeTo: this.route });
            }
          });
      }
    }
  }


  private removeUIRandomId(array: Array<any>) {
    return array.map(it => { if (it.isNewlyAdded) it.id = 0; return it });
  }

  public selectModels() {
    this.showModelListingSidebar = true;
  }

  public closeModelSidebar() {
    this.showModelListingSidebar = false;
  }

  public selectRepCodes() {
    this.showRepCodesListingSidebar = true;
  }

  public closeRepCodesSidebar() {
    this.showRepCodesListingSidebar = false;
  }

  public submitMobileForm() {
    if (this.mobileForm.valid) {
      let formData = this.mobileForm.value;
      var userMobile = {
        countryCode: '+' + formData.Code,
        countryCodeValue: UserCountries[formData.Code],
        phoneNo: formData.Mobile,
        isPrimary: this.userMobiles.length ? false : true,
        id: Utility.getRandomIdentifier(),
        isNewlyAdded: true
      };
      var ind = this.userMobiles.findIndex(it => it.phoneNo === userMobile.phoneNo);
      if (ind === -1) {
        this.userMobiles.push(userMobile);
        this.refreshMobileGridData();
        this.mobileForm.reset();
      } else
        this.messageService.showErrorPopup({ message: 'This Mobile No. already exists' });
    }
  }

  public submiEmailForm() {
    if (this.emailForm.valid) {
      let formData = this.emailForm.value;
      var userEmail = {
        email: formData.Email,
        isPrimary: this.userEmails.length ? false : true,
        id: Utility.getRandomIdentifier(),
        isNewlyAdded: true
      };
      var ind = this.userEmails.findIndex(it => it.email === userEmail.email);
      if (ind === -1) {
        this.userEmails.push(userEmail);
        this.refreshEmailGridData();
        this.emailForm.reset();
      } else
        this.messageService.showErrorPopup({ message: 'This Email already exists' });
    }
  }


  public deleteSeletedMobiles() {
    this.userMobiles = this.userMobiles.filter(it => !this.selectedMobileIds.includes(it.id));
    if (this.userMobiles.length && !this.userMobiles.find(mob => mob.isPrimary))
      this.userMobiles[0].isPrimary = true;
    this.refreshMobileGridData();
  }

  public deleteSeletedEmails() {
    this.userEmails = this.userEmails.filter(it => !this.selectedEmailIds.includes(it.id));
    if (this.userEmails.length && !this.userEmails.find(mob => mob.isPrimary))
      this.userEmails[0].isPrimary = true;
    this.refreshEmailGridData();
  }

  private refreshMobileGridData() {
    this.refreshMobileGrid = !this.refreshMobileGrid;
  }

  private refreshEmailGridData() {
    this.refreshEmailGrid = !this.refreshEmailGrid;
  }


  getUserMobiles = (gridModel: any) => {
    return new Promise(resolveCallback => {
      var data = {
        success: true,
        response: {
          totalRecords: this.userMobiles.length,
          gridData: this.userMobiles
        }
      }
      resolveCallback(data);
    });
  }


  getUserEmails = (gridModel: any) => {
    return new Promise(resolveCallback => {
      var data = {
        success: true,
        response: {
          totalRecords: this.userEmails.length,
          gridData: this.userEmails
        }
      }
      resolveCallback(data);
    });
  }


  public gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    switch (gridSelect.Grid) {
      case this.mobileGridName:
        this.selectedMobileIds = gridSelect.SelectedKeys;
        break;
      case this.emailGridName:
        this.selectedEmailIds = gridSelect.SelectedKeys;
        break;
      case DataGridNames.ModelListingSidebar:
        // manually emitted from Sidebar
        this.userModels = gridSelect.SelectedRows;
        this.closeModelSidebar();
        break;
      case DataGridNames.RepCodeListing:
        // manually emitted from Sidebar
        this.userRepCodes = gridSelect.SelectedRows;
        this.closeRepCodesSidebar();
        break;
    }
  }

  editGridCallback = (gridEditModel: DataGridEditModel) => {
    let GridArray: Array<any> = gridEditModel.Grid === this.emailGridName ? this.userEmails : this.userMobiles;
    var primaryRow = GridArray.find(sl => sl.isPrimary == true);
    var editRow = GridArray.find(sl => sl.id == gridEditModel.EditRowKey);
    if (gridEditModel.UpdatedValues && editRow && editRow.id != primaryRow.id) {
      Object.entries(gridEditModel.UpdatedValues).forEach(([key, value]: [any, any]) => {
        editRow[key] = value;
      })
      if (primaryRow)
        primaryRow.isPrimary = false;
      if (gridEditModel.Grid === this.emailGridName)
        this.refreshEmailGridData();
      else
        this.refreshMobileGridData();
    }
    return new Promise(resolveCallback => {
      resolveCallback(editRow);
    });
  }


}
