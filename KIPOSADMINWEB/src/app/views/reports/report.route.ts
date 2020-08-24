import { Routes } from '@angular/router';
import { ReportComponent } from './report.component';
import { OrderDetailsComponent } from './orderDetails/orderDetails.component';

export const ReportRoutes: Routes = [
  { path: '', component: ReportComponent, data: { title: 'Reports' } },

  {path: 'orderdetails/:id', component: OrderDetailsComponent , data: { title: 'orderdetails' } },
];