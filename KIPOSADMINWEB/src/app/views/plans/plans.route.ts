import { Routes } from '@angular/router';

import { PlansComponent } from './plans.component';
import { AddPlans } from './addPlans/addPlans.component';

  
export const PlansRoutes: Routes = [
  { path: '', component: PlansComponent, data: { title: 'Plans' } },
  { path: 'plans', component: PlansComponent, data: { title: 'Plans' } },
  { path: 'addPlans', component: AddPlans, data: { title: 'Add-Plans' } },
  { path: 'addPlans/:id', component: AddPlans, data: { title: 'Edit-Plans' } },
];