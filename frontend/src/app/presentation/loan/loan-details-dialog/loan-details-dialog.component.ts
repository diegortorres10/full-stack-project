import { Component, inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { LoanUseCase } from '../../../core/use-cases/loan.use-case';
import { LoanPaymentModel } from '../../../core/models/loan';
import { CreatePaymentDialogComponent } from '../create-payment-dialog/create-payment-dialog.component';

export interface LoanDetailsDialogData {
  loanId: number;
  applicantName: string;
  currentBalance: number;
  loanAmount: number;
  status: string;
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
    MatIconModule,
    MatPaginatorModule,
  ],
  templateUrl: './loan-details-dialog.component.html',
  styleUrl: './loan-details-dialog.component.scss',
})
export class LoanDetailsDialogComponent {
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  readonly dialogRef = inject(MatDialogRef<LoanDetailsDialogComponent>);
  readonly data = inject<LoanDetailsDialogData>(MAT_DIALOG_DATA);
  readonly loanUseCase = inject(LoanUseCase);
  readonly dialog = inject(MatDialog);

  payments: LoanPaymentModel[] = [];
  isLoading = false;
  errorMessage: string | null = null;
  displayedColumns: string[] = ['loanPaymentId', 'amount', 'createdAt'];

  // Pagination properties
  totalItems = 0;
  pageSize = 5;
  pageNumber = 1;
  pageSizeOptions = [5, 10, 20];

  ngOnInit(): void {
    this._loadLoanDetails();
  }

  private _loadLoanDetails(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.loanUseCase
      .getLoanDetails(this.data.loanId, { pageNumber: this.pageNumber, pageSize: this.pageSize })
      .subscribe({
        next: (response) => {
          this.isLoading = false;
          if (response.success) {
            this.payments = response.details || [];
            this.totalItems = response.totalItems;
          } else {
            this.errorMessage = response.message || 'Failed to load loan details';
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error.message || 'An error occurred while loading loan details';
        },
      });
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1; // MatPaginator uses 0-based index, API uses 1-based
    this._loadLoanDetails();
  }

  close(): void {
    this.dialogRef.close();
  }

  registerPayment(): void {
    const paymentDialogRef = this.dialog.open(CreatePaymentDialogComponent, {
      width: '550px',
      disableClose: true,
      data: {
        loanId: this.data.loanId,
        applicantName: this.data.applicantName,
        currentBalance: this.data.currentBalance,
        loanAmount: this.data.loanAmount,
      },
    });

    paymentDialogRef.afterClosed().subscribe((result) => {
      if (result) {
        // If a payment was created, reload the payment list
        this._loadLoanDetails();
      }
    });
  }
}
