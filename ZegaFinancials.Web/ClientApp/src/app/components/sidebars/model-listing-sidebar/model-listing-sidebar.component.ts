import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { GridRowSelectionModel } from '../../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { DataGridNames } from '../../../support/enums/data-grid.enum';
import { Utility } from '../../../support/utility/utility';

@Component({
  selector: 'zega-model-listing-sidebar',
  templateUrl: './model-listing-sidebar.component.html',
  styleUrls: ['./model-listing-sidebar.component.less']
})
export class ModelListingSidebarComponent implements OnInit {

  @Input() selectedModels: Array<any> = [];
  
  @Output() confirmCallback: EventEmitter<GridRowSelectionModel> = new EventEmitter();
  @Output() cancelCallback: EventEmitter<boolean> = new EventEmitter();

  gridName: DataGridNames = DataGridNames.ModelListingSidebar;
  refreshGrid: boolean = false;
  deSelectRowsManually: Array<any> = []

  constructor() { }

  ngOnInit(): void {
    Utility.setSidebarContentHeight();
  }

  public gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName) 
      this.selectedModels = gridSelect.SelectedRows;
  }

  public selectModels() {
    var selectionModel = new GridRowSelectionModel();
    selectionModel.Grid = this.gridName;
    selectionModel.Key = 'id';
    selectionModel.SelectedRows = this.selectedModels;
    this.confirmCallback.emit(selectionModel);
  }

  public cancel() {
    this.cancelCallback.emit(false);
  }

  removeSelectedModel(model: any) {
    let index = this.selectedModels.findIndex(it => it === model);
    if (index > -1) {
      this.deSelectRowsManually = [this.selectedModels[index]];
      this.selectedModels.splice(index, 1);
    }
  }

}
