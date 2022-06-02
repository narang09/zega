import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/internal/Subject';
import { AuthMessageState } from '../../models/AuthMessageState/auth-message-state';

@Injectable({
  providedIn: 'root'
})
export class AuthMessageService {

  constructor() {
    AuthMessageService.instance = this;//this is only for the accessing provide out of root scope
  }

  static instance: AuthMessageService;// this is use to access out of root scope
  private AuthMessageSubject = new Subject<AuthMessageState>();
  authMessageState = this.AuthMessageSubject.asObservable();

  showErrorPopup(responseDataObj: any) {
    this.AuthMessageSubject.next(<AuthMessageState>{ showErrorPopup: true, showSuccessPopup: false, responseDataObj });
    this.autoHideMessagePopup();
  }

  showSuccessPopup(responseDataObj: any) {
    this.AuthMessageSubject.next(<AuthMessageState>{ showErrorPopup: false, showSuccessPopup: true, responseDataObj });
    this.autoHideMessagePopup();
  }

  hideMessagePopup() {
    this.AuthMessageSubject.next(<AuthMessageState>{ showErrorPopup: false, showSuccessPopup: false });
  }

  private autoHideMessagePopup() {
    setTimeout(() => {
      this.hideMessagePopup();
    }, 4000)
  }

}
