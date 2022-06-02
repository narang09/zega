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
  selector: 'zega-sleeve-listing',
  templateUrl: './sleeve-listing.component.html',
  styleUrls: ['./sleeve-listing.component.less']
})
export class SleeveListingComponent implements OnInit {

  gridName: DataGridNames = DataGridNames.SleeveListing;
  selectedSleeves: Array<any> = [];
  refreshGrid: boolean = false;
  showSleeveSidebar: boolean = false;
  sleeveViewId: number = 0;
  
  get delMsg() {
    let len = this.selectedSleeves.length;
    let text = len == 1 ? " Sleeve" : " Sleeves";
    return len + text;
  }
  constructor(private modelService: ModelService, private messageService: AuthMessageService, private matDialog: MatDialog) { }

  ngOnInit(): void { }

  gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName)
      this.selectedSleeves = gridSelect.SelectedRows;
  }

  gridRowsDblClickedCallback(gridRowDblClk: DataGridRowDblClickModel) {
    if (gridRowDblClk.Grid == this.gridName && gridRowDblClk.Key)
      this.viewSleeveSidebar(gridRowDblClk.Key);
  }

  private viewSleeveSidebar(id: number) {
    this.sleeveViewId = id;
    this.showSidebar();
  }

  addSleeves() {
    this.viewSleeveSidebar(0);
  }

  viewSleeve() {
    if (this.selectedSleeves.length === 1) 
      this.viewSleeveSidebar(this.selectedSleeves[0].id);
  }

  deleteSleeves() {
    const dialogRef = this.matDialog.open(ConfirmationPopupComponent, {
      width: Constants.DeleteDiaogWidth,
      disableClose: Constants.DiaogDisableClose,
      data: { Header: "Delete Confirmation!", Message: "Are you sure? You want to delete " + this.delMsg }
    });

    dialogRef.afterClosed().subscribe((isSuccess: any) => {
      if (isSuccess)
        this.delSleevesCallBack();
    });
  }
  private delSleevesCallBack() {
    if (this.selectedSleeves.length) {
      let selectedKeys = this.selectedSleeves.map((sr: any) => sr.id)
      this.modelService.deleteSleeeves(selectedKeys)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.refreshDataGrid();
          }
        });
    }
  }

  private showSidebar() {
    this.showSleeveSidebar = true;
  }

  private closeSidebar() {
    this.showSleeveSidebar = false;
  }

  sleeveSidebarCallback(isSuccess: boolean) {
    if (isSuccess)
      this.refreshDataGrid();
    this.closeSidebar();
  }

  refreshDataGrid() {
    this.selectedSleeves = [];
    this.refreshGrid = !this.refreshGrid;
  }

}
