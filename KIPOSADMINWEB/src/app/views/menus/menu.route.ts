import { Routes } from '@angular/router';


import { MenusComponent } from './menu.component';
import { SubMenuComponent } from '../submenus/submenu.component';



export const MenuRoutes: Routes = [
  { path: '', component: MenusComponent, data: { title: 'Menu' } },
];

export const SubMenuRoutes: Routes = [
  { path: ':id', component: SubMenuComponent, data: { title: 'SubMenu' } },
  { path: '', component: SubMenuComponent, data: { title: 'SubMenu' } }
];