import { Routes } from '@angular/router';

import { ChangePasswordComponent } from './change-password.component';

export const ChangePasswordRoutes: Routes = [
  { path: '', component: ChangePasswordComponent, data: { title: 'Change Password' } }
];
