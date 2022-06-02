import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ZegaLoaderService } from '../../../services/zegaLoader/zega-loader.service';
import { tap } from 'rxjs/operators';

@Injectable()
export class ZegaLoaderInterceptor implements HttpInterceptor {

  constructor(private loaderService: ZegaLoaderService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.showLoader();
    return next.handle(req).pipe(
      tap((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse)
          this.hideLoader();
      },
        (err: any) => {
          this.hideLoader();
        })
    );
  }


  private showLoader(): void {
    this.loaderService.show();
  }

  private hideLoader(): void {
    this.loaderService.hide();
  }

}
