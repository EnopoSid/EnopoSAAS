import { Routes } from '@angular/router';

import { deliveryordersComponent } from './deliveryorders.component';

  
export const deliveryordersRoutes: Routes = [
  { path: '', component: deliveryordersComponent, data: { title: 'deliveryorders' } }
];