import { Routes } from '@angular/router';
import {GetVendorEnquiryComponent } from './vendorenquiry.component';

export const VendorEnquiryRoutes: Routes = [
    {path: '', component: GetVendorEnquiryComponent, data:{title: 'vendorenquiry'}},
];