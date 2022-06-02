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
  selector: 'zega-model-details-sidebar',
  templateUrl: './model-details-sidebar.component.html',
  styleUrls: ['./model-details-sidebar.component.less']
})
export class ModelDetailsSidebarComponent implements OnInit {

  @Input() modelId: number = 0;
  @Output() confirmCallback: EventEmitter<boolean> = new EventEmitter();
  @Output() cancelCallback: EventEmitter<boolean> = new EventEmitter();

  modelForm: FormGroup;
  isBlendModel: boolean = false;
  strategiesDropdown: Array<any> = [];
  sleevesDropdown: Array<any> = [];
  private modelSleeves: Array<any> = [];
  modelSubGridName: DataGridNames = DataGridNames.ModelListingSubGrid;
  refreshModelSleevGrid: boolean = false;
  selectedModelSleeveIds: Array<number> = [];
  private isSleeveUpdated: boolean = false;

  get AlloctionsTotal() {
    let alloc: number = 0;
    this.modelSleeves.forEach(sl => alloc += sl.allocationUI);
    return alloc;
  }

  constructor(private fb: FormBuilder, private modelService: ModelService, private messageService: AuthMessageService) {
    this.modelForm = this.fb.group({
      Name: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
      Description: new FormControl(null, [Validators.required, Validators.maxLength(250)]),
      Strategies: new FormControl([], []),
      UISleeves: new FormControl([], [])
    });
  }

  ngOnInit(): void {
    Utility.setSidebarContentHeight(20); // Header in this case is bsed on component api response
    if (this.modelId) {
      this.modelService.getModel(this.modelId)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            let retData = response['response'];
            this.modelForm.get('Name')?.setValue(retData.name);
            this.modelForm.get('Description')?.setValue(retData.description);
            this.modelForm.get('Strategies')?.setValue(retData.strategies);
            this.modelSleeves = retData.modelItems ?? [];
            this.isBlendModel = retData.isBlendModel ?? false;
            if (this.isBlendModel)
              this.modelForm.get('Name')?.disable();
          }
        });
    }

    this.modelService.getModelDropdown()
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          let retData = response['response'];
          this.sleevesDropdown = retData.sleeves ?? [];
          this.strategiesDropdown = retData.strategies ?? [];
        }
      });

  }

  cancel() {
    this.cancelCallback.emit(false);
  }

  submitModelForm() {
    Utility.cleanForm(this.modelForm);
    if (this.modelForm.valid) {
      if (!this.modelSleeves.length)
        this.messageService.showErrorPopup({ message: 'Please add Sleeves to this Model' });
      else if (this.AlloctionsTotal !== 100)
        this.messageService.showErrorPopup({ message: 'Sum of all Model Items should be 100%' });
      else {
        let data = this.modelForm.value;
        if (this.isBlendModel)
          data.Name = this.getBlendedModelName();
        data.Id = this.modelId;
        data.ModelItems = this.modelSleeves;
        data.IsBlendModel = this.isBlendModel;
        data.IsSleeveUpdated = this.isSleeveUpdated;
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
    this.refreshModelSleevGrid = !this.refreshModelSleevGrid;
  }

  addSleeve() {
    let sleevesToAdd: Array<any> = this.modelForm.get("UISleeves")?.value;
    if (sleevesToAdd && sleevesToAdd.length) {
      let alreadyExistNames: Array<string> = [];
      sleevesToAdd.forEach((sleeve: any) => {
        if (!this.modelSleeves.find(sl => sl.id === sleeve.id)) {
          this.modelSleeves.push({
            id: sleeve.id,
            name: sleeve.name,
            description: sleeve.description,
            allocation: 0,
            allocationUI: 0
          });
          this.isSleeveUpdated = true;
        }
        else
          alreadyExistNames.push(sleeve.name);
      })
      if (alreadyExistNames.length)
        this.messageService.showErrorPopup({ message: `Sleeve "${alreadyExistNames.join(', ')} " already exists ` });
      this.refreshDataGrid();
      this.modelForm.get("UISleeves")?.setValue([]);
    }
  }

  getModelSleeves = (gridModel: any) => {
    return new Promise(resolveCallback => {
      var data = {
        success: true,
        response: {
          totalRecords: this.modelSleeves.length,
          gridData: this.modelSleeves
        }
      }
      resolveCallback(data);
    });
  }

  editGridCallback = (gridEditModel: DataGridEditModel) => {
    var editRow = this.modelSleeves.find(sl => sl.id == gridEditModel.EditRowKey);
    if (gridEditModel.UpdatedValues) {
      Object.entries(gridEditModel.UpdatedValues).forEach(([key, value]: [any, any]) => {
        editRow[key] = value;
        if (key === "allocationUI")
          editRow['allocation'] = Number(value / 100).toPrecision(5);
        this.isSleeveUpdated = true;
      })
      if (this.isBlendModel)
        this.modelForm.get('Name')?.setValue(this.getBlendedModelName());
    }
    return new Promise(resolveCallback => {
      resolveCallback(editRow);
    });
  }

  private getBlendedModelName() {
    let parts: Array<string> = [];
    this.modelSleeves.forEach(m => parts.push(m.name, m.allocationUI))
    return parts.join('-');
  }


  gridRowsSelectedCallback(gridSelect: GridRowSelectionModel) {
    if (gridSelect.Grid == this.modelSubGridName)
      this.selectedModelSleeveIds = gridSelect.SelectedKeys;
  }

  deleteSleeves() {
    this.modelSleeves = this.modelSleeves.filter(sl => !this.selectedModelSleeveIds.includes(sl.id))
    this.isSleeveUpdated = true;
    this.refreshDataGrid();
  }

}
