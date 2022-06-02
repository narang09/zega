import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BlendModelSidebarComponent } from './blend-model-sidebar.component';

describe('BlendModelSidebarComponent', () => {
  let component: BlendModelSidebarComponent;
  let fixture: ComponentFixture<BlendModelSidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BlendModelSidebarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BlendModelSidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
