import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef, OnDestroy} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort, PageEvent} from '@angular/material';
import {LogoutService} from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, RolePermissionModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { pendingorderService } from './pendingorder.service';
import { TranslateService } from '@ngx-translate/core';
import * as $ from "jquery";
declare var  jsPDF : any;
import * as XLSX from 'xlsx';
import {DatePipe} from '@angular/common';
import { Subject, observable, Observable } from 'rxjs';
import { ComfirmComponent } from '../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
import { AppComfirmComponent } from '../app-dialogs/app-confirm/app-confirm.component';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
@Component({
    selector: 'app-rolepermissions',
    templateUrl: './pendingorder.component.html'
})
export class pendingorderComponent implements OnInit,OnDestroy {
 
    displayedColumns = ['select','sno','CustomerName','OrderId','StoreName','Actions']
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
    splicedData=[];
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    rolename: string;
    interval;
    adminInterval;
    orderList=[];
    IsCompleteToButtonClicked=false;
    pageEvent: PageEvent;
    pageLength;
    checked=false;
    checkBoxEnabled=true;

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: pendingorderService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
         public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        public datepipe:DatePipe,
        public exportService :ExportService,
        private formBuilder:FormBuilder,
        private route:ActivatedRoute,
        private _router: Router,) { 
        }
  
    ngOnInit() {
      this.userId=this.sendReceiveService.globalUserId;
      this.filterData={
        filterColumnNames:[
          {"Key":'select',"Value":" "},
            {"Key":'sno',"Value":" "},
            {"Key":'CustomerName',"Value":" "},
            {"Key":'OrderId',"Value":" "},
            {"Key":'StoreName',"Value":" "}
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
      
     this.getAllConfigurations();
       
       
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;  
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });

    }
  

getAllPendingorders() {
  this.IsCompleteToButtonClicked=false;
      this.service.getPendingorders().subscribe((response) => {
        this.temp=response;
        const orderData: any = [];     
        this.rows=[];         
        this.checked=false;
        this.checkBoxEnabled=response.length==0?false:true;
        for (let i = 0; i < response.length; i++) {
            response[i].sno = i + 1;
            response[i].editMode = false;
            response[i].isSelected=false;
            orderData.push(response[i]);
            this.rows.push(response[i]);
        }
        this.filterData.gridData = orderData;
        this.dataSource = new MatTableDataSource(orderData);
        this.filterData.dataSource=this.dataSource;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        for (let i = 0; i < this.filterData.filterColumnNames.length; i++) {
          this.filterData.filterColumnNames[i].Value ='';
        }
          document.getElementById('preloader-div').style.display = 'none';
    }, (error) => {
          document.getElementById('preloader-div').style.display = 'none';
    });   
 
  
}
clearFilters(){
  this.dataSource.filter = '';
}
routerOnActivate() {
  this.getAllPendingorders();
}
routerOnDeactivate() {
  clearInterval(this.adminInterval);

}
updatePagination(){
 this.filterData.dataSource=this.filterData.dataSource;
 this.filterData.dataSource.paginator = this.paginator;
}
updateStausOfOrder(id){
    this.orderId=id;
    this.service.UpdateOrderStaus(this.orderId).subscribe((data) => {
      this.getAllPendingorders(); 
    }, error => {

    });
}

AddOrderIds(){

}

UpdateStatusOrderList(orderIds){
  this.orderList=orderIds;
  this.service.UpdateStatusOrderList(this.orderList).subscribe((data) => {
    this.getAllPendingorders(); 
  }, error => {

  });
}


ngOnDestroy(): void {
  clearInterval(this.interval);
}

  onGoBack(){
    this._router.navigate(['../'],{relativeTo: this.route}); 
  }

  cancelOrderStatus(id){
    this.orderId=id;
    const dialogRef = this.dialog.open(AppComfirmComponent, {
      width: "300px",
      height:"160px",
      data: 'Do you want to cancel order?'
      });
   
       dialogRef.afterClosed().subscribe(result => {
        this.dialogRef = null;
        if(!!result){ 
            this.service.CancelOrderStaus(this.orderId).subscribe((data) => {
              this.getAllPendingorders(); 
            }, error => {

            });
          }
  });
}
getAllConfigurations(){
  document.getElementById('preloader-div').style.display = 'block';
  this.service.getAllConfigurations().subscribe((response) => {
   if(response){
    var configuration  =  response.filter(function (e) {
      return e.Key == 'ordersloadingTime'
    });
    this.adminInterval  = parseInt(configuration[0].Value) ;
   
   }else{
    this.adminInterval = 5000;
   }
   this.getAllPendingorders();

  }, (error) => {
      document.getElementById('preloader-div').style.display = 'none';
      });
}
exportToPdf() {
  if(this.temp.length!=0){
      var doc = new jsPDF();
      var rows = [];
      var col = ['sno','Customer Name','OrderId','Store  Name'];
      for(var key in this.temp){
          var temporary = [(parseInt(key) +1), this.temp[key].CustomerName,this.temp[key].OrderId,this.temp[key].StoreName];
          rows.push(temporary);
      }
      let reportname = "OnlineOrders.pdf"
      this.exportService.exportAsPdf(col,rows,reportname);
  }
  else {
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
}
exportToExcel() {
 
  if(this.temp.length!=0){
       var rows = [];
     
      for(var key in this.temp){
        var temporary = [(parseInt(key) +1), this.temp[key].CustomerName,this.temp[key].OrderId,this.temp[key].StoreName];
          rows.push(temporary);
      }
      var createXLSLFormatObj = [];
      var xlsHeader = ['sno','Customer Name','OrderId','order status','Store  Name']; 
      createXLSLFormatObj.push(xlsHeader);
      $.each(rows, function(index, value) {
             var innerRowData = [];
            $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.CustomerName +  "</td><td>" + value.OrderId  +  "</td><td>" + value.StoreName+"</td></tr>");

         
             $.each(value, function(ind, val) {
     
                 innerRowData.push(val);
             });
             createXLSLFormatObj.push(innerRowData);
         });
         var filename = "PosOrders.xlsx";
         var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
         const workbook: XLSX.WorkBook = { Sheets: { 'PosOrders': ws }, SheetNames: ['PosOrders'] };
         XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
     }
  else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
}

onSelectOfOrder(event,row){
  if(!!event.checked){
    row.isSelected=true;
  }
  else{
    row.isSelected=false;
  }
  var orders=this.rows.filter(x=>x.isSelected==true);
  if(orders.length>0){
    this.IsCompleteToButtonClicked=true;
  }else{
    this.IsCompleteToButtonClicked=false;
  }
}

onChangeStatusToCompleteClk(){
  this.rows.filter(x=>{
    if(x.isSelected==true){
      this.orderList.push(x.OrderId);
    }
  });
  if(this.orderList.length>0){
    var data={
      "OrderIds":this.orderList
    }
    document.getElementById('preloader-div').style.display = 'block';
    this.service.UpdateStatusOrderList(data).subscribe(data=>{
      this.orderList=[];
      this.getAllPendingorders();
    },error=>{
      console.log(error);
    });
  }
}

onSelectOfAllOrders(event){
  if(!!this.pageEvent){
    this.pageLength=this.pageEvent.pageSize;
  }
  else{
    this.pageLength=50;
  }
  this.splicedData = this.rows.slice(((0 + 1) - 1) * this.pageLength).slice(0, this.pageLength);
  if(!!event.checked){
    for(var i=0;i<this.splicedData.length;i++){
      this.rows[i].isSelected=true;
    }
    this.IsCompleteToButtonClicked=true;
  }else{
    for(var i=0;i<this.splicedData.length;i++){
      this.rows[i].isSelected=false;
    }
    this.IsCompleteToButtonClicked=false;
  }

}


}