import { BaseApiResponse } from '../common';
import { LoanPaymentModel } from './LoanModelDto.interface';

export interface CreateLoanPaymentResponse extends BaseApiResponse {
  loanPayment: LoanPaymentModel;
}
