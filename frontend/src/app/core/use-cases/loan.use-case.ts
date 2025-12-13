import { Injectable } from '@angular/core';
import { LoanRepository } from '../repositories/loan.repository';

@Injectable({
  providedIn: 'root',
})
export class LoanUseCase {
  constructor(private _loanRepository: LoanRepository) {}

  
}
