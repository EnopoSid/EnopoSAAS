import { Routes } from '@angular/router';
import { pendingorderComponent } from './pendingorder.component';


export const pendingorderRoutes: Routes = [
  { path: '', component: pendingorderComponent, data: { title: 'pendingorder' } }
];