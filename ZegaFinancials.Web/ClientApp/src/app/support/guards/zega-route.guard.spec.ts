import { TestBed } from '@angular/core/testing';

import { ZegaRouteGuard } from './zega-route.guard';

describe('ZegaRouteGuard', () => {
  let guard: ZegaRouteGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ZegaRouteGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
