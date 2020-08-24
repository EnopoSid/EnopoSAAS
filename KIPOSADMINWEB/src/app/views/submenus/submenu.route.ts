import { Routes } from '@angular/router';
import { SubMenuComponent } from './submenu.component';

export const SubMenuRoutes: Routes = [
  { path: ':id', component: SubMenuComponent, data: { title: 'SubMenu' } },
  { path: '', component: SubMenuComponent, data: { title: 'MenuRedirect' } }
];