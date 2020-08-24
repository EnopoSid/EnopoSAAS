import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { LogoutService } from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AppInfoService } from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { orderService } from './order.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, EnquiryModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';
import { CloseScrollStrategy } from '@angular/cdk/overlay';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { DatePipe } from '@angular/common';
import *  as $ from 'jquery'  ;
import * as XLSX from 'xlsx';
declare var jsPDF :any; 


@Component({
    selector: 'order-table',
    templateUrl: './order.component.html',
    encapsulation: ViewEncapsulation.None
})

export class GetOrderComponent implements OnInit {
    displayedColumns = ['sno','OrderId','PickUpInStore','CreatedDate','deliverydate','CustomerName','EmailID','phoneNumber','StoreName','IsCouponApply']
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
                public service: orderService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public exportService: ExportService,
                private formBuilder:FormBuilder,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService,  
                private actRoute: ActivatedRoute,
                public datepipe:DatePipe,
                private activatedRoute: ActivatedRoute,) {
    }



    ngOnInit() {
        this.rows = [];
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'OrderId',"Value":" "},
              {"Key":'PickUpInStore',"Value":" "},
              {"Key":'CreatedDate',"Value":" "},
              {"Key":'deliverydate',"Value":" "},
              {"Key":'CustomerName',"Value":" "},
              {"Key":'EmailID',"Value":" "},
              {"Key":'phoneNumber',"Value":" "},
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
        this.getAllOrders();
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

    getAllOrders() {
        document.getElementById('preloader-div').style.display = 'block';
            this.service.getAllorders().subscribe((response) => {
                this.temp=response;
                const orderData: any = [];              
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1;
                    response[i].editMode = false;
                    if(!!response[i].PickUpInStore){
                        response[i].PickUpInStore="PickUp"
                    }else{
                        response[i].PickUpInStore="Delivery"
                    }
                    if(!!response[i].IsCouponApply){
                        response[i].IsCouponApply="Applied"
                    }else{
                        response[i].IsCouponApply="NA"
                    }

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
        this.getAllOrders();
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
        var col = ['sno','OrderId','PickUpInStore','Created Date','delivery Date','Customer Name','EmailID','phone Number','Store  Name','IsCouponApply'];
        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].OrderId,this.temp[key].PickUpInStore,(this.datepipe.transform(this.temp[key].CreatedDate,'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].deliverydate, 'yyyy/MM/dd')),this.temp[key].CustomerName, this.temp[key].EmailID,this.temp[key].phoneNumber, this.temp[key].StoreName,this.temp[key].IsCouponApply];
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
            var temporary = [(parseInt(key) +1), this.temp[key].OrderId,this.temp[key].PickUpInStore,(this.datepipe.transform(this.temp[key].CreatedDate,'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].deliverydate, 'yyyy/MM/dd')),this.temp[key].CustomerName, this.temp[key].EmailID,this.temp[key].phoneNumber, this.temp[key].StoreName,this.temp[key].IsCouponApply];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader = ['sno','OrderId','PickUpInStore','Created Date','delivery Date','Customer Name','EmailID','phone Number','Store  Name','IsCouponApply']; 
        createXLSLFormatObj.push(xlsHeader);
        $.each(rows, function(index, value) {
               var innerRowData = [];
              $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.FullName +  "</td><td>" + value.Email +  "</td><td>" + value.MobileNumber +   "</td><td>" +value.CreatedDate+ "</td><td>" +value.EmailID+ "</td><td>" +value.phoneNumber+ "</td><td>" +value.StoreName+"</td><td>" +value.IsCouponApply+ "</td></tr>");

           
               $.each(value, function(ind, val) {
       
                   innerRowData.push(val);
               });
               createXLSLFormatObj.push(innerRowData);
           });
           var filename = "onlineOrders.xlsx";
           var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
           const workbook: XLSX.WorkBook = { Sheets: { 'onlineOrders': ws }, SheetNames: ['onlineOrders'] };
           XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
       }


    
    else{
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
  }
  tableFormatofData = "";
excelRowCount = 0;
borderClass='border: 1px solid black;border-collapse: collapse;'

  drawRow(rowData) {
    this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>"+rowData.OrderId+"</td><td>"+rowData.PickUpInStore+"</td><td>"+(this.datepipe.transform(rowData.CreatedDate,'yyyy/MM/dd'))+"</td><td>"+(this.datepipe.transform(rowData.deliverydate, 'yyyy/MM/dd'))+"</td><td>"+rowData.CustomerName+"</td><td>"+ rowData.EmailID+"</td><td>"+rowData.phoneNumber+"</td><td>"+rowData.StoreName+"</td><td>"+rowData.IsCouponApply+"</td></tr>";
    this.excelRowCount++;
}
  drawTable(data) {  
    this.excelRowCount = 1;
    for (var i = 0; i < data.length; i++) {
        if (i == 0) {
            this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
            this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
            this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>OrderId </th><th>PickUpInStore</th><th>CreatedDate</th><th>deliverydate</th><th>CustomerName</th><th>EmailID</th><th>phoneNumber</th><th>StoreName</th><th>IsCouponApply</th></tr></thead><tbody>";
        }
        this.drawRow(data[i]);
        if (i == data.length - 1) {
            this.tableFormatofData = this.tableFormatofData + "</tbody></table>";
            this.tableFormatofData = this.tableFormatofData + "</body></html>";
        }
    }
  
}




}