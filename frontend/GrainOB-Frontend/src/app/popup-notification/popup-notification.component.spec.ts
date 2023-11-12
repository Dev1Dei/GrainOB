import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PopupNotificationComponent } from './popup-notification.component';

describe('PopupNotificationComponent', () => {
  let component: PopupNotificationComponent;
  let fixture: ComponentFixture<PopupNotificationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PopupNotificationComponent]
    });
    fixture = TestBed.createComponent(PopupNotificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
