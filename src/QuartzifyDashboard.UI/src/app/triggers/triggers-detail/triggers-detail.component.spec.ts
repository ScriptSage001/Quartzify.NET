import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TriggersDetailComponent } from './triggers-detail.component';

describe('TriggersDetailComponent', () => {
  let component: TriggersDetailComponent;
  let fixture: ComponentFixture<TriggersDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TriggersDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TriggersDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
