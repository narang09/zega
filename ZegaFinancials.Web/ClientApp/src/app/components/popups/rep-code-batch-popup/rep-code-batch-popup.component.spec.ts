import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RepCodeBatchPopupComponent } from './rep-code-batch-popup.component';

describe('RepCodeBatchPopupComponent', () => {
  let component: RepCodeBatchPopupComponent;
  let fixture: ComponentFixture<RepCodeBatchPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RepCodeBatchPopupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RepCodeBatchPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
