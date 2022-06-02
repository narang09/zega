import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingLeftPanelComponent } from './setting-left-panel.component';

describe('SettingLeftPanelComponent', () => {
  let component: SettingLeftPanelComponent;
  let fixture: ComponentFixture<SettingLeftPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SettingLeftPanelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingLeftPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
