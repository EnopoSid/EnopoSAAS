import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { storesService } from './stores.service';
import {Router} from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, StoresModel } from '../../helpers/common.interface';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';
import * as $ from "jquery";
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { DatePipe } from '@angular/common';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { ComfirmComponent } from '../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';



@Component({
    selector: 'stores-table',
    templateUrl: './stores.component.html',
    styleUrls: ['./stores.component.css']
})
export class StoresComponent implements OnInit {
    status: boolean;
    displayedColumns = ['sno','Name','Address','Actions']
    dataSource: MatTableDataSource<StoresModel>;
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
    PermissionForm:FormGroup;
    userId: number = 0;
    idOnUpdate: number =0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    currentPageLimit: number = 0;
    limit = MyAppHttp.LIMIT;
    pageLimitOptions = MyAppHttp.PAGE_LIMIT_OPTIONS;
    checked = true;

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: storesService,
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
                Name: {},
            };
}

    
ngOnInit(){
    this.userId=this.sendReceiveService.globalUserId;
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'Name',"Value":" "},
              {"Key":'Address',"Value":" "},
               ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
    this.getAllStores();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
}
getAllStores(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getPickupAddress().subscribe((response) => { 
        this.temp=response;
        const storesData: any = [];
        for (let i = 0; i < response.length; i++) {
            response[i].sno = i + 1;
            if(response[i].StorePickupPoint==0){
                response[i].IsActive=false;
            }else if(response[i].StorePickupPoint==1){
                response[i].IsActive=true; 
            }

            storesData.push(response[i]);
        }
        this.filterData.gridData = storesData;
        this.dataSource = new MatTableDataSource(storesData);
        this.filterData.dataSource=this.dataSource;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
          document.getElementById('preloader-div').style.display = 'none';
    }, (error) => {
        document.getElementById('preloader-div').style.display = 'none';
    });
}

addPermission(){
    this.addPermissionFlag=true;
    this.title="Save"
}

onCancel(){
    this.idOnUpdate=0;
    this.PermissionForm.reset();
    this.addPermissionFlag=false;
}



exportToPdf() {
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
       
          var col = ['sno','Name','Address'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].Name,this.temp[key].Address];
                rows.push(temporary);
            }
            let reportname = "Stores.pdf"
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
            var temporary = [(parseInt(key) +1), this.temp[key].Name,this.temp[key].Address];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','Name','Address'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.Key +  "</td><td>" + value.Value+ "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "Stores.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'Stores': ws }, SheetNames: ['Stores'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }
  activateRecord(event, id) {
     if(event==true){
         var message ="Do you want to Enable Pick Up Point?"
     }
     else if(event==false){
         
        var message ="Do you want to Disable Pick Up Point?"

     }
    const dialogRef = this.dialog.open(ComfirmComponent, {
        
      width: '300px',
      height: "180px",
      data: message
    
    });


    dialogRef.afterClosed().subscribe(result => {
      if (!!result) {
          let stroreFlag=0;
          if(!!event){
            stroreFlag=1;
          }else if(!event){
              stroreFlag=0;
          }
        var obj={
            "StorePickupPoint1":stroreFlag
        }
        this.service.activateRecord(obj,id).subscribe((data) => {
         this.getAllStores();
        }, error => {

        });
      }else{
        this.getAllStores(); 
      }
    });
}

}


