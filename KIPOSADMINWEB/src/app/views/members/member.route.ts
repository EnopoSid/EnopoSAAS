import { Routes } from '@angular/router';
import { GetMemberComponent } from './member.component';
import { GetMemberDetailsComponent } from './membersDetails/memberDetails.component';



export const MemberRoutes: Routes = [
  { path: '', component: GetMemberComponent, data: { title: 'Member' } },

  {path: 'membersDetails/:id', component: GetMemberDetailsComponent , data: { title: 'Member' } },

];

