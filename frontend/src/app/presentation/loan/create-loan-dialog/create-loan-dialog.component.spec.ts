import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateLoanDialogComponent } from './create-loan-dialog.component';

describe('CreateLoanDialogComponent', () => {
  let component: CreateLoanDialogComponent;
  let fixture: ComponentFixture<CreateLoanDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateLoanDialogComponent],
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateLoanDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
