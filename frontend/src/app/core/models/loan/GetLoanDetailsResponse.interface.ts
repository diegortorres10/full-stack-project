import { BaseApiResponse } from '../common';
import { LoanPaymentModel } from './LoanModelDto.interface';

export interface GetLoanDetailsResponse extends BaseApiResponse {
  details: LoanPaymentModel[];
  totalItems: number;
}
