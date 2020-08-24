import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {LogoutService} from '../../services/logout/logout.service';
import {Router} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, UserModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { ForgotPasswordUserListService } from './forgotPassword-userList.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';

@Component({
    selector: 'app-forgotPassword-userList',
    templateUrl: './forgotPassword-userList.component.html',
    styleUrls: ['./forgotPassword-userList.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class ForgotPasswordUserListComponent implements OnInit {
    displayedColumns = ['sno','CheckBox','FullName','EmailId','RoleName']
    dataSource: MatTableDataSource<UserModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    formErrors: any;
    title: string;
    rows = [];  
    columns = [];
    temp = [];
    checkedComplaints: number[] = [];
    dialogRef: MatDialogRef<any>;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    requestGrantedUserList=[];

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: ForgotPasswordUserListService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        private formBuilder:FormBuilder) {
            this.formErrors = {
                menuName: {},
                menuUrl: {}
            };
}

    
ngOnInit(){
    this.filterData={  
        filterColumnNames:[
          {"Key":'sno',"Value":" "},
          {"Key":'CheckBox',"Value":" "},
          {"Key":'FullName',"Value":" "},
          {"Key":'EmailId',"Value":" "},
          {"Key":'RoleName',"Value":" "},
        ],
        gridData:  this.gridData,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.getAllChangePasswordRequestUsers();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
}
getAllChangePasswordRequestUsers(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllChangePasswordRequestUsers().subscribe((response) => {
        const userList: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                userList.push(response[i]);
            }
            this.filterData.gridData = userList;
            this.dataSource = new MatTableDataSource(userList);
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
              document.getElementById('preloader-div').style.display = 'none';
    }, (error) => {
          document.getElementById('preloader-div').style.display = 'none';
    });
}

actionAfterError() {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this.router.navigate(['/sessions/signin']);
    });
}

ResetPassword(){
    if(this.requestGrantedUserList.length>0){

        this.service.sendMail(this.requestGrantedUserList).subscribe((response) => {
           this. getAllChangePasswordRequestUsers();
  
            this.requestGrantedUserList = [];
    }, (error) => {

        
    }, () => {

    });
    
    }
}

onCheckboxModalChange(){
    this.requestGrantedUserList = [];
    this.filterData.gridData.forEach((row) => {
       if(row.IsChecked)
        this.requestGrantedUserList.push(row.EmailId);
    });  
}

}
