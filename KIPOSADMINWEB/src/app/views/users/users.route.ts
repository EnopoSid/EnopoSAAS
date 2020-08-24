import { Routes } from '@angular/router';

import { GetUsersComponent } from './users.component';
import { GetAddUsersComponent } from 'src/app/views/users/add-user/add-user.component';
import { GetUserProfileComponent } from 'src/app/views/users/viewprofile/viewprofile.component';


export const GetUsersRoutes: Routes = [
{ path: 'users', component: GetUsersComponent, data: { title: 'Users' }},
 { path: 'users/add', component: GetAddUsersComponent, data: { title: 'Add-User' }},
 { path: 'users/view/:id', component: GetAddUsersComponent, data: { title: 'View-User' }},
 { path: 'users/update/:id', component: GetAddUsersComponent, data: { title: 'Edit-User' }},
  { path: 'viewprofile', component: GetUserProfileComponent, data: { title: 'View-Profile' }}
];
export const ViewProfileRoutes: Routes = [
    { path: '', component: GetUserProfileComponent, data: { title: 'View-Profile' }}
];
