import { Routes } from '@angular/router';
import { CorpMasterComponent } from './corp-master.component';

export const CorpMasterRoutes: Routes = [
    {path: '', component: CorpMasterComponent, data:{title: 'CorpMaster'}},
];