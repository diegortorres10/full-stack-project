import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { LoanUseCase } from '../../../core/use-cases/loan.use-case';
import { CreateLoanRequest } from '../../../core/models/loan';

@Component({
  selector: 'app-create-loan-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatIconModule,
  ],
  templateUrl: './create-loan-dialog.component.html',
  styleUrl: './create-loan-dialog.component.scss',
})
export class CreateLoanDialogComponent {
  readonly dialogRef = inject(MatDialogRef<CreateLoanDialogComponent>);
  readonly loanUseCase = inject(LoanUseCase);
  readonly formBuilder = inject(FormBuilder);

  loanForm: FormGroup;
  isSubmitting = false;
  errorMessage: string | null = null;

  constructor() {
    this.loanForm = this.formBuilder.group({
      applicantName: ['', [Validators.required, Validators.minLength(3)]],
      amount: [null, [Validators.required, Validators.min(1)]],
    });
  }

  onSubmit(): void {
    if (this.loanForm.invalid) {
      this.loanForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    const request: CreateLoanRequest = {
      applicantName: this.loanForm.value.applicantName,
      amount: this.loanForm.value.amount,
    };

    this.loanUseCase.createLoan(request).subscribe({
      next: (response) => {
        this.isSubmitting = false;
        if (response.success) {
          this.dialogRef.close(response.success);
        } else {
          this.errorMessage = response.message || 'Failed to create loan';
        }
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error.message || 'An error occurred while creating the loan';
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  getErrorMessage(fieldName: string): string {
    const field = this.loanForm.get(fieldName);
    if (field?.hasError('required')) {
      return 'This field is required';
    }
    if (field?.hasError('minLength')) {
      return 'Name must be at least 3 characters';
    }
    if (field?.hasError('min')) {
      return 'Amount must be greater than 0';
    }
    return '';
  }
}
