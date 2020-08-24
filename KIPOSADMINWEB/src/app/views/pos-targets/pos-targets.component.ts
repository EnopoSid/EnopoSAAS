import { Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort } from '@angular/material';
import { LogoutService } from '../../services/logout/logout.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AppInfoService } from '../../services/common/appInfo.service';
import { SendReceiveService } from '../../services/common/sendReceive.service';
import { POSTargetsService } from './pos-targets.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, EnquiryModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';
import { CloseScrollStrategy } from '@angular/cdk/overlay';
import { AppComfirmComponent } from '../app-dialogs/app-confirm/app-confirm.component';
declare var jsPDF: any;  
import { DatePipe, NgIf } from '@angular/common';
import {ExportService } from '../../services/common/exportToExcel.service';
import { Template } from '@angular/compiler/src/render3/r3_ast';
import * as $ from 'jquery';
import * as XLSX from 'xlsx';

@Component({
    selector: 'pos-targets-table',
    templateUrl: './pos-targets.component.html',
    encapsulation: ViewEncapsulation.None
})

export class POSTargetsComponent implements OnInit {
    displayedColumns = ['sno', 'StoreName', 'StartDate', 'EndDate','TargetBowlsCount', 'TargetSalesAmt','Actions']
    dataSource: MatTableDataSource<EnquiryModel>;
    gridData = [];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1: boolean = false;
    messageFlag2: boolean = false;
    filterData;
    private currentComponentWidth: number;
    rows = [];
    columns = [];
    temp = [];
    permisssionList: any;
    orderForm: FormGroup;
    dialogRef: MatDialogRef<any>;
    usersFromServiceExists;
    sample: string;
    pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };

    constructor(private spinner: NgxSpinnerService,
        private router: Router,
        private route: ActivatedRoute,
        public service: POSTargetsService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        private formBuilder: FormBuilder,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        public datepipe:DatePipe,
        public exportService:ExportService,
        private actRoute: ActivatedRoute,
        private activatedRoute: ActivatedRoute, 
        ) {
    }



    ngOnInit() {
        this.rows = [];       
        this.filterData = {
            filterColumnNames: [
                { "Key": 'sno', "Value": " " },
                { "Key": 'StoreName', "Value": " " },
                { "Key": 'StartDate', "Value": " " },
                { "Key": 'EndDate', "Value": " " },   
                { "Key": 'TargetBowlsCount', "Value": " " },
                { "Key": 'TargetSalesAmt', "Value": " " },
          
            ],
            gridData: this.gridData,
            dataSource: this.dataSource,
            paginator: this.paginator,
            sort: this.sort
        };
        this.orderForm = this.formBuilder.group({
            'orderstatusId': [null]
        });
        this.getAllPOSTargets();
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
            this.pagePermissions = pageLevelPermissions.response;          
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
    }

    handlePageChange(event: any): void {
    }


    getAllPOSTargets() {
        document.getElementById('preloader-div').style.display = 'block';
        this.service.getAllPOSTargets().subscribe((response) => {
            this.temp=response;
            const orderData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                orderData.push(response[i]);
                this.rows.push(response[i]);
            }
            this.filterData.gridData = orderData;
            this.dataSource = new MatTableDataSource(orderData);
            this.filterData.dataSource = this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
             document.getElementById('preloader-div').style.display = 'none';;
        }, (error) => {
             document.getElementById('preloader-div').style.display = 'none';;
        });
    }

    actionAfterError() {      
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this.router.navigate(['/sessions/signin']);
        });
    }
    

    updatePagination(){
        this.filterData.dataSource=this.filterData.dataSource;
        this.filterData.dataSource.paginator = this.paginator;
        }
    

    deletePOSTarget(id) {
        console.log('id: '+id)
       
        this.dialogRef = this.dialog.open(AppComfirmComponent, {
          width: '300px',
          height:"180px",
        });
        this.dialogRef.componentInstance.message = 'Do you want to Delete?';
        this.dialogRef.afterClosed().subscribe(result => {
            this.dialogRef = null;
            if(result == true){ 
                this.service.deletePOSTarget(id).subscribe((data) => {
                    console.log('success: '+JSON.stringify(data))
                    this.getAllPOSTargets();
                }, error => {
                  console.log('ERROR >>> '+error)
                });
        }   
      }); 
      }
      exportToPdf() {
      
        if(this.temp.length!=0){
            var doc = new jsPDF();
            var rows = [];
           
              var col = ['sno','StoreName','TargetStart Date ','TargetEnd Date','TargetBowels Count ','TargetSales Amount'];
                for(var key in this.temp){
                    var temporary = [(parseInt(key) +1), this.temp[key].StoreName,(this.datepipe.transform(this.temp[key].StartDate,'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].EndDate, 'yyyy/MM/dd')),this.temp[key].TargetBowlsCount,this.temp[key].TargetSalesAmt];
                    rows.push(temporary);
                }
                let reportname = "pos-Targets.pdf"
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
                var temporary = [(parseInt(key) +1), this.temp[key].StoreName,(this.datepipe.transform(this.temp[key].StartDate,'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].EndDate, 'yyyy/MM/dd')),this.temp[key].TargetBowlsCount,this.temp[key].TargetSalesAmt];
                rows.push(temporary);
            }
        
            var createXLSLFormatObj = [];
            var xlsHeader =  ['sno','StoreName','TargetStart Date ','TargetEnd Date','TargetBowels Count ','TargetSales Amount '];
            createXLSLFormatObj.push(xlsHeader);
         $.each(rows, function(index, value) {
                var innerRowData = [];
               $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.StoreName +  "</td><td>" + value.StartDate +  "</td><td>" + value.EndDate +  "</td><td>" + value.TargetBowlsCount +   "</td><td>" +value.TargetSalesAmt + "</td></tr>");
      
            
                $.each(value, function(ind, val) {
        
                    innerRowData.push(val);
                });
                createXLSLFormatObj.push(innerRowData);
            });
            var filename = "pos-Targets.xlsx";
            var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
            const workbook: XLSX.WorkBook = { Sheets: { 'pos-Targets': ws }, SheetNames: ['pos-Targets'] };
            XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
        }
      }


}