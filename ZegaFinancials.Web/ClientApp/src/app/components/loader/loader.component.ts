import { Component, OnDestroy, OnInit } from '@angular/core';

import { ThemePalette } from '@angular/material/core';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';

import { Subscription } from 'rxjs/internal/Subscription';
import { LoaderState } from '../../models/LoaderState/loader-state';
import { ZegaLoaderService } from '../../services/zegaLoader/zega-loader.service';

@Component({
  selector: 'zega-loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.less']
})
export class LoaderComponent implements OnInit, OnDestroy {
  spinnerColor: ThemePalette = 'primary';
  spinnerMode: ProgressSpinnerMode = 'indeterminate'
  showLoader = false;
  private subscription: Subscription = new Subscription;
  constructor(private loaderService: ZegaLoaderService) { }
  ngOnInit() {
    this.subscription = this.loaderService.loaderState
      .subscribe((state: LoaderState) => {
        this.showLoader = state.show;
      });
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

}
