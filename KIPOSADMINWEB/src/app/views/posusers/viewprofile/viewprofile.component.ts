import {Component, OnInit, ViewEncapsulation,Input, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef} from '@angular/material';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { IPageLevelPermissions } from '../../../helpers/common.interface';
import MyAppHttp from '../../../services/common/myAppHttp.service';
import { TopbarComponent } from '../../../components/common/topbar/topbar.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { PosUsersService } from 'src/app/views/posusers/posusers.service';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'view-profile-table',
    templateUrl: './ViewProfile.component.html',
    encapsulation: ViewEncapsulation.None
})
export class GetPosUserProfileComponent implements OnInit {

    id: number = 0;
    title: string = "User";

    userId: number = this.sendReceiveService.globalUserId;
    

    constructor(private spinner: NgxSpinnerService,
        private router: Router,
        public service: PosUsersService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        private formBuilder: FormBuilder,
        public translate: TranslateService,
        fb: FormBuilder,  
        private _router: Router,private route: ActivatedRoute,
    ) {
    }
    ngOnInit() {
        
    }
   
}