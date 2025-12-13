import { Injectable } from '@angular/core';
import { LoanRepository } from '../../core/repositories/loan.repository';
import { Observable } from 'rxjs';
import { LoanApiService } from '../services/loan-api.service';
import { PaginationFilter } from '../../core/models/common';
import {
  CreateLoanPaymentRequest,
  CreateLoanPaymentResponse,
  CreateLoanRequest,
  CreateLoanResponse,
  GetAllLoansResponse,
  GetLoanDetailsResponse,
} from '../../core/models/loan';

@Injectable({
  providedIn: 'root',
})
export class LoanApiRepository extends LoanRepository {
  constructor(private _loanApiService: LoanApiService) {
    super();
  }

  override getAllLoans(
    filters: PaginationFilter
  ): Observable<GetAllLoansResponse> {
    return this._loanApiService.getAllLoans(filters);
  }

  override createLoan(
    request: CreateLoanRequest
  ): Observable<CreateLoanResponse> {
    return this._loanApiService.createLoans(request);
  }

  override getLoanDetails(
    loanId: number,
    filters: PaginationFilter
  ): Observable<GetLoanDetailsResponse> {
    return this._loanApiService.getLoanDetails(loanId, filters);
  }

  override createLoanPayment(
    loanId: number,
    request: CreateLoanPaymentRequest
  ): Observable<CreateLoanPaymentResponse> {
    return this._loanApiService.createLoanPayment(loanId, request);
  }
}
