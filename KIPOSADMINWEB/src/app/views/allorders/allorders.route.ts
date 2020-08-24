import { Routes } from '@angular/router';
import { ALLOrdersComponent } from './allorders.component';


export const ALLOrderRoutes: Routes = [
  { path: '', component: ALLOrdersComponent, data: { title: 'allorder' } }
];