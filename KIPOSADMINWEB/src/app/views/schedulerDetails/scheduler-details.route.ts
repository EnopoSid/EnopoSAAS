import { Routes } from '@angular/router';
import { SchedulerDetailsComponent } from './scheduler-details.component';



export const schedulerDetailsRoutes: Routes = [
  { path: '', component: SchedulerDetailsComponent, data: { title: 'schedulerDetails' } }
];
