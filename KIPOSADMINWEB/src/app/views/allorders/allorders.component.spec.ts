import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ALLOrdersComponent } from './allorders.component';

describe('ALLOrdersComponent', () => {
  let component: ALLOrdersComponent;
  let fixture: ComponentFixture<ALLOrdersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ALLOrdersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ALLOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
