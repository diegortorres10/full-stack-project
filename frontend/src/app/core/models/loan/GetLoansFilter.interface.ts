import { PaginationFilter } from '../common';

export interface GetLoanFilterInterface extends PaginationFilter {
  applicantName?: string;
  startDate?: Date;
  endDate?: Date;
}
