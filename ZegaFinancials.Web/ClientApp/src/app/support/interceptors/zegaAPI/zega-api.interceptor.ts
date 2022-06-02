import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { ResponseVO } from '../../../models/ResponseVO/response-vo';
import { Constants } from '../../constants/constants';
import { AuthMessageService } from '../../../services/AuthMessageService/auth-message.service';
import { AuthenticationService } from '../../../services/AuthenticationService/authentication.service';
import { Router } from '@angular/router';

@Injectable()
export class ZegaAPIInterceptor implements HttpInterceptor {

  constructor(private authMessageService: AuthMessageService, private authenticationService: AuthenticationService, private router: Router) { }

  private token = localStorage.getItem("Token");

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.body && req.body.constructor.name === 'FormData') {
      // Don't set headers
    } else
      req = req.clone({
        headers: req.headers.set('Content-Type', 'application/json')
      })
    this.addToken(req);
    return next.handle(req).pipe(
      tap((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          let responseV0: ResponseVO = event['body'];
          if (responseV0 && responseV0.success == Constants.ErrorResponse)
            this.showErrorPopup(responseV0);
        } else if (event instanceof HttpErrorResponse) {
          let responseV0: ResponseVO = new ResponseVO;
          responseV0.exception = event.error.message || '';
          responseV0.message = 'An error occurred while processing request!'
          responseV0.success = Constants.ErrorResponse;
          this.showErrorPopup(responseV0);
        }
      }),
      catchError((error: HttpErrorResponse) => {
        let responseV0: ResponseVO = new ResponseVO;
        if (error.status === 401) {
          responseV0.message = error?.error?.message ?? 'UnAuthorized Request!'
          this.logOutAndNavigateToLogin();
        } else {
          responseV0.message = 'An error occurred while processing request!'
          responseV0.exception = error.error.message || '';
        }
        responseV0.success = Constants.ErrorResponse;
        this.showErrorPopup(responseV0);
        return throwError(error);
      }),
      finalize(() => {
      })
    )
  }

  private logOutAndNavigateToLogin() {
    this.authenticationService.logoutUserUI()
    this.router.navigate(['/login']);
  }

  showErrorPopup(responseV0: ResponseVO) {
    this.authMessageService.showErrorPopup(responseV0);
  }

  addToken(req: HttpRequest<any>): HttpRequest<any> {
    if (this.token)
      req = req.clone({
        headers: req.headers.set('Authorization', 'Bearer ' + this.token)
      })
    return req;
  }

}
