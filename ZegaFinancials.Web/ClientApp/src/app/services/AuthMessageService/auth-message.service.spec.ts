import { TestBed } from '@angular/core/testing';

import { AuthMessageService } from './auth-message.service';

describe('AuthMessageService', () => {
  let service: AuthMessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthMessageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
