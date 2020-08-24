import { Routes } from '@angular/router';
import { RoleComponent } from 'src/app/views/roles/role.component';
import { RolePermissionComponent } from 'src/app/views/roles/rolepermissions/rolepermission.component';



export const RoleRoutes: Routes = [
  { path: '', component: RoleComponent, data: { title: 'Role' } }
];

export const RolePermissionsRoutes: Routes = [
  { path: ':id', component: RolePermissionComponent, data: { title: 'Rolepermission' } },
  { path: '', component: RolePermissionComponent, data: { title: 'Rolepermission' } }
];