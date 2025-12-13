import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoanUseCase } from '../../../core/use-cases/loan.use-case';
import { LoanPaymentModel } from '../../../core/models/loan';

export interface LoanDetailsDialogData {
  loanId: number;
  applicantName: string;
}

@Component({
  selector: 'app-loan-details-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './loan-details-dialog.component.html',
  styleUrl: './loan-details-dialog.component.scss',
})
export class LoanDetailsDialogComponent {
  readonly dialogRef = inject(MatDialogRef<LoanDetailsDialogComponent>);
  readonly data = inject<LoanDetailsDialogData>(MAT_DIALOG_DATA);
  readonly loanUseCase = inject(LoanUseCase);

  payments: LoanPaymentModel[] = [];
  isLoading = false;
  errorMessage: string | null = null;
  displayedColumns: string[] = ['loanPaymentId', 'amount', 'createdAt'];

  ngOnInit(): void {
    this._loadLoanDetails();
  }

  private _loadLoanDetails(): void {
    this.isLoading = true;
    this.errorMessage = null;

    // TODO: pagination
    this.loanUseCase
      .getLoanDetails(this.data.loanId, { pageNumber: 1, pageSize: 100 })
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success) {
            this.payments = response.details || [];
          } else {
            this.errorMessage = response.message || 'Failed to load loan details';
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = 'An error occurred while loading loan details';
          console.error('Error loading loan details:', error);
        },
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
