import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import {SchedulerDetailsService} from './scheduler-details.service';
import { IPageLevelPermissions, RoleModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import * as $ from "jquery";
import { Subject } from 'rxjs';
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'app-scheduler-details',
    templateUrl: './scheduler-details.component.html',
    styleUrls: ['./scheduler-details.component.css']
})
export class SchedulerDetailsComponent implements OnInit {
    displayedColumns = ['sno','JobName','ProcessedDate','StartTime','EndTime','TotalRecords','ProcessedRecords','UnProcessedRecords','JobStatus']
    dataSource: MatTableDataSource<RoleModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    filterData;
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    userId: number = 0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};

    constructor(
        private spinner: NgxSpinnerService,
        public service: SchedulerDetailsService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public exportService:ExportService,
        public datepipe:DatePipe,
        public translate: TranslateService,
        private _router: Router,
        private router:ActivatedRoute,
        private formBuilder:FormBuilder) {
        }
    
    ngOnInit(){
        this.userId=this.sendReceiveService.globalUserId;
          document.getElementById('preloader-div').style.display = 'block';
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'JobName',"Value":" "},
              {"Key":'ProcessedDate',"Value":" "},
              {"Key":'StartTime',"Value":" "},
              {"Key":'EndTime',"Value":" "},
              {"Key":'TotalRecords',"Value":" "},
              {"Key":'ProcessedRecords',"Value":" "},
              {"Key":'UnProcessedRecords',"Value":" "},
              {"Key":'JobStatus',"Value":" "},
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
        this.getschedulerDetails();
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;  
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
    }
    
    getschedulerDetails(){
        document.getElementById('preloader-div').style.display = 'block';
        this.service.getschedulerDetails().subscribe((response) => {
            this.temp=response;
            const schedulerData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                schedulerData.push(response[i]);
                if(response[i].JobStatus==1){
                    response[i].JobStatus="Success";
                }else{
                    response[i].JobStatus="Failure";
                }
            }
            this.filterData.gridData = schedulerData;
            this.dataSource = new MatTableDataSource(schedulerData);
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
             document.getElementById('preloader-div').style.display = 'none';
    },(error) => {
        document.getElementById('preloader-div').style.display = 'none';
        });
    }
    updatePagination(){
       this.filterData.dataSource=this.filterData.dataSource;
       this.filterData.dataSource.paginator = this.paginator;
      }
    actionAfterError() {
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this._router.navigate(['/sessions/signin']);
        });
    }
    exportToPdf() {
      
        if(this.temp.length!=0){
            var doc = new jsPDF();
            var rows = [];
           
              var col = ['sno','JobName','ProcessedDate','StartTime','EndTime','Total   Records','ProcessedRecords','Un      Processed   Records','JobStatus'];
                for(var key in this.temp){
                    var temporary = [(parseInt(key) +1), this.temp[key].JobName,(this.datepipe.transform(this.temp[key].ProcessedDate,'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].StartTime,'hh:mm')),(this.datepipe.transform(this.temp[key].EndTime,'hh:mm')),this.temp[key].TotalRecords,this.temp[key].ProcessedRecords,this.temp[key].UnProcessedRecords,this.temp[key].JobStatus];
                    rows.push(temporary);
                }
                let reportname = "Schedulerdetails.pdf"
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
                var temporary = [(parseInt(key) +1), this.temp[key].JobName,(this.datepipe.transform(this.temp[key].ProcessedDate,'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].StartTime,'hh:mm')),(this.datepipe.transform(this.temp[key].EndTime,'hh:mm')),this.temp[key].TotalRecords,this.temp[key].ProcessedRecords,this.temp[key].UnProcessedRecords,this.temp[key].JobStatus];
                rows.push(temporary);
            }
        
            var createXLSLFormatObj = [];
            var xlsHeader =  ['sno','JobName','ProcessedDate','StartTime','EndTime','TotalRecords','ProcessedRecords','UnProcessedRecords','JobStatus']
            createXLSLFormatObj.push(xlsHeader);
         $.each(rows, function(index, value) {
                var innerRowData = [];
               $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.JobName +  "</td><td>" + value.ProcessedDate +  "</td><td>" + value.StartTime +  "</td><td>" + value.EndTime +   "</td><td>" +value.TotalRecords + "</td><td>" +value.ProcessedRecords + "</td><td>" +value.UnProcessedRecords + "</td><td>" +value.JobStatus + "</td></tr>");
      
            
                $.each(value, function(ind, val) {
        
                    innerRowData.push(val);
                });
                createXLSLFormatObj.push(innerRowData);
            });
            var filename = "Schedulerdetails.xlsx";
            var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
            const workbook: XLSX.WorkBook = { Sheets: { 'Schedulerdetails': ws }, SheetNames: ['Schedulerdetails'] };
            XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
        }
        else{
            this.sendReceiveService.showDialog('There is No Data Available to Export');
        }
      }
      
      
      
      
}
