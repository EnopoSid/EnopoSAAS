import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { LogoutService } from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AppInfoService } from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, EnquiryModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';
import { CloseScrollStrategy } from '@angular/cdk/overlay';
import { POSOrderService } from './pos-order.service';
import * as XLSX from  'xlsx';
import * as $ from 'jquery';
declare  var  jsPDF : any;
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { DatePipe, DecimalPipe } from '@angular/common';

@Component({
    selector: 'pos-order-table',
    templateUrl: './pos-order.component.html',
    encapsulation: ViewEncapsulation.None
})

export class POSOrderComponent implements OnInit {
    displayedColumns = ['sno','OrderId','TransactionAmount','PaymentTypeName','OrderDate','HavingHere','CustomerName','Email','MobileNumber','StoreName','IsCouponApply']
    dataSource: MatTableDataSource<EnquiryModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    private currentComponentWidth: number;
    rows = [];
    columns = [];
    temp = [];
    permisssionList:any;
    orderForm: FormGroup;
    dialogRef: MatDialogRef<any>;
    usersFromServiceExists ;
    sample : string;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};

    constructor(private spinner: NgxSpinnerService,
                private router: Router,
                private route:ActivatedRoute,
                public service: POSOrderService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                private formBuilder:FormBuilder,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService,  
                public datepipe:DatePipe,
                private _decimalPipe: DecimalPipe,
                public exportService :ExportService,
                private actRoute: ActivatedRoute,
                private activatedRoute: ActivatedRoute,) {
    }



    ngOnInit() {
        this.rows = [];
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'OrderId',"Value":" "},
              {"Key":'TransactionAmount',"Value":" "},
              {"Key":'PaymentTypeName',"Value":" "},
              {"Key":'OrderDate',"Value":" "},
              {"Key":'HavingHere',"Value":" "},
              {"Key":'CustomerName',"Value":" "},
              {"Key":'Email',"Value":" "},
              {"Key":'MobileNumber',"Value":" "},
              {"Key":'StoreName',"Value":" "},
              {"Key":'IsCouponApply',"Value":" "},
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
          this.orderForm = this.formBuilder.group({
            'orderstatusId':  [null]
        });
        this.permissiontypeList();
        this.getAllPOSorders();
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    }

    handlePageChange (event: any): void {
    }

    openResume(filePath) {
        window.open(filePath);
    }

    getAllPOSorders() {
        document.getElementById('preloader-div').style.display = 'block';
            this.service.getAllPOSorders().subscribe((response) => {
                this.temp=response;
                const orderData: any = [];   
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1;
                    response[i].editMode = false;
                    if(!!response[i].HavingHere){
                        response[i].HavingHere="Having Here"
                    }else if(!!response[i].TakeAway){
                        response[i].HavingHere="Take Away"
                    }
                    else
                    {
                        response[i].HavingHere="Take Away with own Bowl"
                    }
                    if(!!response[i].IsCouponApply){
                        response[i].IsCouponApply="Applied"
                    }else{
                        response[i].IsCouponApply="NA"
                    }
                    response[i].TransactionAmount=this._decimalPipe.transform(response[i].TransactionAmount,"1.2-2");
                    response[i].OrderDate = this.datepipe.transform(response[i].OrderDate,'yyyy/MM/dd HH:mm:ss'); 
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
    updatePagination(){
       this.filterData.dataSource=this.filterData.dataSource;
       this.filterData.dataSource.paginator = this.paginator;
      }
      
    permissiontypeList(){
  
        this.service.getOrderListItems().subscribe(resp => {
         this. permisssionList  = resp;
         console.log(this.permisssionList)
        
        })
        }
    actionAfterError () {
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this.router.navigate(['/sessions/signin']);
        });
    }
    editorderPermission(row){
        this.rows.forEach(element => {
          if(element.OrderId == row.OrderId)
          element.editMode = true;
          else
          element.editMode = false;
        });
    }

      
  saveOrderStatus(row){
      if(row.OrderStatus < this.orderForm.controls['orderstatusId'].value){
      let requestData ={
'OrderId': row.OrderId,
'OrderStatus': this.orderForm.controls['orderstatusId'].value
      };
      this.service.updateCancleStatus(requestData).subscribe(resp=> {
        this.getAllPOSorders();
      }, error=>{

      });
    }
    else{
        this.sendReceiveService.showDialog("please select next stage") 
    }
  }
  exportToPdf() {
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
        var col = ['sno','OrderId','Payment Type','order Date','Dining option','Customer Name','EmailID','Mobile Number','Store  Name','IsCouponApply'];
        for(var key in this.temp){
            var temporary = [this.temp[key].sno, this.temp[key].OrderId,this.temp[key].PaymentTypeName,this.temp[key].OrderDate,this.temp[key].HavingHere,this.temp[key].CustomerName, this.temp[key].Email,this.temp[key].MobileNumber, this.temp[key].StoreName,this.temp[key].IsCouponApply];
            rows.push(temporary);
        }
        let reportname = "PosOrders.pdf"
        this.exportService.exportAsPdf(col,rows,reportname);
    }
    else {
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
}
exportToExcel() {
   
    if(this.rows.length!=0){
         var rowsdata = [];
       
        for(var key in this.rows){
            var temporary = [(parseInt(key) +1), this.rows[key].OrderId,this.rows[key].TransactionAmount,this.rows[key].PaymentTypeName,this.rows[key].OrderDate,this.rows[key].HavingHere,this.rows[key].CustomerName, this.rows[key].Email,this.rows[key].MobileNumber, this.rows[key].StoreName,this.rows[key].IsCouponApply];
            rowsdata.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader =['sno','OrderId','Transaction Amount (GST included)','Payment Type','order Date','Dining option','Customer Name','EmailID','Mobile Number','Store  Name','IsCouponApply'];
        createXLSLFormatObj.push(xlsHeader);
        $.each(rowsdata, function(index, value) {
               var innerRowData = [];
              $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.OrderId+  "</td><td>" + value.TransactionAmount+  "</td><td>" + value.PaymentTypeName +  "</td><td>" + value.OrderDate +  "</td><td>" + value.HavingHere +   "</td><td>" +value.CustomerName +  "</td><td>" + value.Email + "</td><td>" + value.MobileNumber +   "</td><td>" + value.StoreName +   "</td><td>" + value.IsCouponApply +  "</td></tr>");

           
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

}