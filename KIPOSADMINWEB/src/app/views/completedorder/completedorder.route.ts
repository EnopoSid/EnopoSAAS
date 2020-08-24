import { Routes } from '@angular/router';
import { completedorderComponent } from './completedorder.component';


export const completedorderRoutes: Routes = [
  { path: '', component: completedorderComponent, data: { title: 'completedorder' } }
];