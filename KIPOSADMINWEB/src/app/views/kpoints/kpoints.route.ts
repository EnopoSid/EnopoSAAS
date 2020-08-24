import { Routes } from '@angular/router';
import { KpointsComponent } from './kpoints.component';

import { addManualKpoints } from './addManualKpoints/addKpoints.component';


export const kpointsRoutes: Routes = [
  { path: '', component: KpointsComponent, data: { title: 'kpointsRoutes' } },
  { path: 'kpoints', component: KpointsComponent, data: { title: 'kpoints' } },
  { path: 'addManualKpoints', component: addManualKpoints, data: { title: 'Add-' } },
];