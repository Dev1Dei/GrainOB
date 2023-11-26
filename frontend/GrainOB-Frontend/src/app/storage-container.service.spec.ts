import { TestBed } from '@angular/core/testing';

import { StorageContainerService } from './storage-container.service';

describe('StorageContainerService', () => {
  let service: StorageContainerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StorageContainerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
