import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridRowSelectionModel } from '../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { DataGridNames } from '../../support/enums/data-grid.enum';
import { Utility } from '../../support/utility/utility';
import { Constants } from '../../support/constants/constants';
import { AccountsService } from '../../services/Accounts/accounts.service';
import { DataGridRowDblClickModel } from '../../models/DataGrid/DataGridRowDBLClickModel/data-grid-row-dbl-click-model';
import { AuthenticationService } from '../../services/AuthenticationService/authentication.service';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { ConfirmationPopupComponent } from '../../components/popups/confirmation-popup/confirmation-popup.component';
import { MatDialog } from '@angular/material/dialog';
import { ImportService } from '../../services/Import/import.service';
import * as $ from 'jquery';

@Component({
  selector: 'zega-account-listing',
  templateUrl: './account-listing.component.html',
  styleUrls: ['./account-listing.component.less']
})
export class AccountListingComponent implements OnInit {

  IsAdmin: boolean = false;
  showBulkEditSidebar: boolean = false;
  gridName: DataGridNames = DataGridNames.AccountListing;
  refreshGrid: boolean = false;
  selectedAccountIds: Array<number> = [];
  selectedAccounts: Array<any> = [];
  
  get delMsg() {
    let len = this.selectedAccountIds.length;
    let text = len == 1 ? " Account" : " Accounts";
    return len + text;
  }
  constructor(private router: Router, private authService: AuthenticationService, private accountService: AccountsService, private importService: ImportService, private route: ActivatedRoute, private messageService: AuthMessageService, private matDialog: MatDialog) { }

  ngOnInit(): void {
    let login = this.authService.getLoggedInUserInfo;
    if (login)
      this.IsAdmin = login.isAdmin;
  }

  gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName) {
      this.selectedAccountIds = gridSelect.SelectedKeys;
      this.selectedAccounts = gridSelect.SelectedRows;
    }
  }

  gridRowsDblClickedCallback(gridRowDblClk: DataGridRowDblClickModel) {
    if (gridRowDblClk.Grid == this.gridName && gridRowDblClk.Key)
      this.navigateToAccount(gridRowDblClk.Key);
  }

  addAccount() {
    if (this.IsAdmin)
      this.navigateToAccount(0);
    else
      this.messageService.showErrorPopup({ message: Constants.AdminOnlyPermissionMsg });
  }

  editAccount() {
    if (this.selectedAccountIds.length === 1)
      this.navigateToAccount(this.selectedAccountIds[0]);
  }

  deleteAccounts() {
    const dialogRef = this.matDialog.open(ConfirmationPopupComponent, {
      width: Constants.DeleteDiaogWidth,
      disableClose: Constants.DiaogDisableClose,
      data: { Header: "Delete Confirmation!", Message: "Are you sure? You want to delete " + this.delMsg }
    });

    dialogRef.afterClosed().subscribe((isSuccess: any) => {
      if (isSuccess)
        this.delAccountsCallBack();
    });
  }
  private delAccountsCallBack() {
    if (this.IsAdmin) {
      if (this.selectedAccountIds.length)
        this.accountService.deleteAccount(this.selectedAccountIds)
          .subscribe((response: any) => {
            if (response['success'] == Constants.SuccessResponse) {
              this.messageService.showSuccessPopup({ message: response['message'] });
              this.refreshDataGrid();
            }
          });
    }
    else
      this.messageService.showErrorPopup({ message: Constants.AdminOnlyPermissionMsg });
  }

  private navigateToAccount(accountId: number) {
    let encrptedId = Utility.compressURL(accountId.toString());
    this.router.navigate(['./', encrptedId], { relativeTo: this.route });
  }

  refreshDataGrid() {
    this.selectedAccounts = this.selectedAccountIds = [];
    this.refreshGrid = !this.refreshGrid;
  }

  bulkEditAcounts() {
    if (this.selectedAccountIds.length > 1)
      this.showBulkSidebar();
  }

  private showBulkSidebar() {
    this.showBulkEditSidebar = true;
  }

  private closeBulkSidebar() {
    this.showBulkEditSidebar = false;
  }

  bulkEditSidebarCallback(isSuccess: boolean) {
    if (isSuccess)
      this.refreshDataGrid();
    this.closeBulkSidebar();
  }

  processUploadedFile(event: any) {
    let files = event.target.files;
    if (files.length === 0)
      return;
    let fileToUpload = <File>files[0];
    let validationErrors: Array<string> = [];
    if (["application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text/csv"].indexOf(fileToUpload.type) < 0)
      validationErrors.push('Please select only CSV Files');
    if (validationErrors.length) {
      this.messageService.showErrorPopup({ message: validationErrors.join(', ') });
      $('#import-file-input').val('');
      event.target.value = '';
      return;
    }
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    formData.append('RequestSource', DataGridNames[this.gridName].toString());
    this.importService.uploadImportFile(formData).
      subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          this.messageService.showSuccessPopup({ message: response['message'] });
          this.refreshDataGrid();
        }
        $('#import-file-input').val('');
        event.target.value = '';
      });
  }


}
