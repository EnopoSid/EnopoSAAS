import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
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
import { deliveryordersService } from './deliveryorders.service';



@Component({
    selector: 'app-deliveryorders',
    templateUrl: './deliveryorders.component.html',
    styleUrls: ['./deliveryorders.component.css']
})
export class deliveryordersComponent implements OnInit {
    status: boolean;
    editdeliveryorderflag:boolean;
    displayedColumns = ['sno','OrderId','OrderAmount','OrderDate','DeliveryDate','MobileNumber','CustomerAddress','StoreName','StoreAssignedStatus','Actions']
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
    tempdata=[];
    dialogRef: MatDialogRef<any>;
    addPermissionFlag:boolean=false;
    deliveryOrdersForm:FormGroup;
    userId: number = 0;
    idOnUpdate: number =0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    currentPageLimit: number = 0;
    limit = MyAppHttp.LIMIT;
    pageLimitOptions = MyAppHttp.PAGE_LIMIT_OPTIONS;
    checked = true;
    storesList = []
    orderId: any;
    StoreAssignedStatus: any;
    interval;
    adminInterval;
    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: deliveryordersService,
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
              {"Key":'OrderId',"Value":" "},
              {"Key":'OrderAmount',"Value":" "},
              {"Key":'OrderDate',"Value":" "},
              {"Key":'DeliveryDate',"Value":" "},
              {"Key":'MobileNumber',"Value":" "},
              {"Key":'CustomerAddress',"Value":" "},
              {"Key":'StoreName',"Value":" "},
              {"Key":'StoreAssignedStatus',"Value":" "},
               ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
   this.getAllDeliveryOrders();

    this.deliveryOrdersForm = this.formBuilder.group({
        id: 0,
        'store': [null, Validators.compose([Validators.required])]
    });
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
}

getAllDeliveryOrders(){
    document.getElementById('preloader-div').style.display = 'block';

    this.service.getAllDeliveryOrders().subscribe((response) => { 
        this.temp=response;
        const storesData: any = [];
        for (let i = 0; i < response.length; i++) {
            response[i].sno = i + 1;
            if(response[i].StoreAssignedStatus==null)
            {
                response[i].StoreAssignedStatus="NA";
            }
            if(response[i].StoreName=="Null")
            {
                response[i].StoreName="NA";
            }
            response[i].OrderDate = this.datepipe.transform(response[i].OrderDate,'yyyy/MM/dd'); 
            response[i].DeliveryDate = this.datepipe.transform(response[i].DeliveryDate,'yyyy/MM/dd HH:mm:ss'); 
            storesData.push(response[i]);
        }
        this.filterData.gridData = storesData;
        this.dataSource = new MatTableDataSource(storesData);
        this.filterData.dataSource=this.dataSource;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        for (let i = 0; i < this.filterData.filterColumnNames.length; i++) {
            this.filterData.filterColumnNames[i].Value ='';
          }
          document.getElementById('preloader-div').style.display = 'none';
    }, (error) => {
        this.sendReceiveService.showDialog('Some thing went wrong please try again');
        document.getElementById('preloader-div').style.display = 'none';
    });
}

clearFilters(){
    this.dataSource.filter = '';
  }

updatePagination(){
   this.filterData.dataSource=this.filterData.dataSource;
   this.filterData.dataSource.paginator = this.paginator;
  }
  

addPermission(){
    this.addPermissionFlag=true;
    this.title="Save"
}
ngOnDestroy(): void {
    clearInterval(this.interval);
  }
  

getAllConfigurations(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllConfigurations().subscribe((response) => {
     if(response){
      var configuration  =  response.filter(function (e) {
        return e.Key == 'DelIVERYloadingtime'
      });
      this.adminInterval  = parseInt(configuration[0].Value) ;
     
     }else{
      this.adminInterval =20000;
     }
     this.getAllDeliveryOrders();
  
    }, (error) => {
        document.getElementById('preloader-div').style.display = 'none';
        });
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
            var temporary = [(parseInt(key) +1), this.temp[key].OrderId,this.temp[key].OrderAmount,this.temp[key].OrderDate,this.temp[key].DeliveryDate,this.temp[key].MobileNumber,this.temp[key].CustomerAddress,this.temp[key].StoreName,this.temp[key].StoreAssignedStatus];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','OrderId','OrderAmount(GST inlcuded)',"OrderDate","DeliveryDate","MobileNumber","CustomerAddress","AssignedStoreName","StoreAssignedStatus"];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>"
            + value.sno + "</td><td>" + value.OrderId +  "</td><td>" 
            + value.OrderDate+ "</td></td>"+ value.DeliveryDate+ "</td></td>"
            + value.MobileNumber+ "</td></td>"+ value.CustomerAddress+ "</td></td>"
            + value.StoreName+ "</td></td>"+ value.StoreAssignedStatus+ "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "DeliveryOrders.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'DeliveryOrders': ws }, SheetNames: ['DeliveryOrders'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }
  

editdeliveryorderassignedstatus(obj){
    this.orderId=obj.OrderId;
    this.StoreAssignedStatus=obj.StoreAssignedStatus;
    if(this.StoreAssignedStatus=="NA")
    {
        this.StoreAssignedStatus=true;
    }
    else{
        this.StoreAssignedStatus=this.StoreAssignedStatus;
    }
    this.editdeliveryorderflag = true;
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllActiveStores().subscribe((response) => { 
        this.temp=response;
        this.storesList = this.temp;
        document.getElementById('preloader-div').style.display = 'none';
    }, (error) => {
        document.getElementById('preloader-div').style.display = 'none';
    });

}
assignordertostore()
{
    let OrderId=this.orderId;
    let storeId=this.deliveryOrdersForm.value.store;
    let assignedstatus=this.StoreAssignedStatus;
    document.getElementById('preloader-div').style.display = 'block';
    this.service.assignordertostore(OrderId,storeId,assignedstatus).subscribe((response) => { 
        this.tempdata=response;
        this.sendReceiveService.showDialog('The delivery order assigned sucessfully');
        document.getElementById('preloader-div').style.display = 'none';
        this.editdeliveryorderflag = false;
        this.getAllDeliveryOrders();
    }, (error) => {
        document.getElementById('preloader-div').style.display = 'none';
        this.editdeliveryorderflag = false;
    });
}

onCancel() {
    this.idOnUpdate = 0;
    this.editdeliveryorderflag = false;
    this.deliveryOrdersForm.reset();
}

}
