import { TestBed } from '@angular/core/testing';

import { ExecutionHistoryService } from './execution-history.service';

describe('ExecutionHistoryServiceService', () => {
  let service: ExecutionHistoryService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExecutionHistoryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
