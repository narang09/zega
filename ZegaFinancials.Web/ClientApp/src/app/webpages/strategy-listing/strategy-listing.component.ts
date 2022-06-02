import { Component, OnInit } from '@angular/core';
import { DataGridRowDblClickModel } from '../../models/DataGrid/DataGridRowDBLClickModel/data-grid-row-dbl-click-model';
import { GridRowSelectionModel } from '../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { ModelService } from '../../services/ModelService/model.service';
import { Constants } from '../../support/constants/constants';
import { DataGridNames } from '../../support/enums/data-grid.enum';
import { ConfirmationPopupComponent } from '../../components/popups/confirmation-popup/confirmation-popup.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'zega-strategy-listing',
  templateUrl: './strategy-listing.component.html',
  styleUrls: ['./strategy-listing.component.less']
})
export class StrategyListingComponent implements OnInit {

  gridName: DataGridNames = DataGridNames.StrategyListing;
  selectedStrategyIds: Array<number> = [];
  selectedStrategies: Array<any> = [];
  refreshGrid: boolean = false;
  showStrategySidebar: boolean = false;
  strategyView: number = 0;

  get delMsg() {
    let len = this.selectedStrategyIds.length;
    let text = len == 1 ? " Strategy" : " Strategies";
    return len + text;
  }

  constructor(private modelService: ModelService, private messageService: AuthMessageService, private matDialog: MatDialog) { }
  ngOnInit(): void { }

  gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName) {
      this.selectedStrategyIds = gridSelect.SelectedKeys;
      this.selectedStrategies = gridSelect.SelectedRows;
    }
  }

  gridRowsDblClickedCallback(gridRowDblClk: DataGridRowDblClickModel) {
    if (gridRowDblClk.Grid == this.gridName && gridRowDblClk.Key)
      this.viewStrategySidebar(gridRowDblClk.Key);
  }

  private viewStrategySidebar(id: number) {
    this.strategyView = id;
    this.showSidebar();
  }

  addStrategy() {
    this.viewStrategySidebar(0);
  }

  viewStrategy() {
    if (this.selectedStrategyIds.length === 1) 
      this.viewStrategySidebar(this.selectedStrategyIds[0]);
  }

  deleteStrategies() {
    const dialogRef = this.matDialog.open(ConfirmationPopupComponent, {
      width: Constants.DeleteDiaogWidth,
      disableClose: Constants.DiaogDisableClose, 
      data: { Header: "Delete Confirmation!", Message: "Are you sure? You want to delete " + this.delMsg }
    });

    dialogRef.afterClosed().subscribe((isSuccess: any) => {
      if (isSuccess)
        this.delStrategiesCallBack();
    });
  }
  private delStrategiesCallBack() {
    if (this.selectedStrategyIds.length) {
      this.modelService.deleteStrategies(this.selectedStrategyIds)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.refreshDataGrid();
          }
        });
    }
  }
  private showSidebar() {
    this.showStrategySidebar = true;
  }

  private closeSidebar() {
    this.showStrategySidebar = false;
  }

  strategySidebarCallback(isSuccess: boolean) {
    if (isSuccess)
      this.refreshDataGrid();
    this.closeSidebar();
  }

  refreshDataGrid() {
    this.selectedStrategies = this.selectedStrategyIds = [];
    this.refreshGrid = !this.refreshGrid;
  }

}
