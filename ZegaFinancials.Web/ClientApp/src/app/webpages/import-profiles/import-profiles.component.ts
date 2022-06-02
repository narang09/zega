import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatOption } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSelect } from '@angular/material/select';
import { RepCodeBatchPopupComponent } from '../../components/popups/rep-code-batch-popup/rep-code-batch-popup.component';
import { Import } from '../../models/Import/import';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { ImportService } from '../../services/Import/import.service';
import { Constants } from '../../support/constants/constants';
import { Utility } from '../../support/utility/utility';
import { ZegaValidators } from '../../support/validators/validators';

@Component({
  selector: 'zega-import-profiles',
  templateUrl: './import-profiles.component.html',
  styleUrls: ['./import-profiles.component.less']
})
export class ImportProfilesComponent implements OnInit {

  private profileId: number = 0;
  AllRepCodes: Array<any> = [];
  allRepCodesSelected: boolean = false;
  repCodeBatches: Array<any> = [];
  importForm: FormGroup;
  selectedRepCodeBatch: number = -1;

  ngOnInit(): void {
    this.getRepCodeDropDownData();
  }

  constructor(private fb: FormBuilder, private importService: ImportService, private messageService: AuthMessageService, private matDialog: MatDialog) {
    this.importForm = this.fb.group({
      Name: new FormControl('', [Validators.required]),
      AutoImport: new FormControl(false, []),
      SchedulerImportTime: new FormControl(null, []),
      RepCodes: new FormControl([], []),
      Login: new FormControl('', [Validators.required]),
      Password: new FormControl('', [Validators.required]),
    }, {
      validator: [ZegaValidators.conditionallyRequiredValidator('AutoImport', true, 'SchedulerImportTime'),]
    });
  }


  private getRepCodeDropDownData() {
    this.importService.getRepCodeDropdownData()
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          let data: Array<any> = response['response'];
          if (data && data.length)
            this.AllRepCodes = data;
          this.getImportProfileData();
        }
      });
  }


  private getImportProfileData() {
    this.importService.getImportData()
      .subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          let data = response['response'];
          this.importForm.patchValue({
            Name: data.name,
            AutoImport: data.autoImport,
            SchedulerImportTime: data.schedulerImportTime,
            Login: data.login,
            RepCodes: data.repCodes
          });
          this.profileId = data.id;
          if (data.repCodes && data.repCodes.length === this.AllRepCodes.length)
            this.allRepCodesSelected = true;
          if (!data.autoImport)
            this.importForm.get('SchedulerImportTime')?.disable();
          if (data.batches && data.batches.length)
            this.repCodeBatches = this.setRepCodeBatch(data.batches);
        }
      });
  }

  onRepCodeChanged(event: any) {
    this.allRepCodesSelected = event.value && event.value.length === this.AllRepCodes.length;
  }

  submitImportForm() {
    Utility.cleanForm(this.importForm);
    if (this.importForm.valid) {
      let importModel: Import = Object.assign({}, this.importForm.value);
      importModel.Batches = this.getRepCodesList();
      importModel.Id = this.profileId;
      this.importService.saveImportProfile(importModel)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.profileId = response['response'];
            this.messageService.showSuccessPopup({ message: response['message'] });
          }
        });
    }
  }

  private getRepCodesList() {
    let processedBatch: Array<Array<number>> = [];
    this.repCodeBatches.forEach(bat => processedBatch.push(bat.map((rc: any) => rc.id)));
    return processedBatch;
  }

  private setRepCodeBatch(batches: Array<Array<number>>) {
    let processedBatch: Array<Array<any>> = [];
    batches.forEach(bat => processedBatch.push(this.AllRepCodes.filter((rc: any) => bat.includes(rc.id))));
    return processedBatch;
  }

  runManualImport() {
    if (this.importForm.valid) {
      this.importService.runImport(this.profileId)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse)
            this.messageService.showSuccessPopup({ message: response['message'] });
        });
    }
  }

  toggleAllSelection(event: any) {
    if (event.checked)
      this.importForm.get('RepCodes')?.setValue(this.AllRepCodes.map(it => it.id));
    else
      this.importForm.get('RepCodes')?.setValue([]);
  }

  onAutoImportChange(event: any) {
    if (event.checked)
      this.importForm.controls["SchedulerImportTime"]?.enable();
    else
      this.importForm.get('SchedulerImportTime')?.disable();
  }

  addRepCodesBatch() {
    this.resetSelectedRepCode();
    let unbatchedRepCodes = this.getUnMappedRepCodes();
    if (unbatchedRepCodes.length) {
      const dialogRef = this.matDialog.open(RepCodeBatchPopupComponent, {
        panelClass: 'set-padding',
        width: '40%',
        data: { 'AvailableRepCodes': unbatchedRepCodes, 'SelectedRepCodeIds': [] }
      });
      dialogRef.afterClosed().subscribe(this.repCodeBatchAdded);
    } else
      this.messageService.showErrorPopup({ message: "No RepCodes left to batch!" });
  }

  editRepCodesBatch() {
    if (this.selectedRepCodeBatch > -1) {
      var repCodeIdsToEdit = this.repCodeBatches[this.selectedRepCodeBatch];
      let unbatchedRepCodes = this.getUnMappedRepCodes();
      unbatchedRepCodes.push(...repCodeIdsToEdit);
      if (unbatchedRepCodes.length) {
        const dialogRef = this.matDialog.open(RepCodeBatchPopupComponent, {
          panelClass: 'set-padding',
          width: '40%',
          data: { 'AvailableRepCodes': unbatchedRepCodes, 'SelectedRepCodeIds': repCodeIdsToEdit }
        });
        dialogRef.afterClosed().subscribe(this.repCodeBatchEditCallback);
      } else
        this.messageService.showErrorPopup({ message: "No RepCodes left to batch!" });
    }

  }

  private getUnMappedRepCodes() {
    let batchedRepCodeIds: Array<number> = this.repCodeBatches.reduce((accumulator, value) => accumulator.concat(...value), []).map((rc: any) => rc.id);
    return this.AllRepCodes.filter(rc => !batchedRepCodeIds.includes(rc.id));
  }

  repCodesBatchClicked(index: number) {
    this.selectedRepCodeBatch = index;
  }

  private repCodeBatchAdded = (result: Array<any>) => {
    if (result && result.length)
      this.repCodeBatches.push(result);
    this.resetSelectedRepCode();
  }

  private repCodeBatchEditCallback = (result: Array<any>) => {
    if (result && result.length && this.selectedRepCodeBatch > -1)
      this.repCodeBatches[this.selectedRepCodeBatch] = result;
    this.resetSelectedRepCode();
  }

  removeAutoImportBatch(index: number) {
    this.repCodeBatches.splice(index, 1);
    this.resetSelectedRepCode();
  }

  getBatchRepCodes(batch: Array<any>) {
    return batch.map(rp => rp.name).join(", ");
  }

  private resetSelectedRepCode() {
    this.selectedRepCodeBatch = -1;
  }

}
