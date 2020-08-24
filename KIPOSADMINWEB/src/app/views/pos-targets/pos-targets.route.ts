import { Routes } from '@angular/router';
import { POSTargetsComponent } from './pos-targets.component';
import { AddPOSTargetsComponent } from './addPOSTargets/addPOSTargets.component';



export const  POSTargetsRoutes: Routes = [
  { path: '', component: POSTargetsComponent, data: { title: 'POSTargets' } },
  { path: 'postargets', component: POSTargetsComponent, data: { title: 'POSTargets' } },
  { path: 'addPOSTargets', component: AddPOSTargetsComponent, data: { title: 'Add-POS-Targets' } },
  { path: 'addPOSTargets/:id', component: AddPOSTargetsComponent, data: { title: 'Edit-POS-Targets' } },
  
];


