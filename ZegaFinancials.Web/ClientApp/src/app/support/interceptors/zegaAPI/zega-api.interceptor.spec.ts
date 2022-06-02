import { TestBed } from '@angular/core/testing';

import { ZegaAPIInterceptor } from './zega-api.interceptor';

describe('ZegaAPIInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      ZegaAPIInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: ZegaAPIInterceptor = TestBed.inject(ZegaAPIInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
