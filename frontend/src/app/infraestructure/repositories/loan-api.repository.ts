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
    throw new Error('Method not implemented.');
  }
  override createLoan(
    request: CreateLoanRequest
  ): Observable<CreateLoanResponse> {
    throw new Error('Method not implemented.');
  }
  override getLoanDetails(
    loanId: number,
    filters: PaginationFilter
  ): Observable<GetLoanDetailsResponse> {
    throw new Error('Method not implemented.');
  }
  override createLoanPayment(
    loanId: number,
    request: CreateLoanPaymentRequest
  ): Observable<CreateLoanPaymentResponse> {
    throw new Error('Method not implemented.');
  }
}
