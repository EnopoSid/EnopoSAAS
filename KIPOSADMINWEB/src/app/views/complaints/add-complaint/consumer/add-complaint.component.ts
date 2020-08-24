import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef, Input} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef} from '@angular/material';
import {LogoutService} from '../../../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../../../services/common/appInfo.service';
import {SendReceiveService} from '../../../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../../../services/common/myAppHttp.service';
import { ComplaintsService } from './../../complaints.service';
import {LowerCasePipe} from '@angular/common';

import * as $ from 'jquery';


@Component({
    selector: 'consumer-add-complaint-component',
    templateUrl: './add-complaint.component.html',
    // styleUrls: ['./menus.component.css']
})
export class ConsumerAddComplaintComponent implements OnInit {
ngOnInit(){
    
}
}
