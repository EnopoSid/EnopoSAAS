import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {UsersService} from './users.service';
import {Router, ActivatedRoute} from '@angular/router';
import { AppInfoService } from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, UserModel } from '../../helpers/common.interface';
import { NavigationService } from '../../services/navigation/navigation.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';
import * as $ from "jquery";
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { DatePipe } from '@angular/common';
import { ExportService } from 'src/app/services/common/exportToExcel.service';

@Component({
    selector: 'users-table',
    templateUrl: './users.component.html',
    encapsulation: ViewEncapsulation.None
})


export class GetUsersComponent implements OnInit {
    displayedColumns = ['sno','UserId','FullName','FirstName','LastName','StoreName','Actions']
    dataSource: MatTableDataSource<UserModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    private currentComponentWidth: number;
    temp = [];
    dialogRef: MatDialogRef<any>;
    usersFromServiceExists ;
    sample : string;
    pagePermissions:IPageLevelPermissions = {View: false, Edit: false, Delete: false,Add: false};


    constructor(private spinner: NgxSpinnerService,
                private router: Router,
                private route:ActivatedRoute,
                public service: UsersService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService,
                public datepipe:DatePipe,
                public exportService:ExportService,  
                private actRoute: ActivatedRoute,
                private activatedRoute: ActivatedRoute,
             ) {
    }

    ngOnInit() {
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'UserId',"Value":" "},
              {"Key":'FullName',"Value":" "},
              {"Key":'FirstName',"Value":" "},
              {"Key":'LastName',"Value":" "},
              {"Key":'StoreName',"Value":" "},
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
        this.getAllUsers();
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;  
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
      
    }

    handlePageChange (event: any): void {
    }


    updatePagination(){
        this.filterData.dataSource=this.filterData.dataSource;
        this.filterData.dataSource.paginator = this.paginator;
        }
    openResume(filePath) {
        window.open(filePath);
    }

    getAllUsers() {
        document.getElementById('preloader-div').style.display = 'block';
            this.service.getAllUsers().subscribe((response) => {
                this.temp=response;
                const userData: any = [];
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1;
                    userData.push(response[i]);
                }
                this.filterData.gridData = userData;
                this.dataSource = new MatTableDataSource(userData);
                this.filterData.dataSource=this.dataSource;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                  document.getElementById('preloader-div').style.display = 'none';

            }, (error) => {

                  document.getElementById('preloader-div').style.display = 'none';

            });
    }

    actionAfterError () {
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this.router.navigate(['/sessions/signin']);
        });
    }
    editUser(id: number){
        this.service.getUsersListById(id)
        this.router.navigate(['/users/update/'+id]);
        this.service.UserView(false);
    }
    ViewUser()
    {
        this.service.UserView(true);
    }
    deleteUser(id) {
        this.appInfoService.confirmationDialog().subscribe(result=>{ 
            if(!!result){ 
                this.service.deleteUser(id).subscribe((data) => {
                    this.getAllUsers();
                }, error => {

                });
        }   
    }); 
}

exportToPdf() {
  
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
       
          var col = ['sno','UserId','FullName','FirstName','LastName','StoreName'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].UserId,this.temp[key].FullName,this.temp[key].FirstName,this.temp[key].LastName,this.temp[key].StoreName];
                rows.push(temporary);
            }
            let reportname = "Users.pdf"
            this.exportService.exportAsPdf(col,rows,reportname);
        
     
       
    }
    else {
  
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
  }
  exportToExcel() {
    if(this.temp.length!=0 ){
        var rows = [];
  
        
        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].UserId,this.temp[key].FullName,this.temp[key].FirstName,this.temp[key].LastName,this.temp[key].StoreName];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','UserId','FullName','FirstName','LastName','StoreName'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.UserId +  "</td><td>" + value.FullName +  "</td><td>" + value.FirstName +  "</td><td>" + value.LastName +   "</td><td>" +value.StoreName + "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "Users.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'Users': ws }, SheetNames: ['Users'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }


}

