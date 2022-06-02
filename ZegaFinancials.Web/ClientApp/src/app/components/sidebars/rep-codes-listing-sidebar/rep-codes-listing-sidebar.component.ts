import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { GridRowSelectionModel } from '../../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { AuthMessageService } from '../../../services/AuthMessageService/auth-message.service';
import { UserService } from '../../../services/User/user.service';
import { Constants } from '../../../support/constants/constants';
import { DataGridNames } from '../../../support/enums/data-grid.enum';
import { RepCodeType } from '../../../support/enums/user.enum';
import { Utility } from '../../../support/utility/utility';

@Component({
  selector: 'zega-rep-codes-listing-sidebar',
  templateUrl: './rep-codes-listing-sidebar.component.html',
  styleUrls: ['./rep-codes-listing-sidebar.component.less']
})
export class RepCodesListingSidebarComponent implements OnInit {

  @Output() cancelCallback: EventEmitter<boolean> = new EventEmitter();

  gridName: DataGridNames = DataGridNames.RepCodeListing;
  refreshGrid: boolean = false;
  selectedRepCodes: Array<any> = [];
  repCodeForm: FormGroup;
  repCodesDropdown = RepCodeType;
  repCodeId: number = 0;

  constructor(private fb: FormBuilder, private userService: UserService, private messageService: AuthMessageService) {
    this.repCodeForm = this.fb.group({
      Type: new FormControl(RepCodeType.AssetBased, [Validators.required]),
      Code: new FormControl(null, [Validators.required])
    });
  }

  ngOnInit(): void {
    Utility.setSidebarContentHeight();
  }

  public gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName)
      this.selectedRepCodes = gridSelect.SelectedRows;
  }

  public cancel() {
    this.cancelCallback.emit(false);
  }

  public submiRepCodeForm() {
    Utility.cleanForm(this.repCodeForm);
    if (this.repCodeForm.valid) {
      let repCode = this.repCodeForm.value;
      repCode.Id = this.repCodeId;
      repCode.Code = repCode.Code.toUpperCase();
      this.userService.saveRepCode(repCode)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.refreshGridData();
            this.repCodeForm.reset();
          }
        });
    }
  }

  public deleteSeletedRepCodes() {
    if (this.selectedRepCodes.length) {
      var repCodeIds = this.selectedRepCodes.map(rc => rc.id);
      this.userService.deleteRepCodes(repCodeIds)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.refreshGridData();
          }
        });
    }
  }

  private refreshGridData() {
    this.selectedRepCodes = [];
    this.repCodeId = 0;
    this.refreshGrid = !this.refreshGrid;
  }

  public editRepCode() {
    if (this.selectedRepCodes.length == 1) {
      var repCode = this.selectedRepCodes[0];
      this.repCodeId = repCode.id;
      this.repCodeForm.patchValue({
        Type: repCode.type,
        Code: repCode.code
      });
    }
  }

}
