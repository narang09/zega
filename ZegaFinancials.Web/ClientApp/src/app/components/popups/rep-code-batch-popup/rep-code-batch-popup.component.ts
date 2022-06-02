import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthMessageService } from '../../../services/AuthMessageService/auth-message.service';

@Component({
  selector: 'zega-rep-code-batch-popup',
  templateUrl: './rep-code-batch-popup.component.html',
  styleUrls: ['./rep-code-batch-popup.component.less']
})
export class RepCodeBatchPopupComponent implements OnInit {

  repCodesForm: FormGroup;
  isUpdate: boolean;
  ALLRopCodes: Array<any>;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private fb: FormBuilder, private dialogRef: MatDialogRef<RepCodeBatchPopupComponent>, private messageService: AuthMessageService) {
    this.repCodesForm = this.fb.group({
      RepCodes: new FormControl([], [])
    });
    this.ALLRopCodes = data.AvailableRepCodes;
    this.isUpdate = data.SelectedRepCodeIds && data.SelectedRepCodeIds.length 
    this.repCodesForm.patchValue({ "RepCodes": data.SelectedRepCodeIds ?? [] });
  }

  ngOnInit(): void { }

  submitRepCodesForm() {
    var selectedRepCodes = this.repCodesForm.get("RepCodes")?.value;
    if (selectedRepCodes && selectedRepCodes.length) {
      console.log(selectedRepCodes);
      this.dialogRef.close(selectedRepCodes);
    } else
      this.messageService.showErrorPopup({ message: "Select at least one Rep Code" });
  }

}
