import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecentHistoryComponent } from './recent-history.component';

describe('RecentHistoryComponent', () => {
  let component: RecentHistoryComponent;
  let fixture: ComponentFixture<RecentHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecentHistoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecentHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
