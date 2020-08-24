import { Routes } from '@angular/router';


import { CommunicationComponent } from './communication.component';


export const CommunicationRoutes: Routes = [
  { path: '', component: CommunicationComponent, data: { title: 'Communication' } }
];