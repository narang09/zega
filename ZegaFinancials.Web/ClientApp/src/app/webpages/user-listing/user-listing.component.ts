import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DataGridRowDblClickModel } from '../../models/DataGrid/DataGridRowDBLClickModel/data-grid-row-dbl-click-model';
import { GridRowSelectionModel } from '../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { UserService } from '../../services/User/user.service';
import { Constants } from '../../support/constants/constants';
import { DataGridNames } from '../../support/enums/data-grid.enum';
import { Utility } from '../../support/utility/utility';
import { ConfirmationPopupComponent } from '../../components/popups/confirmation-popup/confirmation-popup.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'zega-user-listing',
  templateUrl: './user-listing.component.html',
  styleUrls: ['./user-listing.component.less']
})
export class UserListingComponent implements OnInit {

  refreshGrid: boolean = false;
  showBulkEditSidebar: boolean = false;
  showRepCodesListingSidebar: boolean = false;
  gridName: DataGridNames = DataGridNames.AdvisorListing;
  selectedUserIds: Array<number> = []
  selectedUsers: Array<any> = []

  constructor(private router: Router, private userService: UserService, private route: ActivatedRoute, private messageService: AuthMessageService, private matDialog: MatDialog) { }

  ngOnInit(): void { }

  public gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName) {
      this.selectedUserIds = gridSelect.SelectedKeys;
      this.selectedUsers = gridSelect.SelectedRows;
    }      
  }

  gridRowsDblClickedCallback(gridRowDblClk: DataGridRowDblClickModel) {
    if (gridRowDblClk.Grid == this.gridName && gridRowDblClk.Key)
      this.navigateToUser(gridRowDblClk.Key);
  }

  public addAdvisor() {
    this.navigateToUser(0);
  }

  public viewAdvisor() {
    if (this.selectedUserIds.length === 1) {
      var userId: number = this.selectedUserIds[0];
      this.navigateToUser(userId);
    }
  }

  public deleteAdvisors() {
    const dialogRef = this.matDialog.open(ConfirmationPopupComponent, {
      width: Constants.DeleteDiaogWidth,
      disableClose: Constants.DiaogDisableClose,
      data: { Header: "Delete Confirmation!", Message: `Are you sure? You want to delete ${this.selectedUserIds.length} User(s)` }
    });

    dialogRef.afterClosed().subscribe((isSuccess: any) => {
      if (isSuccess)
        this.delAdvisorCallBack();
    });
  }

  private delAdvisorCallBack() {
    if (this.selectedUserIds.length) {
      this.userService.deleteUsers(this.selectedUserIds)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.refreshDataGrid();
          }
        });
    }
  }

  private navigateToUser(userId: number) {
    let encrptedId = Utility.compressURL(userId.toString());
    this.router.navigate(['./', encrptedId], { relativeTo: this.route });
  }

  refreshDataGrid() {
    this.selectedUsers = this.selectedUserIds = [];
    this.refreshGrid = !this.refreshGrid;
  }

  bulkEditAdvisors() {
    if (this.selectedUserIds.length > 1)
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

  public openRepCodesListingSidebar() {
    this.showRepCodesListingSidebar = true;
  }

  public closeRepCodesSidebar() {
    this.showRepCodesListingSidebar = false;
  }

}
