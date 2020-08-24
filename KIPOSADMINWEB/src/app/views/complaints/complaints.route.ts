import { Routes } from '@angular/router';

import { ComplaintListComponent } from './complaint-list.component';
import { UserAddComplaintComponent } from './add-complaint/user/add-complaint.component';
import { AuthService } from '../../services/auth/auth.service';
import { ConsumerAddComplaintComponent } from './add-complaint/consumer/add-complaint.component'
import { ComplaintSummaryComponent } from './complaint-summary/complaint-summary.component';
import { CheckStatusComponent } from 'src/app/views/consumer/check-status/check-status.component';
 
export const ComplaintsRoutes: Routes = [
  { path: '',   component: ComplaintListComponent, data: { title: 'ComplaintList' } },
  { path: 'addcomplaint', component: UserAddComplaintComponent , data: { title: 'AddComplaint' } },
  { path: 'addcomplaint/:id', component: UserAddComplaintComponent , data: { title: 'AddComplaint' } },
  { path: 'updateComplaint/:id', component: UserAddComplaintComponent , data: { title: 'UpdateComplaint' } },
  { path: 'viewcomplaint/:id',  component: UserAddComplaintComponent, data: { title: 'ViewComplaint' }},
  { path: 'complaintsummary/:id',  component: ComplaintSummaryComponent, data: { title: 'ComplaintSummary' }},
];

export const ConsumerRoutes: Routes = [
 { path: 'add', component: ConsumerAddComplaintComponent , data: { title: 'AddComplaint' } },
 { path: 'checkstatus', component: CheckStatusComponent , data: { title: 'checkstatus' } },
];