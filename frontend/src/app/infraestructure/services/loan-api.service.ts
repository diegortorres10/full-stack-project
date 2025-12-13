import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../core/config/const.config';
import {
  CreateLoanPaymentRequest,
  CreateLoanPaymentResponse,
  CreateLoanRequest,
  CreateLoanResponse,
  GetAllLoansResponse,
  GetLoanDetailsResponse,
} from '../../core/models/loan';
import { PaginationFilter } from '../../core/models/common';

@Injectable({
  providedIn: 'root',
})
export class LoanApiService {
  private readonly baseUrl = API_BASE_URL;

  constructor(private _http: HttpClient) {}

  getAllLoans(filters: PaginationFilter): Observable<GetAllLoansResponse> {
    return this._http.get<GetAllLoansResponse>(`${this.baseUrl}/loans`, {
      params: this._buildParams(filters),
    });
  }

  createLoans(request: CreateLoanRequest): Observable<CreateLoanResponse> {
    return this._http.post<CreateLoanResponse>(`${this.baseUrl}/loans`, {
      request,
    });
  }

  getLoanDetails(
    loanId: number,
    filters: PaginationFilter
  ): Observable<GetLoanDetailsResponse> {
    return this._http.get<GetLoanDetailsResponse>(
      `${this.baseUrl}/loans/${loanId}`,
      {
        params: this._buildParams(filters),
      }
    );
  }

  createLoanPayment(
    loanId: number,
    request: CreateLoanPaymentRequest
  ): Observable<CreateLoanPaymentResponse> {
    return this._http.post<CreateLoanPaymentResponse>(
      `${this.baseUrl}/loans/${loanId}/payment`,
      {
        request,
      }
    );
  }

  private _buildParams(filters: PaginationFilter): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', filters.pageNumber.toString())
      .set('pageSize', filters.pageSize.toString());

    return params;
  }
}
