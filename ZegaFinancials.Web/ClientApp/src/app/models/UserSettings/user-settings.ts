export class UserSettings {
  constructor() {
    this.FirstName = '';
    this.LastName = '';
    this.Email = '';
    this.CountryCode = '';
    this.PhoneNo = '';
    this.Company = '';
    this.Designation = '';

    this.CurrentPassword = '';
    this.Password = '';
    this.ReNewPassword = '';
    this.isPasswordChanged = false;
  }

  FirstName: string;
  LastName: string;
  Email: string;
  CountryCode: string;
  PhoneNo: string;
  Company: string;
  Designation: string;

  CurrentPassword: string;
  Password: string;
  ReNewPassword: string;
  isPasswordChanged: boolean;
}
