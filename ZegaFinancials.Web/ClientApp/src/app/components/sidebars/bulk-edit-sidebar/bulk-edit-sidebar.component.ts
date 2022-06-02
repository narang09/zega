import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { AuthMessageService } from '../../../services/AuthMessageService/auth-message.service';
import { SidebarService } from '../../../services/SidebarService/sidebar.service';
import { Constants } from '../../../support/constants/constants';
import { DataGridNames } from '../../../support/enums/data-grid.enum';
import { Utility } from '../../../support/utility/utility';

@Component({
  selector: 'zega-bulk-edit-sidebar',
  templateUrl: './bulk-edit-sidebar.component.html',
  styleUrls: ['./bulk-edit-sidebar.component.less']
})
export class BulkEditSidebarComponent implements OnInit {

  @Input() selectedIds: Array<number> = [];
  @Input() gridName: DataGridNames = DataGridNames.None;
  @Output() confirmCallback: EventEmitter<boolean> = new EventEmitter();
  @Output() cancelCallback: EventEmitter<boolean> = new EventEmitter();

  bulkEditForm: FormGroup;
  dropDown1 = {};
  dropDown2 = {};
  dropDownArr1: Array<any> = [];
  get disableBtn() {
    const obj = this.getBulkEditRequestModel();
    return Object.keys(obj).length;
  }

  // For Dropdowns
  DataGridNames = DataGridNames;

  constructor(private fb: FormBuilder, private messageService: AuthMessageService, private sidebarService: SidebarService) {
    this.bulkEditForm = this.fb.group({
      Value1: new FormControl("-1", []),
      Value2: new FormControl("-1", []),
      Value3: new FormControl("-1", []),
      Value4: new FormControl([], []),

    });
  }

  ngOnInit(): void {
    Utility.setSidebarContentHeight();
    if (this.gridName != DataGridNames.None && this.selectedIds.length) {
      let bulkEditReqModel = {
        DataStoreIds: this.selectedIds,
        GridName: DataGridNames[this.gridName]
      };
      this.sidebarService.getBulkSidebarDropdowns(bulkEditReqModel)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse)
            this.setBulkEditDropDowns(response['response']);
        });

    }
  }
  private setBulkEditDropDowns(dropdowns: any) {
    if (dropdowns) {
      switch (this.gridName) {
        case DataGridNames.AccountListing:
          this.dropDown1 = dropdowns.accountStatus ?? {};
          this.dropDown2 = dropdowns.accountType?? {};
          this.dropDownArr1 = dropdowns.models ?? [];
          break;
        case DataGridNames.AdvisorListing:
          this.dropDown1 = dropdowns.status ?? {};
          break;
        case DataGridNames.ModelListing:
          this.dropDownArr1 = dropdowns.strategies ?? [];
          break;
      }
    }
  }

  cancel() {
    this.cancelCallback.emit(false);
  }

  submitBulkEditForm() {
    let bulkEditModel = this.getBulkEditRequestModel();
    if (Object.keys(bulkEditModel).length) {
      bulkEditModel.GridName = DataGridNames[this.gridName];
      bulkEditModel.DataStoreIds = this.selectedIds;
      this.sidebarService.saveBulkEditInfo(bulkEditModel)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.confirmCallback.emit(true);
          }
        });
    }
  }

  private getBulkEditRequestModel() {
    let data = this.bulkEditForm.value;
    let bulkModel: any = {}
    switch (this.gridName) {
      case DataGridNames.AccountListing:
        if (this.isSelectedValueValid(data.Value1))
          bulkModel.AccountStatus = data.Value1;
        if (this.isSelectedValueValid(data.Value2))
          bulkModel.AccountType = data.Value2;
        if (this.isSelectedValueValid(data.Value3))
          bulkModel.ModelId = data.Value3;
        break;
      case DataGridNames.AdvisorListing:
        if (this.isSelectedValueValid(data.Value1))
          bulkModel.Status = data.Value1;
        break;
      case DataGridNames.ModelListing:
        if (this.isSelectedValueValidArray(data.Value4))
          bulkModel.StrategyIds = data.Value4;
        break;
    }
    return bulkModel;
  }

  private isSelectedValueValid(value: any) {
    return !(value === null || value === undefined || value === '' || value == -1);
  }
  
  private isSelectedValueValidArray(value: any) {
    return value && value.length;
  }

}
