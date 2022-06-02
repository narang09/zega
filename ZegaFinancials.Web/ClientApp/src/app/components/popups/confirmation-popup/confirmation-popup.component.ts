import { Component, Inject, Input, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'zega-confirmation-popup',
  templateUrl: './confirmation-popup.component.html',
  styleUrls: ['./confirmation-popup.component.less']
})
export class ConfirmationPopupComponent implements OnInit {

  header: string = 'Confirmation Popup';
  message: string = 'Are you sure you want to proceed ?';
  
  constructor(private dialogRef: MatDialogRef<ConfirmationPopupComponent>, @Inject(MAT_DIALOG_DATA) private dialogData: any) { }

  ngOnInit(): void {
    if (this.dialogData.Header)
      this.header = this.dialogData.Header;
    if (this.dialogData.Message)
      this.message = this.dialogData.Message;
  }

}
