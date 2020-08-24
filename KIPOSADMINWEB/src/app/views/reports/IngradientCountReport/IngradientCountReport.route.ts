import { Routes } from '@angular/router';
import { IngradientCountReportComponent } from './IngradientCountReport.component';

export const IngradientCountRoute: Routes = [
    {path: '', component: IngradientCountReportComponent, data:{title: 'IngradientCountReport'}},
    
];