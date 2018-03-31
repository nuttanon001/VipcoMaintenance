import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkgroupDialogComponent } from './workgroup-dialog.component';

describe('WorkgroupDialogComponent', () => {
  let component: WorkgroupDialogComponent;
  let fixture: ComponentFixture<WorkgroupDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkgroupDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkgroupDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
