import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormArray, FormControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Constants } from '../../support/constants/constants';
import { GridRowSelectionModel } from '../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { DataGridNames } from '../../support/enums/data-grid.enum';
import { Utility } from '../../support/utility/utility';
import { AccountsService } from '../../services/Accounts/accounts.service';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { MatStepper } from '@angular/material/stepper';
import { ZegaValidators } from '../../support/validators/validators';
import { WithdrawalStatusEnum, WithdrawlDepositStatusEnum } from '../../support/enums/account.enum';
import { AuthenticationService } from '../../services/AuthenticationService/authentication.service';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'zega-add-account',
  templateUrl: './add-account.component.html',
  styleUrls: ['./add-account.component.less']
})
export class AddAccountComponent implements OnInit {
  @ViewChild('stepper') private myStepper!: MatStepper;
  basicDetailsForm: FormGroup;
  AdditionalWithdrawals: FormGroup;
  AdditionalDeposits: FormGroup;
  ZegaCustomFields: FormGroup;

  gridPanelOpenState: boolean = false;

  public accountId: number = 0;
  advisorsName: string = '';
  gridName: DataGridNames = DataGridNames.ModelListing;
  withdrawlorDepositStatus = WithdrawlDepositStatusEnum;
  withdrawalStatus = WithdrawalStatusEnum;
  selected: number = 1;
  disabledForAddWithdrawl: boolean = true; disabledForOneWithdrawl: boolean = true; disabledForAddDeposit: boolean = true;
  TotalAvailableBalance: number = 0;
  AvailableBalance: number = 0;
  Id: number = 0;
  modelId: number = 0;
  modelName: string = '';
  selectedModelIds: Array<number> = [];
  selectedModels: Array<any> = [];
  accountStatus: any = {};
  accountType: any = {};
  frequency: any = {};
  broker: any = {};
  repcodeList: Array<any> = [];
  repcodeListObs: Observable<any[]> | undefined;

  minDate: any = Utility.GetTodaysDate();
  IsAdmin: boolean = false;
  isEditable = false;
  showBlendModelSidebar: boolean = false;
  blendedModelsItems: Array<any> = [];
  refreshGrid: boolean = false;
  isLinearStepper: boolean = true;
  private accountRepCodeId = 0;

  ngOnInit(): void {
    var encodedParam = this.route.snapshot.paramMap.get('accountId')
    var param = Utility.decompressURL(String(encodedParam));
    this.accountId = param ? Number.parseInt(param) : 0;
    this.GetAccountDropdowns();

    this.AdditionalWithdrawals.get('Future_Withdrawal')?.valueChanges.subscribe(data => {
      if (data == WithdrawlDepositStatusEnum.No) {
        this.disabledForAddWithdrawl = true;
        this.AdditionalWithdrawals.patchValue({ Withdrawl_Amount: null, Withdrawl_Date: null });
      } else {
        this.disabledForAddWithdrawl = false;
      }
    });

    this.AdditionalWithdrawals.get('One_Time_Withdrawal')?.valueChanges.subscribe(data => {
      if (data == WithdrawlDepositStatusEnum.No) {
        this.disabledForOneWithdrawl = true;
        this.AdditionalWithdrawals.patchValue({ One_Time_Withdrawal_Amount: null, One_Time_Withdrawal_Date: null });
      } else {
        this.disabledForOneWithdrawl = false;
      }
    });

    this.AdditionalDeposits.get('Deposit_Status')?.valueChanges.subscribe(data => {
      if (data == WithdrawlDepositStatusEnum.No) {
        this.disabledForAddDeposit = true;
        this.AdditionalDeposits.patchValue({ Deposit_Amount: null, Deposit_Date: null });
      } else {
        this.disabledForAddDeposit = false;
      }
    });
    let login = this.authService.getLoggedInUserInfo;
    if (login)
      this.IsAdmin = login.isAdmin;

    this.repcodeListObs = this.basicDetailsForm.get('RepCode')?.valueChanges.pipe(
      startWith(''),
      map(value => (typeof value === 'string' ? value : value.name)),
      map(name => (name ? this._filter(name) : this.repcodeList.slice())),
    );
  }

  private _filter(name: string): any[] {
    const filterValue = name.toLowerCase();
    return this.repcodeList.filter(option => option.name.toLowerCase().includes(filterValue));
  }

  public repCodeSelected(event: MatAutocompleteSelectedEvent) {
    var repCode = event.option.value;
    this.basicDetailsForm.get('RepCode')?.setValue(repCode.name);
    this.getAdvisorsByRepCode(repCode.id);
  }

  private getAdvisorsByRepCode(repCodeId: number) {
    if (this.accountRepCodeId != repCodeId) {
      this.accountRepCodeId = repCodeId;
      this.accountsService.getAdvisorsByRepCode(repCodeId)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse)
            this.advisorsName = response['response'];
        });
    }
  }

  constructor(private fb: FormBuilder, private accountsService: AccountsService, private router: Router, private route: ActivatedRoute, private messageService: AuthMessageService, private authService: AuthenticationService) {

    this.basicDetailsForm = this.fb.group({
      ClientName: new FormControl(null, [Validators.required]),
      Name: new FormControl(null, []),
      AccountValue: new FormControl(null, [Validators.required]),
      Number: new FormControl(null, [Validators.required, Validators.pattern(Constants.SpecialCharNotAllowed)]),
      AccountType: new FormControl(0, [Validators.required]),
      SBuyingPower: new FormControl(null, []),
      OBuyingPower: new FormControl(null, []),
      CashEq: new FormControl(null, []),
      RepCode: new FormControl(null, [Validators.required]),
      CashNetBal: new FormControl(null, []),
      AccountStatus: new FormControl(0, [Validators.required]),
      VeoImportDate: new FormControl({ value: null, disabled: true }, []),
      AllocationDate: new FormControl(null, []),
      Broker: new FormControl(0, [Validators.required]),
      Notes: new FormControl(null, []),
    });

    this.AdditionalWithdrawals = this.fb.group({
      Future_Withdrawal: new FormControl(WithdrawlDepositStatusEnum.No, [Validators.required]),
      Withdrawl_Amount: new FormControl(null, []),
      Withdrawl_Frequency: new FormControl(0, [Validators.required]),
      Withdrawl_Date: new FormControl(null, []),
      One_Time_Withdrawal: new FormControl(WithdrawlDepositStatusEnum.No, [Validators.required]),
      One_Time_Withdrawal_Amount: new FormControl(null, []),
      One_Time_Withdrawal_Date: new FormControl(null, []),
    }, {
      validator: [
        ZegaValidators.conditionallyRequiredValidator('Future_Withdrawal', WithdrawlDepositStatusEnum.Yes, 'Withdrawl_Amount'),
        ZegaValidators.conditionallyRequiredValidator('Future_Withdrawal', WithdrawlDepositStatusEnum.Yes, 'Withdrawl_Frequency'),
        ZegaValidators.conditionallyRequiredValidator('Future_Withdrawal', WithdrawlDepositStatusEnum.Yes, 'Withdrawl_Date'),
        // ZegaValidators.conditionallyRequiredDateMinToday('Future_Withdrawal', WithdrawlDepositStatusEnum.Yes, 'Withdrawl_Date', this.minDate),
        ZegaValidators.conditionallyRequiredValidator('One_Time_Withdrawal', WithdrawlDepositStatusEnum.Yes, 'One_Time_Withdrawal_Amount'),
        ZegaValidators.conditionallyRequiredValidator('One_Time_Withdrawal', WithdrawlDepositStatusEnum.Yes, 'One_Time_Withdrawal_Date'),
        // ZegaValidators.conditionallyRequiredDateMinToday('One_Time_Withdrawal', WithdrawlDepositStatusEnum.Yes, 'One_Time_Withdrawal_Date', this.minDate)
      ]
    });

    this.AdditionalDeposits = this.fb.group({
      Deposit_Status: new FormControl(WithdrawlDepositStatusEnum.No, [Validators.required]),
      Deposit_Amount: new FormControl(null, []),
      Deposit_Frequency: new FormControl(0, [Validators.required]),
      Deposit_Date: new FormControl(null, []),
    }, {
      validator: [
        ZegaValidators.conditionallyRequiredValidator('Deposit_Status', WithdrawlDepositStatusEnum.Yes, 'Deposit_Amount'),
        ZegaValidators.conditionallyRequiredValidator('Deposit_Status', WithdrawlDepositStatusEnum.Yes, 'Deposit_Frequency'),
        ZegaValidators.conditionallyRequiredValidator('Deposit_Status', WithdrawlDepositStatusEnum.Yes, 'Deposit_Date'),
        // ZegaValidators.conditionallyRequiredDateMinToday('Deposit_Status', WithdrawlDepositStatusEnum.Yes, 'Deposit_Date', this.minDate)
      ]
    });

    this.ZegaCustomFields = this.fb.group({
      ZEGA_Confirmed: new FormControl(WithdrawlDepositStatusEnum.No, [Validators.required]),
      ZEGA_Alert_Date: new FormControl(null, []),
      ZEGA_Notes: new FormControl(null, []),
    });
  }

  public gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName) {
      this.selectedModelIds = gridSelect.SelectedKeys;
      this.selectedModels = gridSelect.SelectedRows;
      this.modelId = this.selectedModelIds.length ? this.selectedModelIds[0] : 0;
      this.modelName = this.selectedModelIds.length ? this.selectedModels[0].name : '';
    }
  }

  get AccoutModelListingGridAddParam() {
    return {
      AccountId: this.accountId,
      RepCodeId: this.accountRepCodeId
    }
  }

  private navigateToAccountListing() {
    this.router.navigate(['/accounts']);
  }

  submitbasicDetailsForm(moveToNextStep: boolean) {
    Utility.cleanForm(this.basicDetailsForm);
    if (this.basicDetailsForm.valid) {
      let Data = {
        Id: this.accountId,
        accountBasicDetails: Object.assign({}, this.basicDetailsForm.value)
      };
      Data.accountBasicDetails.VeoImportDate = this.basicDetailsForm.get("VeoImportDate")?.value;
      Data.accountBasicDetails.Broker = this.basicDetailsForm.get("Broker")?.value;
      Data.accountBasicDetails.RepCode = { Id: this.accountRepCodeId },
        this.accountsService.saveBasicDetails(Data)
          .subscribe((response: any) => {
            if (response['success'] == Constants.SuccessResponse) {
              var retData = response['response'];
              this.accountId = retData.accountId;
              moveToNextStep ? this.myStepper.next() : this.navigateToAccountListing();
              this.isLinearStepper = false;
            }
          });
      this.TotalAvailableBalance = this.basicDetailsForm.get('CashNetBal')?.value;
      this.AvailableBalance = this.basicDetailsForm.get('CashNetBal')?.value;

    }
  }

  submitModelForm(moveToNextStep: boolean) {
    let Data = {
      Id: this.accountId,
      Model: {
        Id: this.modelId,
        Name: this.modelName,
      }
    }
    this.accountsService.saveModelDetails(Data)
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          this.messageService.showSuccessPopup({ message: response['message'] });
          moveToNextStep ? this.myStepper.next() : this.navigateToAccountListing();
        }
      });
  }

  submitAddWithdrawlForm(moveToNextStep: boolean) {
    let Data = {
      Id: this.accountId,
      accounWithdrawlInfoModelcs: Object.assign({}, this.AdditionalWithdrawals.value),
    };
    this.accountsService.saveAddWithdrawl(Data)
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          this.messageService.showSuccessPopup({ message: response['message'] });
          moveToNextStep ? this.myStepper.next() : this.navigateToAccountListing();
        }
      });
  }

  submitAddDepositForm(moveToNextStep: boolean) {
    let Data = {
      Id: this.accountId,
      accountDepositsInfoModelcs: Object.assign({}, this.AdditionalDeposits.value),
    };
    this.accountsService.saveAddDeposit(Data)
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          this.messageService.showSuccessPopup({ message: response['message'] });
          moveToNextStep ? this.myStepper.next() : this.navigateToAccountListing();
        }
      });
  }

  submitZegaCustomForm() {
    let Data = {
      Id: this.accountId,
      accountZegaCustomFieldsModel: Object.assign({}, this.ZegaCustomFields.value),
    };
    this.accountsService.saveZegaCustom(Data)
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          this.messageService.showSuccessPopup({ message: response['message'] });
          this.navigateToAccountListing();
        }
      });
  }

  private GetAccountDropdowns() {
    this.accountsService.addAccountDropdown()
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          var retData = response['response'];
          this.setAccountDropdownData(retData);
          this.getAccountInfo();
        }
      });
  }

  private setAccountDropdownData(data: any) {
    this.accountStatus = data.accountStatus;
    this.basicDetailsForm.get('accountStatus')?.setValue(data.accountStatus.key[0]);
    this.accountType = data.accountType;
    this.frequency = data.frequency;
    this.repcodeList = data.repCodes;
    this.broker = data.broker;
  }

  private getAccountInfo() {
    if (this.accountId) {
      this.accountsService.getAccount(this.accountId)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            var retData = response['response'];
            this.setAccountData(retData);
            this.isLinearStepper = false;
          }
        });
    }
  }

  public setAccountData(Data: any) {
    if (Data) {
      var accountRepCodeId = Data.accountBasicDetails.repCode.id;
      this.basicDetailsForm.patchValue({
        ClientName: Data.accountBasicDetails.clientName,
        Name: Data.accountBasicDetails.name,
        AccountValue: Data.accountBasicDetails.accountValue,
        Number: Data.accountBasicDetails.number,
        AccountType: Data.accountBasicDetails.accountType,
        SBuyingPower: Data.accountBasicDetails.sBuyingPower,
        OBuyingPower: Data.accountBasicDetails.oBuyingPower,
        CashEq: Data.accountBasicDetails.cashEq,
        RepCode: this.repcodeList.find(rc => rc.id == accountRepCodeId)?.name ?? '',
        CashNetBal: Data.accountBasicDetails.cashNetBal,
        AccountStatus: Data.accountBasicDetails.accountStatus,
        VeoImportDate: Data.accountBasicDetails.veoImportDate ? Data.accountBasicDetails.veoImportDate.split('T')[0] : null,
        AllocationDate: Data.accountBasicDetails.allocationDate ? Data.accountBasicDetails.allocationDate.split('T')[0] : null,
        Broker: Data.accountBasicDetails.broker,
        Notes: Data.accountBasicDetails.notes,
      });
      this.basicDetailsForm.get("RepCode")?.disable();
      this.getAdvisorsByRepCode(accountRepCodeId);
      if (Data.accountBasicDetails.veoImportDate)
        this.basicDetailsForm.get('Broker')?.disable();
      this.AdditionalWithdrawals.patchValue({
        Future_Withdrawal: Data.accounWithdrawlInfoModelcs.future_Withdrawal,
        Withdrawl_Amount: Data.accounWithdrawlInfoModelcs.withdrawl_Amount,
        Withdrawl_Frequency: Data.accounWithdrawlInfoModelcs.withdrawl_Frequency,
        Withdrawl_Date: Data.accounWithdrawlInfoModelcs.withdrawl_Date ? Data.accounWithdrawlInfoModelcs.withdrawl_Date.split('T')[0] : null,
        One_Time_Withdrawal: Data.accounWithdrawlInfoModelcs.one_Time_Withdrawal,
        One_Time_Withdrawal_Amount: Data.accounWithdrawlInfoModelcs.one_Time_Withdrawal_Amount,
        One_Time_Withdrawal_Date: Data.accounWithdrawlInfoModelcs.one_Time_Withdrawal_Date ? Data.accounWithdrawlInfoModelcs.one_Time_Withdrawal_Date.split('T')[0] : null,
      });
      this.AdditionalDeposits.patchValue({
        Deposit_Status: Data.accountDepositsInfoModelcs.deposit_Status,
        Deposit_Amount: Data.accountDepositsInfoModelcs.deposit_Amount,
        Deposit_Frequency: Data.accountDepositsInfoModelcs.deposit_Frequency,
        Deposit_Date: Data.accountDepositsInfoModelcs.deposit_Date ? Data.accountDepositsInfoModelcs.deposit_Date.split('T')[0] : null,
      });
      this.ZegaCustomFields.patchValue({
        ZEGA_Confirmed: Data.accountZegaCustomFieldsModel.zega_Confirmed,
        ZEGA_Alert_Date: Data.accountZegaCustomFieldsModel.zega_Alert_Date ? Data.accountZegaCustomFieldsModel.zega_Alert_Date.split('T')[0] : null,
        ZEGA_Notes: Data.accountZegaCustomFieldsModel.zega_Notes,
      });
      this.modelName = Data.model?.name ?? '';
      this.modelId = Data.model?.id ?? 0;
      this.TotalAvailableBalance = Data.accountBasicDetails.cashNetBal;
      this.AvailableBalance = Data.accountBasicDetails.cashNetBal;
      Data.model ? this.selectedModels.push(Data.model) : false;
    }
  }

  private showBlendSidebar() {
    this.showBlendModelSidebar = true;
  }

  private closeBlendSidebar() {
    this.showBlendModelSidebar = false;
  }

  blendModelSidebarCallback(isSuccess: boolean) {
    if (isSuccess)
      this.refreshDataGrid();
    this.closeBlendSidebar();
  }

  refreshDataGrid() {
    this.refreshGrid = !this.refreshGrid;
  }

  blendModels() {
    if (this.selectedModels.length) {
      if (this.selectedModels.filter(m => m.isBlendModel).length)
        this.messageService.showErrorPopup({ message: "Blended Model can't be Blended again" });
      else {
        this.blendedModelsItems = this.selectedModels;
        this.showBlendSidebar();
      }
    }
  }

}
