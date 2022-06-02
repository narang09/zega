import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiURLs } from '../../support/apiURLs/api-urls';
@Injectable({
  providedIn: 'root'
})
export class AccountsService {

  constructor(private http: HttpClient) { }

  addAccountDropdown() {
    return this.http.get(ApiURLs.addAccountDropdownAPI)
      .pipe(response => response);
  }
  addaccount(data: Object) {
    return this.http.post(ApiURLs.addAccountAPI, data)
      .pipe(response => response);
  }
  getAccount(userId: number) {
    return this.http.post(ApiURLs.getAccountAPI, userId)
      .pipe(response => response);
  }
  getAdvisorsByRepCode(repCodeId: number) {
    return this.http.post(ApiURLs.getAdvisorsByRepCodeAPI, repCodeId)
      .pipe(response => response);
  }
  deleteAccount(AccountIds: Array<number>) {
    return this.http.post(ApiURLs.deleteAccountAPI, AccountIds)
      .pipe(response => response);
  }
  saveBasicDetails(data: Object) {
    return this.http.post(ApiURLs.saveBasicDetailsAPI, data)
      .pipe(response => response);
  }
  saveModelDetails(data: Object) {
    return this.http.post(ApiURLs.saveModelDetailsAPI, data)
      .pipe(response => response);
  }
  saveAddWithdrawl(data: Object) {
    return this.http.post(ApiURLs.saveAddWithdrawlAPI, data)
      .pipe(response => response);
  }
  saveAddDeposit(data: Object) {
    return this.http.post(ApiURLs.saveAddDepositAPI, data)
      .pipe(response => response);
  }
  saveZegaCustom(data: Object) {
    return this.http.post(ApiURLs.saveZegaCustomAPI, data)
      .pipe(response => response);
  }
}
