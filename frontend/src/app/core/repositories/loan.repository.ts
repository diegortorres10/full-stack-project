import { Observable } from 'rxjs';
import { PaginationFilter } from '../models/common';
import {
  CreateLoanPaymentRequest,
  CreateLoanPaymentResponse,
  CreateLoanRequest,
  CreateLoanResponse,
  GetAllLoansResponse,
  GetLoanDetailsResponse,
  GetLoanFilterInterface,
} from '../models/loan';

export abstract class LoanRepository {
  abstract getAllLoans(
    filters: GetLoanFilterInterface
  ): Observable<GetAllLoansResponse>;

  abstract createLoan(
    request: CreateLoanRequest
  ): Observable<CreateLoanResponse>;

  abstract getLoanDetails(
    loanId: number,
    filters: PaginationFilter
  ): Observable<GetLoanDetailsResponse>;

  abstract createLoanPayment(
    loanId: number,
    request: CreateLoanPaymentRequest
  ): Observable<CreateLoanPaymentResponse>;
}
