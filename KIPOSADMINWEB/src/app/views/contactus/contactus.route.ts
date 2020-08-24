import { Routes } from '@angular/router';
import {GetContactUsComponent } from './contactus.component';

export const ContactUsRoutes: Routes = [
    {path: '', component: GetContactUsComponent, data:{title: 'ContactUsComponentent'}},
];