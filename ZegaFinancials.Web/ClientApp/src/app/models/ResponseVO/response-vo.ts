export class ResponseVO {
  constructor() {
    this.errorCode = '';
    this.exception = '';
    this.success = false;
    this.message = '';
    this.response = {};
  }
  errorCode: string;
  exception: string;
  success: boolean;
  message: string;
  response: any;
}
