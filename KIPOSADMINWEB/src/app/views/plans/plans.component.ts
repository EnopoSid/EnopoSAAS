import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { plansService } from './plans.service';
import {Router} from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, RolePermissionModel } from '../../helpers/common.interface';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';
import * as $ from "jquery";
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { DatePipe } from '@angular/common';
import { ExportService } from 'src/app/services/common/exportToExcel.service';



@Component({
    selector: 'plans-table',
    templateUrl: './plans.component.html',
    styleUrls: ['./plans.component.css']
})
export class PlansComponent implements OnInit {
    status: boolean;
    displayedColumns = ['sno','PlanName','SubscriptionAmt','DiscountPercentage','Actions']
    dataSource: MatTableDataSource<RolePermissionModel>;
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
    dialogRef: MatDialogRef<any>;
    addPermissionFlag:boolean=false;

    userId: number = 0;
    idOnUpdate: number =0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    currentPageLimit: number = 0;
    limit = MyAppHttp.LIMIT;
    pageLimitOptions = MyAppHttp.PAGE_LIMIT_OPTIONS

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: plansService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public datepipe:DatePipe,
        public exportService:ExportService,
        public translate: TranslateService,
        private formBuilder:FormBuilder) {
            this.formErrors = {
                PlanName: {},
                SubscriptionAmt:{},
                DiscountPercentage:{}

            };
}

    
ngOnInit(){
    this.userId=this.sendReceiveService.globalUserId;
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'PlanName',"Value":" "},
              {"Key":'SubscriptionAmt',"Value":" "},
              {"Key":'DiscountPercentage',"Value":" "}
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
    this.getAllPlans();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });

    
}

updatePagination(){
    this.filterData.dataSource=this.filterData.dataSource;
    this.filterData.dataSource.paginator = this.paginator;
    }
getAllPlans(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllPlans().subscribe((response) => { 
        this.temp=response;
        const menuData: any = [];
        for (let i = 0; i < response.length; i++) {
            response[i].sno = i + 1;
            menuData.push(response[i]);
        }
        this.filterData.gridData = menuData;
        this.dataSource = new MatTableDataSource(menuData);
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



addPermission(){
    this.addPermissionFlag=true;
    this.title="Save"
}
updatePlans(id: number){
    this.service.getPlansById(id)
    this.router.navigate(['plans/addPlans/'+id]);
    this.service.UserView(false);
}
ViewUser()
{
    this.service.UserView(true);
}
deletePlans(id) {
    this.appInfoService.confirmationDialog().subscribe(result=>{ 
        if(!!result){ 
    this.service.deletePlans(id)
        .subscribe((data) => {
            this.getAllPlans();
        }, error => {
            this.formErrors = error
        });
}
});
}



exportToPdf() {
  
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
       
          var col = ['sno','PlanName','SubscriptionAmount','DiscountPercentage'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].PlanName,this.temp[key].SubscriptionAmt,this.temp[key].DiscountPercentage];
                rows.push(temporary);
            }
            let reportname = "plans.pdf"
            this.exportService.exportAsPdf(col,rows,reportname);
        
     
       
    }
    else {
  
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
  }
  exportToExcel() {
    if(this.temp.length!=0 ){
        var rows = [];
  
        
        
        var col = ['sno','PlanName','SubscriptionAmount','DiscountPercentage'];
        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].PlanName,this.temp[key].SubscriptionAmt,this.temp[key].DiscountPercentage];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','PlanName','SubscriptionAmount','DiscountPercentage'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.PlanName +  "</td><td>" + value.SubscriptionAmt + "</td><td>" + value.DiscountPercentage +"</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "Plans.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'plans': ws }, SheetNames: ['plans'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }




}
