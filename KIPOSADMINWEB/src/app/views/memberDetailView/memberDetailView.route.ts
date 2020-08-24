import { Routes } from '@angular/router';
import { GetMemberDetailViewComponent } from './memberDetailViewd.component';



export const MemberDetailViewRoutes: Routes = [
  { path: 'memberdetailview/:id', component: GetMemberDetailViewComponent, data: { title: 'GetMemberDetailView' } },
  ];


