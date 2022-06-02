import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiURLs } from '../../support/apiURLs/api-urls';


@Injectable({
  providedIn: 'root'
})
export class DataGridService {

  constructor(private http: HttpClient) { }

  getGridHeader(dataGrid: string) {
    return this.http.post(ApiURLs.getGridHeaderAPI, dataGrid)
      .pipe(response => response);
  }

  getGridData(gridModel: any) {
    return this.http.post(ApiURLs.getDataGridDataAPI, gridModel)
      .pipe(response => response);
  }

  saveGridPreferences(gridPreferenceModel: any) {
    return this.http.post(ApiURLs.saveGridPreferencesAPI, gridPreferenceModel)
      .pipe(response => response);
  }

  exportPaginatedGridToExcel(gridExportModel: any) {
    return this.http.post(ApiURLs.exportPaginatedGridToExcelAPI, gridExportModel, { responseType: 'blob' })
      .pipe(response => response);
  }

}
