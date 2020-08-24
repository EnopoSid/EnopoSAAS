import { Routes } from '@angular/router';
import { SalesReportComponent } from './salesReport.component';

export const SalesReportRoutes: Routes = [
  { path: '', component: SalesReportComponent, data: { title: 'SalesReport' } },
];