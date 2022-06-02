import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiURLs } from '../../support/apiURLs/api-urls';

@Injectable({
  providedIn: 'root'
})
export class SidebarService {

  constructor(private http: HttpClient) { }

  getBulkSidebarDropdowns(bulkEditModel: any) {
    return this.http.post(ApiURLs.getBulkEditDropdownsAPI, bulkEditModel)
      .pipe(response => response);
  }

  saveBulkEditInfo(bulkEditModel: any) {
    return this.http.post(ApiURLs.saveBulkEditInformationAPI, bulkEditModel)
      .pipe(response => response);
  }

}
