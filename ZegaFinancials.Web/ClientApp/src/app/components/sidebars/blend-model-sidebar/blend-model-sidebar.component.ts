import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { DataGridEditModel } from '../../../models/DataGrid/DataGridEditModel/data-grid-edit-model';
import { GridRowSelectionModel } from '../../../models/DataGrid/DataGridRowSelectionModel/grid-row-selection-model';
import { AuthMessageService } from '../../../services/AuthMessageService/auth-message.service';
import { ModelService } from '../../../services/ModelService/model.service';
import { Constants } from '../../../support/constants/constants';
import { DataGridNames } from '../../../support/enums/data-grid.enum';
import { Utility } from '../../../support/utility/utility';

@Component({
  selector: 'zega-blend-model-sidebar',
  templateUrl: './blend-model-sidebar.component.html',
  styleUrls: ['./blend-model-sidebar.component.less']
})
export class BlendModelSidebarComponent implements OnInit {
  @Input() subModels: Array<any> = [];
  @Input() accountId: number = 0;
  @Output() confirmCallback: EventEmitter<boolean> = new EventEmitter();
  @Output() cancelCallback: EventEmitter<boolean> = new EventEmitter();

  blendModelForm: FormGroup;
  blendModelGridName: DataGridNames = DataGridNames.ModelListingSubGrid;
  refreshBlendModelGrid: boolean = false;
  selectedModelIds: Array<number> = [];

  get AlloctionsTotal() {
    let alloc: number = 0;
    this.subModels.forEach(sl => alloc += sl.allocationUI);
    return alloc;
  }

  constructor(private fb: FormBuilder, private modelService: ModelService, private messageService: AuthMessageService) {
    this.blendModelForm = this.fb.group({
      Description: new FormControl('', [Validators.required, Validators.maxLength(250)]),
    });

  }

  ngOnInit(): void {
    Utility.setSidebarContentHeight();
    let key = 'id'
    this.subModels.sort((a: any, b: any) => {
      var x = a[key]; var y = b[key];
      return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    })
    this.subModels.forEach(m => m.allocation = m.allocationUI = 0);
  }

  cancel() {
    this.cancelCallback.emit(false);
  }

  get getBlendedModelName() {
    let parts: Array<string> = [];
    this.subModels.forEach(m => parts.push(m.name, m.allocationUI))
    return parts.join('-');
  }

  submitBlendModelForm() {
    Utility.cleanForm(this.blendModelForm);
    if (this.blendModelForm.valid) {
      if (this.subModels.length <= 1)
        this.messageService.showErrorPopup({ message: 'Please add at least 2 Models to Blend' });
      else if (this.AlloctionsTotal !== 100)
        this.messageService.showErrorPopup({ message: 'Sum of all Allocations should be 100%' });
      else {
        let data = this.blendModelForm.value;
        data.ModelItems = this.subModels;
        data.Name = this.getBlendedModelName;
        data.IsBlendModel = true;
        data.AccountId = this.accountId;
        this.modelService.saveModel(data)
          .subscribe((response: any) => {
            if (response['success'] == Constants.SuccessResponse) {
              this.messageService.showSuccessPopup({ message: response['message'] });
              this.confirmCallback.emit(true);
            }
          });

      }
    }
  }

  private refreshDataGrid() {
    this.refreshBlendModelGrid = !this.refreshBlendModelGrid;
  }

  getSubModels = (gridModel: any) => {
    return new Promise(resolveCallback => {
      var data = {
        success: true,
        response: {
          totalRecords: this.subModels.length,
          gridData: this.subModels
        }
      }
      resolveCallback(data);
    });
  }

  editGridCallback = (gridEditModel: DataGridEditModel) => {
    var editRow = this.subModels.find(sl => sl.id == gridEditModel.EditRowKey);
    if (gridEditModel.UpdatedValues) {
      Object.entries(gridEditModel.UpdatedValues).forEach(([key, value]: [any, any]) => {
        editRow[key] = value;
        if (key === "allocationUI")
          editRow['allocation'] = Number(value / 100).toPrecision(5);
      })
    }
    return new Promise(resolveCallback => {
      resolveCallback(editRow);
    });
  }

  gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.blendModelGridName)
      this.selectedModelIds = gridSelect.SelectedKeys;
  }

  deleteModels() {
    if (this.subModels.length - this.selectedModelIds.length > 1) {
      this.subModels = this.subModels.filter(sl => !this.selectedModelIds.includes(sl.id))
      this.refreshDataGrid();
    } else
      this.messageService.showErrorPopup({ message: 'There should be at least 2 Models to Blend' });
  }

}
