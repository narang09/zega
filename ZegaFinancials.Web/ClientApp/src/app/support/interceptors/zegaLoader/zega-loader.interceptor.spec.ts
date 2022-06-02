import { TestBed } from '@angular/core/testing';

import { ZegaLoaderInterceptor } from './zega-loader.interceptor';

describe('ZegaLoaderInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      ZegaLoaderInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: ZegaLoaderInterceptor = TestBed.inject(ZegaLoaderInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
