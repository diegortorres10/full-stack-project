import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoanRepository } from '../repositories/loan.repository';
import { PaginationFilter } from '../models/common';
import {
  CreateLoanPaymentRequest,
  CreateLoanPaymentResponse,
  CreateLoanRequest,
  CreateLoanResponse,
  GetAllLoansResponse,
  GetLoanDetailsResponse,
} from '../models/loan';

@Injectable({
  providedIn: 'root',
})
export class LoanUseCase {
  constructor(private _loanRepository: LoanRepository) {}

  getAllLoans(filters: PaginationFilter): Observable<GetAllLoansResponse> {
    return this._loanRepository.getAllLoans(filters);
  }

  createLoan(request: CreateLoanRequest): Observable<CreateLoanResponse> {
    return this._loanRepository.createLoan(request);
  }

  getLoanDetails(
    loanId: number,
    filters: PaginationFilter
  ): Observable<GetLoanDetailsResponse> {
    return this._loanRepository.getLoanDetails(loanId, filters);
  }

  createLoanPayment(
    loanId: number,
    request: CreateLoanPaymentRequest
  ): Observable<CreateLoanPaymentResponse> {
    return this._loanRepository.createLoanPayment(loanId, request);
  }
}
