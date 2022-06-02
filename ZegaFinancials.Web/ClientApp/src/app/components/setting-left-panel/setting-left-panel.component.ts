import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/User/user.service';
import { Constants } from '../../support/constants/constants';
import * as $ from 'jquery';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';


@Component({
  selector: 'zega-setting-left-panel',
  templateUrl: './setting-left-panel.component.html',
  styleUrls: ['./setting-left-panel.component.less']
})
export class SettingLeftPanelComponent implements OnInit {

  imageBase64URL: string = '';
  isPasswordView: boolean = false;
  loadUserImage: boolean = false;

  ngOnInit(): void {
    this.getUserProfileImage(false);
  }

  constructor(private userService: UserService, private messageService: AuthMessageService) { }

  onSelectFile(event: any) {
    let files = event.target.files;
    if (files.length === 0)
      return;
    let fileToUpload = <File>files[0];
    let validationErrors: Array<string> = [];
    if (fileToUpload.size >= 2 * 1024 * 1024)
      validationErrors.push('File Size exceed, maximum of 2MB');
    if (["image/jpg", "image/jpeg", "image/png"].indexOf(fileToUpload.type) < 0)
      validationErrors.push('Please select only Image Files');
    if (validationErrors.length) {
      this.messageService.showErrorPopup({ message: validationErrors.join(', ') });
      $('#file-input').val('');
      event.target.value = '';
      return;
    }
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    this.userService.uploadImage(formData).
      subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          this.messageService.showSuccessPopup({ message: response['message'] });
          $('#file-input').val('');
          this.getUserProfileImage(true);
          event.target.value = '';
        }
      });
  }

  private getUserProfileImage(isHeadeerUpdateNeeded: boolean) {
    this.userService.getUserImage().
      subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          var retData = response['response'];
          this.imageBase64URL = retData.imageBytes;
          this.loadUserImage = true;
          if (isHeadeerUpdateNeeded)
            $("#headerUserProfileImage").attr("src", retData.imageBytes);
        }
      });
  }

  changeViewToPassword(value: boolean) {
    this.isPasswordView = value;
  }

  removeUploadedImage() {
    this.userService.removeUserImage().
      subscribe((response: any) => {
        if (response['success'] == Constants.SuccessResponse) {
          this.messageService.showSuccessPopup({ message: response['message'] });
          this.imageBase64URL = '';
          $("#headerUserProfileImage").attr("src", '../../assets/images/ZegaUserProfile.jpg');
        }
      });
  }

}
