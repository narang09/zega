import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiURLs } from '../../support/apiURLs/api-urls';
import { Import } from '../../models/Import/import';

@Injectable({
  providedIn: 'root'
})
export class ImportService {

  constructor(private http: HttpClient) { }

  saveImportProfile(importData: Import) {
    return this.http.post(ApiURLs.saveImportProfileAPI, importData)
      .pipe(response => response);
  }

  getImportData() {
    return this.http.get(ApiURLs.getImportDataAPI)
      .pipe(response => response);
  }

  getRepCodeDropdownData() {
    return this.http.get(ApiURLs.getRepCodeDropdownDataAPI)
      .pipe(response => response);
  }

  runImport(profileId: number) {
    return this.http.post(ApiURLs.runImportAPI, profileId)
      .pipe(response => response);
  }

  uploadImportFile(formData: FormData) {
    return this.http.post(ApiURLs.uploadImportFileAPI, formData)
      .pipe(response => response);
  }

}
