import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs/internal/Subscription';

import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { ResponseVO } from '../../models/ResponseVO/response-vo';

@Component({
  selector: 'zega-auth-message',
  templateUrl: './auth-message.component.html',
  styleUrls: ['./auth-message.component.less']
})
export class AuthMessageComponent implements OnInit {

  responseDataObj = new ResponseVO();
  showErrorDescription: boolean = false;
  showErrorPopup: boolean = false;
  showSuccessPopup: boolean = false;

  private subscription: Subscription = new Subscription;

  constructor(private authMessageService: AuthMessageService) { }

  ngOnInit() {
    this.subscription = this.authMessageService.authMessageState
      .subscribe((stateObj: any) => {
        this.showErrorPopup = stateObj.showErrorPopup;
        this.showSuccessPopup = stateObj.showSuccessPopup;
        this.responseDataObj = stateObj['responseDataObj'] || {};
        this.showErrorDescription = false;
      });
  }

  /* Show More Details in Error Message Popup */
  showMoreOrLessErrorDetails() {
    this.showErrorDescription = !this.showErrorDescription;
  }

  /* Hide Errror Message Popup */
  hideErrorMessagePopup() {
    this.authMessageService.hideMessagePopup();
  }

  /* Hide Success Message Popup */
  hideSuccessMessagePopup() {
    this.authMessageService.hideMessagePopup();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

}
