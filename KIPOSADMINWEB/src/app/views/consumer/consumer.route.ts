import { Routes } from '@angular/router';
import { ConsumerComponent } from './consumer.component';



export const ConsumerRoute: Routes = [
   { path: '', component: ConsumerComponent , data: { title: 'Consumer' } },
  { path: 'complaints/consumer', component: ConsumerComponent, data: { title: 'ConsumerComplaint' }}
];