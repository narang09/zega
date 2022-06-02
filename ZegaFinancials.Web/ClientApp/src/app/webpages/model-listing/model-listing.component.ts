import { Component, OnInit } from '@angular/core';
import { DataGridRowDblClickModel } from '../../models/DataGrid/DataGridRowDBLClickModel/data-grid-row-dbl-click-model';
import { GridRowSelectionModel } from '../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { ModelService } from '../../services/ModelService/model.service';
import { Constants } from '../../support/constants/constants';
import { DataGridNames } from '../../support/enums/data-grid.enum';
import { AuthenticationService } from '../../services/AuthenticationService/authentication.service';
import { ConfirmationPopupComponent } from '../../components/popups/confirmation-popup/confirmation-popup.component';
import { MatDialog } from '@angular/material/dialog';
@Component({
  selector: 'zega-model-listing',
  templateUrl: './model-listing.component.html',
  styleUrls: ['./model-listing.component.less']
})
export class ModelListingComponent implements OnInit {

  gridName: DataGridNames = DataGridNames.ModelListing;
  subGridName: DataGridNames = DataGridNames.ModelListingSubGrid;
  selectedModelIds: Array<number> = [];
  selectedModels: Array<any> = [];
  blendedModelsItems: Array<any> = [];
  refreshGrid: boolean = false;
  showModelSidebar: boolean = false;
  showBlendModelSidebar: boolean = false;
  showBulkEditSidebar: boolean = false;
  modelViewId: number = 0;
  IsAdmin: boolean = false;
  
  get delMsg() {
    let len = this.selectedModelIds.length;
    let text = len == 1 ? " Model" : " Models";
    return len + text;
  }
  constructor(private modelService: ModelService, private authService: AuthenticationService, private messageService: AuthMessageService, private matDialog: MatDialog) { }

  ngOnInit(): void {
    let login = this.authService.getLoggedInUserInfo;
    if (login)
      this.IsAdmin = login.isAdmin;
  }

  gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName) {
      this.selectedModelIds = gridSelect.SelectedKeys;
      this.selectedModels = gridSelect.SelectedRows;
    }
  }

  gridRowsDblClickedCallback(gridRowDblClk: DataGridRowDblClickModel) {
    if (gridRowDblClk.Grid == this.gridName && gridRowDblClk.Key && this.IsAdmin == true)
      this.viewModelSidebar(gridRowDblClk.Key);
  }

  addModel() {
    if (this.IsAdmin)
      this.viewModelSidebar(0);
    else
      this.messageService.showErrorPopup({ message: Constants.AdminOnlyPermissionMsg });
  }

  viewModel() {
    if (this.IsAdmin) {
      if (this.selectedModelIds.length === 1)
        this.viewModelSidebar(this.selectedModelIds[0]);
    } else
      this.messageService.showErrorPopup({ message: Constants.AdminOnlyPermissionMsg });
  }

  private viewModelSidebar(modelId: number) {
    this.modelViewId = modelId;
    this.showSidebar();
  }

  deleteModels() {
    const dialogRef = this.matDialog.open(ConfirmationPopupComponent, {
      width: Constants.DeleteDiaogWidth,
      disableClose: Constants.DiaogDisableClose,
      data: { Header: "Delete Confirmation!", Message: "Are you sure? You want to delete " + this.delMsg }
    });

    dialogRef.afterClosed().subscribe((isSuccess: any) => {
      if (isSuccess)
        this.delModelsCallBack();
    });

  }
  delModelsCallBack() {
    if (this.IsAdmin) {
      if (this.selectedModelIds.length) {
        this.modelService.deleteModels(this.selectedModelIds)
          .subscribe((response: any) => {
            if (response['success'] == Constants.SuccessResponse) {
              this.messageService.showSuccessPopup({ message: response['message'] });
              this.refreshDataGrid();
            }
          });
      }
    } else
      this.messageService.showErrorPopup({ message: Constants.AdminOnlyPermissionMsg });
  }

  blendModels() {
    if (this.IsAdmin) {
      if (this.selectedModels.length) {
        if (this.selectedModels.filter(m => m.isBlendModel).length)
          this.messageService.showErrorPopup({ message: "Blended Model can't be Blended again" });
        else {
          this.blendedModelsItems = this.selectedModels;
          this.showBlendSidebar();
        }
      }
    } else
      this.messageService.showErrorPopup({ message: Constants.AdminOnlyPermissionMsg });
  }

  private showSidebar() {
    this.showModelSidebar = true;
  }

  private closeSidebar() {
    this.showModelSidebar = false;
  }

  modelSidebarCallback(isSuccess: boolean) {
    if (isSuccess)
      this.refreshDataGrid();
    this.closeSidebar();
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
    this.selectedModels = this.selectedModelIds = [];
    this.refreshGrid = !this.refreshGrid;
  }

  bulkEditModels() {
    if (this.IsAdmin) {
      if (this.selectedModelIds.length > 1) {
        if (this.selectedModels.filter(m => m.isBlendModel).length)
          this.messageService.showErrorPopup({ message: "Blended Model can't be Bulk Edited!" });
        else
          this.showBulkSidebar();
      }
    } else
      this.messageService.showErrorPopup({ message: Constants.AdminOnlyPermissionMsg });
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

}
