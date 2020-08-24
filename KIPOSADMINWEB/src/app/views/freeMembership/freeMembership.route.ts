import { Routes } from '@angular/router';
import { FreeMembershipComponent } from './freeMembership.component';



export const FreeMembershipRoutes: Routes = [
  { path: '', component: FreeMembershipComponent, data: { title: 'Configuration' } }
];