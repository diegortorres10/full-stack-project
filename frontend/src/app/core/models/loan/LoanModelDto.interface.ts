export interface LoanModel {
  loanId: number;
  amount: number;
  currentBalance: number;
  status: string;
  createdAt: string;
  appllicantName: string;
}

export interface LoanPaymentModel {
  loanPaymentId: number;
  amount: number;
  createdAt: string;
}
