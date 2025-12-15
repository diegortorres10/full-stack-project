import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, inject, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { LoanUseCase } from '../../../core/use-cases/loan.use-case';
import { LoanModel, GetLoanFilterInterface } from '../../../core/models/loan';
import { LoanDetailsDialogComponent } from '../loan-details-dialog/loan-details-dialog.component';
import { CreateLoanDialogComponent } from '../create-loan-dialog/create-loan-dialog.component';
import { CreatePaymentDialogComponent } from '../create-payment-dialog/create-payment-dialog.component';

@Component({
  selector: 'app-loan-list',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatIconModule,
    MatTooltipModule,
    MatPaginatorModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
  ],
  templateUrl: './loan-list.component.html',
  styleUrl: './loan-list.component.scss',
})
export class LoanListComponent implements OnInit, OnDestroy {
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  displayedColumns: string[] = [
    'amount',
    'currentBalance',
    'appllicantName',
    'createdAt',
    'status',
    'actions',
  ];
  loans: LoanModel[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  // Pagination properties
  totalItems = 0;
  pageSize = 5;
  pageNumber = 1;
  pageSizeOptions = [5, 10, 20];

  // Filter form
  filterForm = new FormGroup({
    applicantName: new FormControl(''),
    dateRange: new FormGroup({
      start: new FormControl<Date | null>(null),
      end: new FormControl<Date | null>(null),
    }),
  });

  private readonly _loanUseCase = inject(LoanUseCase);
  private readonly _dialog = inject(MatDialog);
  private readonly _destroy$ = new Subject<void>();

  constructor() {}

  ngOnInit(): void {
    this.loadLoans();
    this._setupFilterListeners();
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  private _setupFilterListeners(): void {
    // Listen to applicant name changes with debounce
    this.filterForm.get('applicantName')?.valueChanges
      .pipe(
        debounceTime(800),
        distinctUntilChanged(),
        takeUntil(this._destroy$)
      )
      .subscribe(() => {
        this._resetPaginationAndLoadLoans();
      });

    // Listen to date range changes (start date)
    this.filterForm.get('dateRange.start')?.valueChanges
      .pipe(
        distinctUntilChanged(),
        takeUntil(this._destroy$)
      )
      .subscribe(() => {
        this._resetPaginationAndLoadLoans();
      });

    // Listen to date range changes (end date)
    this.filterForm.get('dateRange.end')?.valueChanges
      .pipe(
        distinctUntilChanged(),
        takeUntil(this._destroy$)
      )
      .subscribe(() => {
        this._resetPaginationAndLoadLoans();
      });
  }

  private _resetPaginationAndLoadLoans(): void {
    this.pageNumber = 1;
    if (this.paginator) {
      this.paginator.pageIndex = 0;
    }
    this.loadLoans();
  }

  loadLoans(): void {
    this.isLoading = true;
    this.errorMessage = null;

    const filters = this._buildFilters();

    this._loanUseCase.getAllLoans(filters).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.loans = response.loans;
          this.totalItems = response.totalItems;
        } else {
          this.errorMessage = response.message || 'Failed to load loans';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error.message || 'An error occurred while loading loans';
      },
    });
  }

  get hasActiveFilters(): boolean {
    const formValue = this.filterForm.value;
    return !!(
      formValue.applicantName ||
      formValue.dateRange?.start ||
      formValue.dateRange?.end
    );
  }

  clearFilters(): void {
    this.filterForm.reset();
    this._resetPaginationAndLoadLoans();
  }

  createNewLoan(): void {
    const dialogRef = this._dialog.open(CreateLoanDialogComponent, {
      width: '500px',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        // If a loan was created, reload the list
        this.loadLoans();
      }
    });
  }

  private _buildFilters(): GetLoanFilterInterface {
    const formValue = this.filterForm.value;
    const filters: GetLoanFilterInterface = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
    };

    if (formValue.applicantName) {
      filters.applicantName = formValue.applicantName;
    }

    if (formValue.dateRange?.start) {
      filters.startDate = formValue.dateRange.start;
    }

    if (formValue.dateRange?.end) {
      filters.endDate = formValue.dateRange.end;
    }

    return filters;
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1; // MatPaginator uses 0-based index, API uses 1-based
    this.loadLoans();
  }

  openDetails(loan: LoanModel): void {
    this._dialog.open(LoanDetailsDialogComponent, {
      width: '600px',
      data: {
        loanId: loan.loanId,
        applicantName: loan.appllicantName,
        currentBalance: loan.currentBalance,
        loanAmount: loan.amount,
        status: loan.status,
      },
    });
  }

  registerPayment(loan: LoanModel): void {
    const dialogRef = this._dialog.open(CreatePaymentDialogComponent, {
      width: '550px',
      disableClose: true,
      data: {
        loanId: loan.loanId,
        applicantName: loan.appllicantName,
        currentBalance: loan.currentBalance,
        loanAmount: loan.amount,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        // If a payment was created, reload the list
        this.loadLoans();
      }
    });
  }
}
