import { Routes } from '@angular/router';
import { CategoriesComponent } from './categories.component';


export const CategoriesRoutes: Routes = [
  { path: '', component: CategoriesComponent, data: { title: 'allorder' } }
];