import { Routes } from '@angular/router';
import { CancelOrderComponent } from './cancelorder.component';

export const cancelorderRoutes: Routes = [
  { path: '', component: CancelOrderComponent, data: { title: 'cancelorder' } }
];