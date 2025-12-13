import { BaseApiResponse } from '../common';
import { LoanModel } from './LoanModelDto.interface';

export interface CreateLoanResponse extends BaseApiResponse {
  loan: LoanModel;
}
