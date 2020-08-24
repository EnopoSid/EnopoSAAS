import { Routes } from '@angular/router';
import { GetEnquiryComponent } from './enquiry.component';
import { UserAddEnquiryComponent } from './add-enquiry/user/add-enquiry.component';
import { ConsumerAddEnquiryComponent } from './add-enquiry/consumer/add-enquiry.component';



export const EnquiryRoutes: Routes = [
  { path: '', component: GetEnquiryComponent, data: { title: 'Enquiry' } },
  { path: 'addenquiry', component: UserAddEnquiryComponent, data: { title: 'AddEnquiry' }},
  {path:"viewenquiry/:id",component:UserAddEnquiryComponent,data:{title:'ViewEnquiry'}},
];

export const ConsumerRoutes: Routes = [
   { path: 'add', component: ConsumerAddEnquiryComponent , data: { title: 'AddEnquiry' } }
];

