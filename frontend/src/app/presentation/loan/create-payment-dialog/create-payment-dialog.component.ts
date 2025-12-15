import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { LoanUseCase } from '../../../core/use-cases/loan.use-case';
import { CreateLoanPaymentRequest } from '../../../core/models/loan';

export interface CreatePaymentDialogData {
  loanId: number;
  applicantName: string;
  currentBalance: number;
  loanAmount: number;
}

@Component({
  selector: 'app-create-payment-dialog',
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
  templateUrl: './create-payment-dialog.component.html',
  styleUrl: './create-payment-dialog.component.scss',
})
export class CreatePaymentDialogComponent {
  readonly dialogRef = inject(MatDialogRef<CreatePaymentDialogComponent>);
  readonly data = inject<CreatePaymentDialogData>(MAT_DIALOG_DATA);
  readonly loanUseCase = inject(LoanUseCase);
  readonly formBuilder = inject(FormBuilder);

  paymentForm: FormGroup;
  isSubmitting = false;
  errorMessage: string | null = null;

  constructor() {
    this.paymentForm = this.formBuilder.group({
      amount: [
        null,
        [
          Validators.required,
          Validators.min(0.01),
          Validators.max(this.data.currentBalance),
        ],
      ],
    });
  }

  onSubmit(): void {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    const request: CreateLoanPaymentRequest = {
      amount: this.paymentForm.value.amount,
    };

    this.loanUseCase.createLoanPayment(this.data.loanId, request).subscribe({
      next: (response) => {
        this.isSubmitting = false;
        if (response.success) {
          this.dialogRef.close(response.success);
        } else {
          this.errorMessage = response.message || 'Failed to create payment';
        }
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error.message || 'An error occurred while creating the payment';
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  getErrorMessage(fieldName: string): string {
    const field = this.paymentForm.get(fieldName);
    if (field?.hasError('required')) {
      return 'This field is required';
    }
    if (field?.hasError('min')) {
      return 'Amount must be greater than 0';
    }
    if (field?.hasError('max')) {
      return `Amount cannot exceed current balance ($${this.data.currentBalance.toFixed(2)})`;
    }
    return '';
  }
}
