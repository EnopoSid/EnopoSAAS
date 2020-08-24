import { Routes } from '@angular/router';
import { GetOrderComponent } from './order.component';



export const orderRoutes: Routes = [
  { path: '', component: GetOrderComponent, data: { title: 'Order' } },
  
];


