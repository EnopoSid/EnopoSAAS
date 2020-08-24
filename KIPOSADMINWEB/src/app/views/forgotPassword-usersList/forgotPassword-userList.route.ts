import { Routes } from '@angular/router';


import { ForgotPasswordUserListComponent } from './forgotPassword-userList.component';

export const ForgotPasswordUserListRoutes: Routes = [
  { path: '', component: ForgotPasswordUserListComponent, data: { title: 'ForgotPasswordUserList' } },
];