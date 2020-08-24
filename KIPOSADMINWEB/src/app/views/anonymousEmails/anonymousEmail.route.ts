import { Routes } from '@angular/router';
import {GetAnonymousEmailComponent } from './anonymousEmail.component';

export const AnonymousEmailRoutes: Routes = [
    {path: '', component: GetAnonymousEmailComponent, data:{title: 'AnonymousEmailComponent'}},
];