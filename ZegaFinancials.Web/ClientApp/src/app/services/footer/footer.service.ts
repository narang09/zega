import { Injectable } from '@angular/core';
import { ApiURLs } from '../../support/apiURLs/api-urls';
import { HttpClient } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class FooterService {

  constructor(private http: HttpClient) { }
  getVersionInfo() {
    return this.http.get(ApiURLs.getVersionInfoAPI)
      .pipe(response => response);
  }
}
