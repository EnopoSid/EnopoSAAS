import { Routes } from '@angular/router';
import { AllReportsComponent } from './all-reports.component';

export const AllReportsRoute: Routes = [
    {path: '', component: AllReportsComponent, data:{title: 'AllReports'}},
    
];