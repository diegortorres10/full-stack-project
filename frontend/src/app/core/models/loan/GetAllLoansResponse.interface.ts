import { BaseApiResponse } from '../common';
import { LoanModel } from './LoanModelDto.interface';

export interface GetAllLoansResponse extends BaseApiResponse {
  loans: LoanModel[];
  totalItems: number;
}