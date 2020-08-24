import { Routes } from '@angular/router';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { SigninComponent } from './signin/signin.component';

export const SessionsRoutes: Routes = [
    {
        path: '',
        children: [
         {
            path: 'signin',
            component: SigninComponent,
            data: { title: '' }
        },  
        {
            path: 'forgot-password',
            component: ForgotPasswordComponent,
            data: { title: 'Forgot password' }
        },  
        {
            path: '404',
            component: SigninComponent,
            data: { title: '' }
        }, 
    ]
    }
];
