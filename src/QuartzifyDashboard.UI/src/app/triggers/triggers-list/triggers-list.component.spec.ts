import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TriggersListComponent } from './triggers-list.component';

describe('TriggersListComponent', () => {
  let component: TriggersListComponent;
  let fixture: ComponentFixture<TriggersListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TriggersListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TriggersListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
