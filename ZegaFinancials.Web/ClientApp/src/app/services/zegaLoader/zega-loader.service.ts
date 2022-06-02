import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/internal/Subject';
import { LoaderState } from '../../models/LoaderState/loader-state';

@Injectable({
  providedIn: 'root'
})
export class ZegaLoaderService {
  private loaderSubject = new Subject<LoaderState>();
  loaderState = this.loaderSubject.asObservable();

  constructor() { }

  show() {
    this.loaderSubject.next(<LoaderState>{ show: true });
  }
  hide() {
    this.loaderSubject.next(<LoaderState>{ show: false });
  }

}
