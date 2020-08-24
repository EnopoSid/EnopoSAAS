import { Routes } from '@angular/router';
import {GetCareerComponent } from './career.component';

export const CareerRoutes: Routes = [
    {path: '', component: GetCareerComponent, data:{title: 'CareerComponent'}},
];