import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ModelService } from '../../../services/ModelService/model.service';
import { Constants } from '../../../support/constants/constants';
import { GridRowSelectionModel } from '../../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { DataGridNames } from '../../../support/enums/data-grid.enum';
import { Utility } from '../../../support/utility/utility';
import { AuthMessageService } from '../../../services/AuthMessageService/auth-message.service';

@Component({
  selector: 'zega-strategy-sidebar',
  templateUrl: './strategy-sidebar.component.html',
  styleUrls: ['./strategy-sidebar.component.less']
})
export class StrategySidebarComponent implements OnInit {
  
  @Input() strategyId: number = 0;
  @Output() confirmCallback: EventEmitter<boolean> = new EventEmitter();
  @Output() cancelCallback: EventEmitter<boolean> = new EventEmitter();
  strategyForm: FormGroup;
  gridName: DataGridNames = DataGridNames.ModelListingSidebar;
  refreshGrid: boolean = false;
  loadGrid: boolean = false;
  selectedModels: Array<any> = [];
  deSelectRowsManually: Array<any> = []

  constructor(private fb: FormBuilder, private modelService: ModelService, private messageService: AuthMessageService) {
    this.strategyForm = this.fb.group({
      Name: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
      Description: new FormControl(null, [Validators.required, Validators.maxLength(250)])
    });
  }

  ngOnInit(): void {
    Utility.setSidebarContentHeight();
    if (this.strategyId) {
      this.modelService.getStrategy(this.strategyId)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            var retData = response['response'];
            this.strategyForm.get('Name')?.setValue(retData.name);
            this.strategyForm.get('Description')?.setValue(retData.description);
            this.selectedModels = retData.models;
            this.loadGrid = true;
          }
        });
    } else
        this.loadGrid = true;
  }

  public gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.gridName) {
      this.selectedModels = gridSelect.SelectedRows;
    }
  }

  removeSelectedModel(model: any) {
    let index = this.selectedModels.findIndex(it => it === model);
    if (index > -1) {
      this.deSelectRowsManually = [this.selectedModels[index]];
      this.selectedModels.splice(index, 1);
    }
  }

  cancel() {
    this.cancelCallback.emit(false);
  }

  submitStrategyForm() {
    Utility.cleanForm(this.strategyForm);
    if (this.strategyForm.valid) {
      let data = this.strategyForm.value;
      data.Id = this.strategyId;
      data.Models= this.selectedModels;
      this.modelService.saveStrategy(data)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.confirmCallback.emit(true);
          }
        });
    }
  }

}
