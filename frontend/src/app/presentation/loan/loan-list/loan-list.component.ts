import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { LoanUseCase } from '../../../core/use-cases/loan.use-case';
import { LoanModel } from '../../../core/models/loan';
import { LoanDetailsDialogComponent } from '../loan-details-dialog/loan-details-dialog.component';

@Component({
  selector: 'app-loan-list',
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatIconModule,
  ],
  templateUrl: './loan-list.component.html',
  styleUrl: './loan-list.component.scss',
})
export class LoanListComponent implements OnInit {
  displayedColumns: string[] = [
    'amount',
    'currentBalance',
    'appllicantName',
    'status',
    'actions',
  ];
  loans: LoanModel[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  private readonly _loanUseCase = inject(LoanUseCase);
  private readonly _dialog = inject(MatDialog);

  constructor() {}

  ngOnInit(): void {
    this.loadLoans();
  }

  loadLoans(): void {
    this.isLoading = true;
    this.errorMessage = null;

    // TODO: pagination
    this._loanUseCase.getAllLoans({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.loans = response.loans;
        } else {
          this.errorMessage = response.message || 'Failed to load loans';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'An error occurred while loading loans';
        console.error('Error loading loans:', error);
      },
    });
  }

  openDetails(loan: LoanModel): void {
    this._dialog.open(LoanDetailsDialogComponent, {
      width: '600px',
      data: {
        loanId: loan.loanId,
        applicantName: loan.appllicantName,
      },
    });
  }
}
