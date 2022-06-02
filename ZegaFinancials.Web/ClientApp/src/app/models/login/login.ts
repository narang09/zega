export class Login {

  constructor() {
    this.Login = '';
    this.Password = '';
    this.Email = '';
    this.TempAuthToken = '';
    this.IsForceLogin = false
  }

  Login: string;
  Password: string;
  Email: string;
  TempAuthToken: string;
  IsForceLogin: boolean;
}
