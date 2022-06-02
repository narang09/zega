import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiURLs } from '../../support/apiURLs/api-urls';
import { UserSettings } from '../../models/UserSettings/user-settings';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

  deleteUsers(userIds: Array<number>) {
    return this.http.post(ApiURLs.deleteUsersAPI, userIds)
      .pipe(response => response);
  }
  saveUser(user: any) {
    return this.http.post(ApiURLs.saveUserAPI, user)
      .pipe(response => response);
  }
  getUser(userId: number) {
    return this.http.post(ApiURLs.getUserAPI, userId)
      .pipe(response => response);
  }
  getUserSettingsData() {
    return this.http.get(ApiURLs.getSettingsInformationAPI)
      .pipe(response => response);
  }
  getUserImage() {
    return this.http.get(ApiURLs.getImageAPI)
      .pipe(response => response);
  }
  userSettings(userSettings: UserSettings) {
    return this.http.post(ApiURLs.userSettingsAPI, userSettings)
      .pipe(response => response);
  }
  passwordSettings(passwordSettings: UserSettings) {
    return this.http.post(ApiURLs.userSettingsAPI, passwordSettings)
      .pipe(response => response);
  }
  uploadImage(formData: FormData) {
    return this.http.post(ApiURLs.uploadImageAPI, formData)
      .pipe(response => response);
  }
  removeUserImage() {
    return this.http.post(ApiURLs.removeUserProfileImageAPI, null)
      .pipe(response => response);
  }
  saveRepCode(repCodeModel: any) {
    return this.http.post(ApiURLs.saveRepCodeAPI, repCodeModel)
      .pipe(response => response);
  }
  deleteRepCodes(repCodeIds: Array<number>) {
    return this.http.post(ApiURLs.deleteRepCodeAPI, repCodeIds)
      .pipe(response => response);
  }

}
