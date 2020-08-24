import { Routes } from '@angular/router';
import { StoreTimingsComponent } from './store-timings.component';

  
export const StoreTimingsRoutes: Routes = [
  { path: '', component: StoreTimingsComponent, data: { title: 'Stores' } }
];