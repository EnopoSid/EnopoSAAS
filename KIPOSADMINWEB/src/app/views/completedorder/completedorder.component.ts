import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {LogoutService} from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, RolePermissionModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { completedorderService } from './completedorder.service';
import { TranslateService } from '@ngx-translate/core';
import * as $ from "jquery";
import { Subject } from 'rxjs';
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { DatePipe } from '@angular/common';
import { ComfirmComponent } from '../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
import { AppComfirmComponent } from '../app-dialogs/app-confirm/app-confirm.component';
@Component({
    selector: 'app-rolepermissions',
    templateUrl: './completedorder.component.html'
})
export class completedorderComponent implements OnInit {
    displayedColumns = ['sno','CustomerName','OrderId','OrderStatus','StoreName','ModifiedDate']
    dataSource: MatTableDataSource<RolePermissionModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    permission: any;
    id:number = 0;
    saveRole:boolean = false;
    @ViewChild('myTable', {static: false}) table: any;
    @ViewChild('tableWrapper', {static: false}) tableWrapper;
    dialogRef: MatDialogRef<any>;
    editRole:boolean = true; 
    rolepermissions:any;
    PermissionId: string;
    role: string;
    permissionId:number;
    selectedRow : Number;
    orderId:number;
    permisssionList:any;
    status:boolean ;
    public permissionname ;
    permissionRole :boolean = true;
    userId: number = 0;
    permissionType = false;
    RolePermissionForm: FormGroup;
    rows = [];
    temp = [];
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    rolename: string;

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: completedorderService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
         public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        public datepipe:DatePipe,
        public exportService:ExportService,
        private formBuilder:FormBuilder,
        private route:ActivatedRoute,
        private _router: Router,) { 
        }
  
    ngOnInit() {
      this.userId=this.sendReceiveService.globalUserId;
      this.filterData={
        filterColumnNames:[
            {"Key":'sno',"Value":" "},
            {"Key":'CustomerName',"Value":" "},
            {"Key":'OrderId',"Value":" "},
            {"Key":'OrderStatus',"Value":""},
            {"Key":'StoreName',"Value":" "},
            {"Key":'ModifiedDate',"Value":" "},
          


        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
      this.RolePermissionForm = this.formBuilder.group({
        'RefundStatusId':  [null]
    });
        this.id = 2 ;
        
        if(!isNaN(this.id))
        {
        this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
          this.rolename = resp;
        
        })
      }
      
        
        this.getAllCompleteorders();
       
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;  
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });

    }
  

    getAllCompleteorders() {
      document.getElementById('preloader-div').style.display = 'block';
  this.service.getCompletedorder().subscribe((response) => {
    this.temp=response;
      const orderData: any = [];              
      for (let i = 0; i < response.length; i++) {
          response[i].sno = i + 1;
          response[i].editMode = false;
          orderData.push(response[i]);
          this.rows.push(response[i]);
      }
      this.filterData.gridData = orderData;
      this.dataSource = new MatTableDataSource(orderData);
      this.filterData.dataSource=this.dataSource;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    
        document.getElementById('preloader-div').style.display = 'none';
  }, (error) => {
        document.getElementById('preloader-div').style.display = 'none';
  });
}

exportToPdf() {

  if(this.temp.length!=0){
      var doc = new jsPDF();
      var rows = [];
     
        var col = ['sno','CustomerName','OrderId','OrderStatus','StoreName','OrderDate'];
          for(var key in this.temp){
              var temporary = [(parseInt(key) +1), this.temp[key].CustomerName,this.temp[key].OrderId,this.temp[key].OrderStatus,this.temp[key].StoreName,(this.datepipe.transform(this.temp[key].ModifiedDate, 'yyyy/MM/dd'))];
              rows.push(temporary);
          }
          let reportname = "Completeorders.pdf"
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
        var temporary = [(parseInt(key) +1), this.temp[key].CustomerName,this.temp[key].OrderId,this.temp[key].OrderStatus,this.temp[key].StoreName,(this.datepipe.transform(this.temp[key].ModifiedDate, 'yyyy/MM/dd'))];
        rows.push(temporary);
    }
  
      var createXLSLFormatObj = [];
      var xlsHeader =  ['sno','CustomerName','OrderId','OrderStatus','StoreName','OrderDate'];
      createXLSLFormatObj.push(xlsHeader);
   $.each(rows, function(index, value) {
          var innerRowData = [];
         $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.CustomerName +  "</td><td>" + value.OrderId +  "</td><td>" + value.OrderStatus +  "</td><td>" + value.StoreName +   "</td><td>" +value.orderDate +  "</td></tr>");

      
          $.each(value, function(ind, val) {
  
              innerRowData.push(val);
          });
          createXLSLFormatObj.push(innerRowData);
      });
      var filename = "Completeorders.xlsx";
      var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
      const workbook: XLSX.WorkBook = { Sheets: { 'Completeorders': ws }, SheetNames: ['Completeorders'] };
      XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
  }
  else{
    this.sendReceiveService.showDialog('There is No Data Available to Export');
}
}



 onGoBack(){
    this._router.navigate(['../'],{relativeTo: this.route}); 
  }
  updatePagination(){
   this.filterData.dataSource=this.filterData.dataSource;
   this.filterData.dataSource.paginator = this.paginator;
  }

}