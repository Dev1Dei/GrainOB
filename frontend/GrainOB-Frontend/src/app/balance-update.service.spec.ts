import { TestBed } from '@angular/core/testing';

import { BalanceUpdateService } from './balance-update.service';

describe('BalanceUpdateService', () => {
  let service: BalanceUpdateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BalanceUpdateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
