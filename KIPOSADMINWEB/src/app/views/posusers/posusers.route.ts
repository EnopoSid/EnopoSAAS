import { Routes } from '@angular/router';

import { GetPosUsersComponent } from './posusers.component';
import { GetAddPosUsersComponent } from 'src/app/views/posusers/add-posusers/add-posusers.component';
import { GetPosUserProfileComponent } from 'src/app/views/posusers/viewprofile/viewprofile.component';


export const GetPosUsersRoutes: Routes = [
{ path: '', component: GetPosUsersComponent, data: { title: 'Users' }},
 { path: 'add', component: GetAddPosUsersComponent, data: { title: 'Add-User' }},
 { path: 'view/:id', component: GetAddPosUsersComponent, data: { title: 'View-User' }},
 { path: 'update/:id', component: GetAddPosUsersComponent, data: { title: 'Edit-User' }}
];

