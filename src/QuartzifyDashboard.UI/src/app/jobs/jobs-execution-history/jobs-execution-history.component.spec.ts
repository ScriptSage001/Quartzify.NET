import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobsExecutionHistoryComponent } from './jobs-execution-history.component';

describe('JobsExecutionHistoryComponent', () => {
  let component: JobsExecutionHistoryComponent;
  let fixture: ComponentFixture<JobsExecutionHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobsExecutionHistoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobsExecutionHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
