import { Routes } from '@angular/router';

import { PaymentComponent } from './payment.component';

  
export const PaymentRoutes: Routes = [
  { path: '', component: PaymentComponent, data: { title: 'Payment' } }
];