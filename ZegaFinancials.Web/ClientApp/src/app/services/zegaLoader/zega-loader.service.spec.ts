import { TestBed } from '@angular/core/testing';

import { ZegaLoaderService } from './zega-loader.service';

describe('ZegaLoaderService', () => {
  let service: ZegaLoaderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ZegaLoaderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
