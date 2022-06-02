import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthMessageService } from '../../../services/AuthMessageService/auth-message.service';
import { ModelService } from '../../../services/ModelService/model.service';
import { Constants } from '../../../support/constants/constants';
import { Utility } from '../../../support/utility/utility';

@Component({
  selector: 'zega-sleeve-sidebar',
  templateUrl: './sleeve-sidebar.component.html',
  styleUrls: ['./sleeve-sidebar.component.less']
})
export class SleeveSidebarComponent implements OnInit {

  @Input() sleeveId: number;
  @Output() confirmCallback: EventEmitter<boolean> = new EventEmitter();
  @Output() cancelCallback: EventEmitter<boolean> = new EventEmitter();
  sleeveForm: FormGroup;

  constructor(private fb: FormBuilder, private modelService: ModelService, private messageService: AuthMessageService) {
    this.sleeveForm = this.fb.group({
      Name: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
      Description: new FormControl(null, [Validators.required, Validators.maxLength(250)])
    });
    this.sleeveId = 0;
  }

  ngOnInit(): void {
    Utility.setSidebarContentHeight();
    if (this.sleeveId) {
      this.modelService.getSleeve(this.sleeveId)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            let retData = response['response'];
            this.sleeveForm.get('Name')?.setValue(retData.name);
            this.sleeveForm.get('Description')?.setValue(retData.description);
          }
        });
    }
  }

  cancel() {
    this.cancelCallback.emit(false);
  }

  submitSleeveForm() {
    Utility.cleanForm(this.sleeveForm);
    if (this.sleeveForm.valid) {
      let data = this.sleeveForm.value;
      data.Id = this.sleeveId;
      this.modelService.saveSleeve(data)
        .subscribe((response: any) => {
          if (response['success'] == Constants.SuccessResponse) {
            this.messageService.showSuccessPopup({ message: response['message'] });
            this.confirmCallback.emit(true);
          }
        });
    }
  }

}
