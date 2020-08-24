import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { LogoutService } from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import { AppInfoService } from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import {ResetpasswordService} from './resetpassword.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, EnquiryModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';
import { CloseScrollStrategy } from '@angular/cdk/overlay';
import { FormGroup, Validators ,FormControl} from '@angular/forms';
import { FormBuilder } from '@angular/forms';
import { InfoDialogComponent } from '../app-dialogs/info-dialog/info-dialog.component';
import { empty } from 'rxjs';




@Component({
    selector: 'rese-table',
    templateUrl: './resetpassword.component.html',
    encapsulation: ViewEncapsulation.None
})

export class ResetPasswordComponent implements OnInit {
    displayedColumns ;
    dataSource: MatTableDataSource<EnquiryModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
   resetpasswordForm:FormGroup;
    filterData;
    private currentComponentWidth: number;
    rows = [];
    columns = [];
    temp = [];
    title: string;
    dialogRef: MatDialogRef<any>;
    usersFromServiceExists ;
    AddMenuFlag:boolean=false;
    datalode :boolean=false;
    datanotfound:boolean=false;
    sample : string;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    urlpath : string;
    isShow = true ;
    nonmember:boolean;
    additionalkpoints: boolean=false;
    navservice: any;
    idOnUpdate: any;
    formErrors: any;
    custData:any[];
    EmaiilId:string;
    MobileNumber:string;
    MemberId:string;
    notificationMessage:string;
    Email:string;
    FullName:string;
    Password:string;
    obj:any;
    
    constructor(private spinner: NgxSpinnerService,
                private router: Router,
                private route:ActivatedRoute,
                public resetpasswordservice: ResetpasswordService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService,  
                private actRoute: ActivatedRoute,
                private activatedRoute: ActivatedRoute,
                private formBuilder:FormBuilder ,
               
                
            ) {
                this.formErrors = {
                   EmailId:null
                }
    }



    ngOnInit() {
      

        this.resetpasswordForm = this.formBuilder.group({
            'EmailId': [null,Validators.compose([Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.emailidpattern)])],
        });
   
      
    }

    getDetails()
    {
        let email = this.resetpasswordForm.controls['EmailId'].value
        this.resetpasswordservice.getAllMemberLoginCustomers(email).subscribe((response) => {
           
           
           this.Email=response.Email,
           this.FullName=response.FullName,
           this.MemberId=response.MemberId,
           this.Password=response.Password,
           this.MobileNumber=response.MobileNumber
           this.obj ={
           "EmailId": this.Email,
           "OldPassword": this.Password}
           console.log(this.obj);
           if(response!=null)
           {
               this.datalode=true;
           }
           else{
            this.datanotfound=true;
           }
              document.getElementById('preloader-div').style.display = 'none';
        }, (error) => {
            if(error.statusText="Not Found" || error.status == 500)
            this.datanotfound=true;
            this.datalode=false;
             document.getElementById('preloader-div').style.display = 'none';
        });
    }
    
    onCancel(){
        this.custData=[];
        this.datalode=false;
        this.datanotfound=false;
        this.resetpasswordForm.reset();
        for( let i in this.resetpasswordForm.controls ) {

            this.resetpasswordForm.controls[i].setErrors(null);
        }
       
    }
   
    
    resetpassword()
    {
        let confirmation :boolean = window.confirm("Do you really want to reset password");
        if(confirmation==true)
        {
          document.getElementById('preloader-div').style.display = 'block'; 
        console.log(this.obj);
        this.resetpasswordservice.ChangePassword(this.obj).subscribe((response) => {
              this.showDialog("Email with instruction has been sent to "+this.FullName);
               this.onCancel();
                document.getElementById('preloader-div').style.display = 'none';
            },
            (error) => {
                 document.getElementById('preloader-div').style.display = 'none';
            });
        }
    }
    showDialog(message){
        let dialogInstance = this.dialog.open(
            InfoDialogComponent, {
            data: {
              message: message  
            }
          });
    
          setTimeout(() => {
            dialogInstance.close();
          }, 5000);
    }
}