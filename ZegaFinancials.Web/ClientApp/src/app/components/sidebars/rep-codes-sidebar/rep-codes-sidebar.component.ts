
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { GridRowSelectionModel } from '../../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { AuthMessageService } from '../../../services/AuthMessageService/auth-message.service';
import { UserService } from '../../../services/User/user.service';
import { Constants } from '../../../support/constants/constants';
import { DataGridNames } from '../../../support/enums/data-grid.enum';
import { RepCodeType } from '../../../support/enums/user.enum';
import { Utility } from '../../../support/utility/utility';

@Component({
  selector: 'zega-rep-codes-sidebar',
  templateUrl: './rep-codes-sidebar.component.html',
  styleUrls: ['./rep-codes-sidebar.component.less']
})
export class RepCodesSidebarComponent implements OnInit {

  @Input() selectedRepCodes: Array<any> = [];
  @Output() confirmCallback: EventEmitter<GridRowSelectionModel> = new EventEmitter();
  @Output() cancelCallback: EventEmitter<boolean> = new EventEmitter();

  gridName: DataGridNames = DataGridNames.RepCodeListing;
  refreshGrid: boolean = false;
  deSelectRowsManually: Array<any> = []

  constructor() {  }

  ngOnInit(): void {
    Utility.setSidebarContentHeight();
  }

  public gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName)
      this.selectedRepCodes = gridSelect.SelectedRows;
  }

  public selecRepCodes() {
    var selectionModel = new GridRowSelectionModel();
    selectionModel.Grid = this.gridName;
    selectionModel.Key = 'id';
    selectionModel.SelectedRows = this.selectedRepCodes;
    this.confirmCallback.emit(selectionModel);
  }

  public cancel() {
    this.cancelCallback.emit(false);
  }

  removeSelectedRepCode(repCode: any) {
    let index = this.selectedRepCodes.findIndex(it => it === repCode);
    if (index > -1) {
      this.deSelectRowsManually = [this.selectedRepCodes[index]];
      this.selectedRepCodes.splice(index, 1);
    }
  }

}
