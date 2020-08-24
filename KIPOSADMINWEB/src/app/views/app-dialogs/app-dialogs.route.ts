import { Routes } from '@angular/router';


export const DialogsRoutes: Routes = [
  {
    path: '',
    children: [{
      path: 'confirm',
      data: { title: 'Confirm', breadcrumb: 'CONFIRM' },
    }, {
      path: 'loader',
      data: { title: 'Loader', breadcrumb: 'LOADER' },
    }]
  }
];
