import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort, MatDatepicker} from '@angular/material';
import {LogoutService} from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl, FormGroupDirective } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, ComplaintReportModel } from '../../helpers/common.interface';
import { ReportService } from './report.service';
import {DatePipe} from "@angular/common";
import {ExportService } from '../../services/common/exportToExcel.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import * as XLSX from 'xlsx';

import * as $ from 'jquery';
import { ThrowStmt } from '@angular/compiler';
declare var jsPDF: any;  

@Component({
    selector:'app-report',
    templateUrl: './report.component.html',
    styleUrls: ['./report.component.css']
  })
export class ReportComponent implements OnInit {
    displayedColumns = ['sno','OrderId','productSKU','productName','amount','customerName','customerEmail','store','OrderDate']
    displayedFreshColumns = ['sno','OrderId','DeliveryMode','DeliveryDate','MealTime','DeliveryAddress','ProductCatogeryModifiedName','productSKU','ProductNameModifiedName','customerName','customerEmail','contactNumber','store','OrderDate']
    dataSource: MatTableDataSource<ComplaintReportModel>;
    dataSourceFresh: MatTableDataSource<ComplaintReportModel>;
    gridData =[];  
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    @ViewChild(FormGroupDirective, {static: false}) myForm;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    formErrors: any;
    title: string;
    SearchForm :FormGroup;  
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    isAdvanceSearchValid: boolean = true;
    allComplaintStatuses=[];
    CatogeryList =[];
    OrderStatusList=[];
    serviceProviderList=[];
    serviceCategoryList=[];
    complaintTypeList=[];
    communicationTypes=[];
    userId: number = 0;
    myDate;
    zoneList=[];
    regionId;
    freshFlag:boolean=false;
    isRegionSelected:boolean=false;    
    minDate;
    maxDate;
    IsFromGourmetandGrocer:boolean;
    IsFromGrocery:boolean;
    IsFromGourmet:boolean;
    IsFromFresh:boolean;
    filterDataFresh;
    flag:boolean;
    FromGrocery:boolean=false;
    defaultValue = 'Completed';
    FromDate : string
    ToDate:String
    
    OrderStatusListFiltered: any[];
    dataobj:any;
    dataForSearchReport:any;
    reportSearchBy = { FromDate: null,ToDate: null,Category:null,orderstatus: null, orderserach : false};
    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public ref: ChangeDetectorRef,
        public service: ReportService,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public exportService:ExportService,
        public translate: TranslateService,
        public datepipe:DatePipe,
        private route:ActivatedRoute,
        private formBuilder:FormBuilder) {  
            this.formErrors = {
                region: {}
            };
}
    
ngOnInit(){
    this.maxDate  = new Date();

    var report =localStorage.getItem("reportSearch");
    var reportSearch=JSON.parse(report);
    console.log(reportSearch);
    
    this.filterData={
        filterColumnNames:[
           { "Key":'sno',"Value":" "},
          {"Key":'OrderId',"Value":" "},
          {"Key":'productSKU',"Value":" "},
          {"Key":'productName',"Value":" "},
          {"Key":'amount',"Value":" "},
          {"Key":'customerName',"Value":" "},
          {"Key":'customerEmail',"Value":" "},
          {"Key":'store',"Value":" "},
          {"Key":'OrderDate',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
      this.filterDataFresh={
        filterColumnNames:[
           { "Key":'sno',"Value":" "},
          {"Key":'OrderId',"Value":" "},
          {"Key":'DeliveryMode',"Value":" "},
          {"Key":'DeliveryDate',"Value":" "},
          {"Key":'MealTime',"Value":" "},
          {"Key":'DeliveryAddress',"Value":" "},
          {"Key":'ProductCatogeryModifiedName',"Value":" "},
          {"Key":'productSKU',"Value":" "},
          {"Key":'ProductNameModifiedName',"Value":" "},
          {"Key":'customerName',"Value":" "},
          {"Key":'customerEmail',"Value":" "},
          {"Key":'contactNumber',"Value":" "},
          {"Key":'store',"Value":" "},
          {"Key":'OrderDate',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSourceFresh: this.dataSourceFresh,
        paginator:  this.paginator,
        sort:  this.sort
      };

    this.userId=this.sendReceiveService.globalUserId;
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    this.SearchForm = this.formBuilder.group({
        'fromDate':[null,Validators.required],
        'toDate':[null,Validators.required],
        'Category':  [null,Validators.required],
        'orderstatus':[null],
    });

    this.SearchForm.get('toDate').disable();
    this.CatogeryList=[
             "Gourmet"
    ]
    this.OrderStatusList=[
        "All",
        "Pending",
        "Processing",
        "Completed",
        "Cancelled"
   ]
   this.OrderStatusListFiltered=[
        "All",
        "Processing",
        "Completed",
        "Cancelled"
   ]
    this.getAllComplaintStatus();
          this.sendReceiveService.serachOrderedData.subscribe(dataForSearchReport =>this.dataForSearchReport = dataForSearchReport);
          this.getAllSearchedOrders();
}
myFilter = (d: Date): boolean => {
    const day = d.getDay();
    return day !== 0 && day !== 6;
  }
actionAfterError() {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this.router.navigate(['/sessions/signin']);
    });
}

getAllComplaintStatus(){
    
}  

onChangeCatogeryRegion(Category)
{
    this.filterData.dataSource=null;
    this.filterDataFresh.dataSource=null;
    this.rows=[];
    this.temp=[]; 
    if(Category=="Grocery")
    {
        this.FromGrocery=false;
    }
    else if(Category=="Gourmet" || Category=="Fresh")
    {
        this.FromGrocery=true;;
    }
}

getAllSearchedOrders()
{
    this.isAdvanceSearchValid = true;
        if((!!this.dataForSearchReport) && (!!this.dataForSearchReport.FromDate))
        {
                this.reportSearchBy = this.dataForSearchReport;
        }
    if(this.reportSearchBy.orderserach==false)
    {
        let fdate =  this.SearchForm.value.fromDate;
        let transformfdate=this.datepipe.transform(fdate,'MM/dd/yyyy');   
        this.reportSearchBy.FromDate=transformfdate;
        let tdate=this.SearchForm.value.toDate;
        let transtormtdate=this.datepipe.transform(tdate,'MM/dd/yyyy');
        this.reportSearchBy.ToDate = transtormtdate;
        let Category = this.SearchForm.value.Category;
        if(Category=="Gourmet")
        {
            this.reportSearchBy.Category=1;
            this.IsFromGourmet=true;
            this.IsFromGrocery=false;
            this.IsFromFresh=false;
        }
        else if(Category=="Grocery")
        {
            this.reportSearchBy.Category=2;
            this.IsFromGrocery=true;
            this.IsFromGourmet=false;
            this.IsFromFresh=false;
        }
        else if(Category=="Fresh")
        {
            this.reportSearchBy.Category=3;
            this.IsFromFresh=true;
            this.IsFromGrocery=false;
            this.IsFromGourmet=false;
        }
        else if(Category=="Promotions")
        {
            this.reportSearchBy.Category=4;
            this.IsFromGourmet=true;
            this.IsFromGrocery=false;
            this.IsFromFresh=false;
        }
        else
        {
            this.reportSearchBy.Category=null;
        }
        let orderstatus=this.SearchForm.value.orderstatus;
        if(orderstatus=="All")
        {
            this.reportSearchBy.orderstatus=0;
        }
       else if(orderstatus=="Pending")
        {
            this.reportSearchBy.orderstatus=10;
        }
        else if(orderstatus=="Processing")
        {
            this.reportSearchBy.orderstatus=20;
        }
        else if(orderstatus=="Completed")
        {
            this.reportSearchBy.orderstatus=30;
        }
        else if(orderstatus=="Cancelled")
        {
            this.reportSearchBy.orderstatus=40;
        }
        else{
            this.reportSearchBy.orderstatus=30;
        }
        this.reportSearchBy.orderserach=false;
        this.sendReceiveService.SendGlobalParamsForReportOrder(this.reportSearchBy);
        if(!!this.reportSearchBy.FromDate || !!this.reportSearchBy.ToDate || !!this.reportSearchBy.Category|| !!this.reportSearchBy.orderstatus)
        {
            if(this.reportSearchBy.Category==3)
            {
                document.getElementById('preloader-div').style.display = 'block';
                this.service.getFreshOrders(this.reportSearchBy).subscribe((response) => {
                    this.temp = response;
                    this.freshFlag = true;
                    this.IsFromFresh=true;
                    this.IsFromGourmetandGrocer=false;
                    const reportSearchData: any = [];
                    for (let i = 0; i < response.length; i++) {
                        response[i].sno = i + 1;
                        response[i].OrderDate =  this.datepipe.transform(response[i].OrderDate,'yyyy/MM/dd'); 
                        response[i].ProductCatogeryModifiedName=response[i].ProductCatogery.split('@')[0];
                        response[i].ProductNameModifiedName = response[i].ProductCatogery.split('@')[1];
                        if(response[i].productSKU==null)
                        {
                            response[i].productSKU="NA" 
                        }
                        reportSearchData.push(response[i]);
                    }
                    this.filterDataFresh.gridData = reportSearchData;
                    this.dataSourceFresh = new MatTableDataSource(reportSearchData);
                    this.filterDataFresh.dataSource=this.dataSourceFresh;
                    this.dataSourceFresh.paginator = this.paginator;
                    this.dataSourceFresh.sort = this.sort;
                    this.dataForSearchReport=null;
                    document.getElementById('preloader-div').style.display = 'none';
                }, (error) => {
                document.getElementById('preloader-div').style.display = 'none';
                this.sendReceiveService.showDialog("Something went wrong");
          });
            }
            else
            {
                    if(!!this.reportSearchBy.Category){
                         document.getElementById('preloader-div').style.display = 'block'; 
                        this.service.OrdersSearch(this.reportSearchBy).subscribe((response) => {
                            this.temp = response;
                            this.freshFlag = false;
                            this.IsFromGourmetandGrocer=true;
                            this.IsFromFresh=false;
                            const reportSearchData: any = [];
                            for (let i = 0; i < response.length; i++) {
                                response[i].sno = i + 1; 
                                response[i].OrderDate =  this.datepipe.transform(response[i].OrderDate,'yyyy/MM/dd'); 
                                if(response[i].productSKU==null)
                                {
                                    response[i].productSKU="NA" 
                                }
                                reportSearchData.push(response[i]);
                            }
                            this.filterData.gridData = reportSearchData;
                            this.dataSource = new MatTableDataSource(reportSearchData);
                            this.filterData.dataSource=this.dataSource;
                            this.dataSource.paginator = this.paginator;
                            this.dataSource.sort = this.sort;
                            this.dataForSearchReport=null;
                           document.getElementById('preloader-div').style.display = 'none';
                        },(error) => {
                            document.getElementById('preloader-div').style.display = 'none';
                            this.sendReceiveService.showDialog("Something went wrong");
                      });
                    }
                   
            }
        }
        else
        {
            this.isAdvanceSearchValid= false;
            document.getElementById('preloader-div').style.display = 'none';
        }
    }
    else if(this.reportSearchBy.orderserach==true)
    {
        this.reportSearchBy.orderserach=false;
        this.sendReceiveService.SendGlobalParamsForReportOrder(this.reportSearchBy);
        if(!!this.reportSearchBy.FromDate || !!this.reportSearchBy.ToDate || !!this.reportSearchBy.Category|| !!this.reportSearchBy.orderstatus)
        {
            if(this.reportSearchBy.Category==3)
            {
                document.getElementById('preloader-div').style.display = 'block';
                this.service.getFreshOrders(this.reportSearchBy).subscribe((response) => {
                    this.temp = response;
                    this.freshFlag = true;
                    this.IsFromFresh=true;
                    this.IsFromGourmetandGrocer=false;
                    const reportSearchData: any = [];
                    for (let i = 0; i < response.length; i++) {
                        response[i].sno = i + 1;
                        response[i].OrderDate =  this.datepipe.transform(response[i].OrderDate,'yyyy/MM/dd'); 
                        response[i].ProductCatogeryModifiedName=response[i].ProductCatogery.split('@')[0];
                        response[i].ProductNameModifiedName = response[i].ProductCatogery.split('@')[1];
                        if(response[i].productSKU==null)
                        {
                            response[i].productSKU="NA" 
                        }
                        reportSearchData.push(response[i]);
                    }console.log(reportSearchData);
                    this.filterDataFresh.gridData = reportSearchData;
                    this.dataSourceFresh = new MatTableDataSource(reportSearchData);
                    this.filterDataFresh.dataSource=this.dataSourceFresh;
                    this.dataSourceFresh.paginator = this.paginator;
                    this.dataSourceFresh.sort = this.sort;
                    document.getElementById('preloader-div').style.display = 'none';
                   
                },(error) => {
                    document.getElementById('preloader-div').style.display = 'none';
                    this.sendReceiveService.showDialog("Something went wrong");
              });
            }
            else
            {
                document.getElementById('preloader-div').style.display = 'block';
            this.service.OrdersSearch(this.reportSearchBy).subscribe((response) => {
                this.temp = response;
                this.freshFlag = false;
                this.IsFromGourmetandGrocer=true;
                this.IsFromFresh=false;
                const reportSearchData: any = [];
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1; 
                    response[i].OrderDate =  this.datepipe.transform(response[i].OrderDate,'yyyy/MM/dd'); 
                    if(response[i].productSKU==null)
                    {
                        response[i].productSKU="NA" 
                    }
                    reportSearchData.push(response[i]);
                }
                this.filterData.gridData = reportSearchData;
                this.dataSource = new MatTableDataSource(reportSearchData);
                this.filterData.dataSource=this.dataSource;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                document.getElementById('preloader-div').style.display = 'none';
              
               
            },(error) => {
                document.getElementById('preloader-div').style.display = 'none';
                this.sendReceiveService.showDialog("Something went wrong");
          });
            }
          
            if(this.reportSearchBy.Category==1)
            {
                this.reportSearchBy.Category="Gourmet";
            }
            else if(this.reportSearchBy.Category==2)
            {
                this.reportSearchBy.Category="Grocery";
            }
            else if(this.reportSearchBy.Category==3)
            {
                this.reportSearchBy.Category="Fresh";
            }
            else if(this.reportSearchBy.Category==4)
            {
                this.reportSearchBy.Category="Promotions";
            }
            if(this.reportSearchBy.orderstatus==0)
            {
                this.reportSearchBy.orderstatus="All"
            }
            else if(this.reportSearchBy.orderstatus==10)
            {
                this.reportSearchBy.orderstatus="Pending"
            }
            else if(this.reportSearchBy.orderstatus==20)
            {
                this.reportSearchBy.orderstatus="Processing"
            }
            else if(this.reportSearchBy.orderstatus==30)
            {
                this.reportSearchBy.orderstatus="Completed"
            }
            else 
            {
                this.reportSearchBy.orderstatus="Cancelled"
            }
            this.SearchForm.get('toDate').enable();
            this.SearchForm.patchValue({
                fromDate: new Date(this.reportSearchBy.FromDate),
                toDate:  new Date(this.reportSearchBy.ToDate),
                Category : this.reportSearchBy.Category,
                orderstatus :this.reportSearchBy.orderstatus
            });
           
        }
        else
        {
            document.getElementById('preloader-div').style.display = 'none';
            this.isAdvanceSearchValid= false;
        }
    }
    
    
    
}  




updatePagination(){
    this.filterData.dataSource=this.filterData.dataSource;
    this.filterData.dataSource.paginator = this.paginator;
    }
    
orderdetailsMethod(item){
    
    console.log(item);
    this.sendReceiveService.SendGlobalParams(item);
    this.router.navigate(['orderreport/orderdetails/'+item.OrderId], { queryParams: { OrderType: "NotFresh" } });
}
orderdetailsFreshMethod(itemForFresh)
{
    
    console.log(itemForFresh);
    this.sendReceiveService.SendGlobalParamsForFresh(itemForFresh);
    this.router.navigate(['orderreport/orderdetails/'+itemForFresh.OrderId], { queryParams: { OrderType: "Fresh" }});
}


exportToExcel(event) {
    let fdate =  this.SearchForm.value.fromDate;
    let transformfdate=this.datepipe.transform(fdate,'MM/dd/yyyy');
    let tdate=this.SearchForm.value.toDate;
    let transtormtdate=this.datepipe.transform(tdate,'MM/dd/yyyy');
    if(this.temp.length!=0 && this.IsFromGourmetandGrocer==true){
      var rows = [];
        
        for(var key in this.temp){
            let transformOrderdate= this.datepipe.transform(this.temp[key].OrderDate,'MM/dd/yyyy');
            let transformCustomerName=this.temp[key].customerName.toUpperCase();
            var temporary = [parseInt(key)+1, this.temp[key].OrderId,this.temp[key].productSKU,this.temp[key].productName,"SG$"+this.temp[key].amount,transformCustomerName,this.temp[key].customerEmail,this.temp[key].store,transformOrderdate];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader = ['S.NO.','Order Id','Product SKU','Product Name','Order Amount(GST included)','Customer Name','Customer Email','store','Order Date'];
        createXLSLFormatObj.push(xlsHeader);
        var reportname;
        if(this.IsFromGourmet)
        {
         reportname = "GourmetOrderReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        else if(!!this.IsFromGrocery==true)
        {
            reportname = "GrocerOrderReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        else if(!!this.IsFromFresh==true)
        {
            reportname = "FreshOrderReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.OrderId +  "</td><td>" + value.productSKU +  "</td><td>" + value.productName +  "</td><td>" + value.customerName +   "</td><td>" +value.customerEmail +   "</td><td>" +value.Store +   "</td><td>" +value.OrderDate  + "</td></tr>");
           
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'OrderReport'   : ws }, SheetNames: ['OrderReport'] };
        XLSX.writeFile(workbook, reportname , { bookType: 'xlsx', type: 'buffer' });
    }
     



    

    else if(this.temp.length!=0 && this.IsFromFresh==true)
    {
        var rows = [];
        
        
        for(var key in this.temp){
            let transformOrderdate= this.datepipe.transform(this.temp[key].OrderDate,'MM/dd/yyyy');
            let transformDeliverydate=this.datepipe.transform(this.temp[key].DeliveryDate,'MM/dd/yyyy');
            let transformCustomerName=this.temp[key].customerName.toUpperCase();
            var temporary = [parseInt(key)+1, this.temp[key].OrderId,this.temp[key].DeliveryMode,transformDeliverydate,this.temp[key].MealTime,this.temp[key].DeliveryAddress,this.temp[key].ProductCatogeryModifiedName,this.temp[key].productSKU,this.temp[key].ProductNameModifiedName,transformCustomerName,this.temp[key].customerEmail,this.temp[key].contactNumber,this.temp[key].store,transformOrderdate];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader =  ['S.NO.','Order Id','Delivery Mode','Delivery Date','Meal Time','Delivery Address','Product Catogery','Product SKU','Product Name','Customer Name','CustomerEmail','ContactNumber','Store','Order Date'];
        createXLSLFormatObj.push(xlsHeader);
        var reportname;
        if(!!this.IsFromGourmet==true)
        {
         reportname = "GourmetOrderReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        else if(!!this.IsFromGrocery==true)
        {
            reportname = "GrocerOrderReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        else if(!!this.IsFromFresh==true)
        {
            reportname = "FreshOrderReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
      $.each(rows, function(index, value) {
        var innerRowData = [];
       $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.OrderId +  "</td><td>" + value.DeliveryMode +  "</td><td>" + value.DeliveryDate +  "</td><td>" + value.MealTime +   "</td><td>" +value.DeliveryAddress +   "</td><td>" +value.ProductCatogeryModifiedName +   "</td><td>" +value.productSKU  + "</td><td>" +value.ProductNameModifiedName  + "</td><td>" +value.customerName  + "</td><td>" +value.customerEmail  + "</td><td>" +value.contactNumber  + "</td><td>" +value.store  + "</td><td>" +value.OrderDate  + "</td></tr>");

    
        $.each(value, function(ind, val) {

            innerRowData.push(val);
        });
        createXLSLFormatObj.push(innerRowData);
    });
    var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
    const workbook: XLSX.WorkBook = { Sheets: { 'OrderReport'   : ws }, SheetNames: ['OrderReport'] };
    XLSX.writeFile(workbook, reportname , { bookType: 'xlsx', type: 'buffer' });
    }
    else{
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
  }
  
tableFormatofData = "";
excelRowCount = 0;
borderClass='border: 1px solid black;border-collapse: collapse;'

drawTable(data) {
    if(this.IsFromGourmetandGrocer==true)
    {
        this.excelRowCount = 1;
    for (var i = 0; i < data.length; i++) {
        if (i == 0) {
            this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
            this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
            this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>Order Id</th><th>Product SKU</th><th>Product Name</th><th>Order Amount</th><th>Customer Name</th><th>Customer Email</th><th>Store</th><th>Order Date</th></tr></thead><tbody>";
        }
        this.drawRow(data[i]);
        if (i == data.length - 1) {
            this.tableFormatofData = this.tableFormatofData + "</tbody></table>";
            this.tableFormatofData = this.tableFormatofData + "</body></html>";
        }
    }
  }
  else if(this.IsFromFresh=true)
  {
    this.excelRowCount = 1;
    for (var i = 0; i < data.length; i++) {
        if (i == 0) {
            this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
            this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
            this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>Order Id</th><th>Delivery Mode</th><th>Delivery Date</th><th>Meal Time</th><th>Delivery Address</th><th>Product Catogery</th><th>Product SKU</th><th>Product Name</th><th>Customer Name</th><th>Customer Email</th><th>Contact Number</th><th>Store</th><th>Order Date</th></tr></thead><tbody>";
        }
        this.drawRow(data[i]);
        if (i == data.length - 1) {
            this.tableFormatofData = this.tableFormatofData + "</tbody></table>";
            this.tableFormatofData = this.tableFormatofData + "</body></html>";
        }
    }
  }
}
drawRow(rowData) {
    console.log(rowData);
    if(this.IsFromGourmetandGrocer==true)
    {
    let transformOrderdate= this.datepipe.transform(rowData.OrderDate,'MM/dd/yyyy');
    let transformCustomerName=rowData.customerName.toUpperCase();
    this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>" + rowData.OrderId + "</td><td>"  + rowData.productSKU + "</td><td>" + rowData.productName + "</td><td>" + "SG$" + rowData.amount + "</td><td>" + transformCustomerName + "</td><td>"+rowData.customerEmail+"</td><td>" +rowData.store+"</td><td>"+transformOrderdate+"</td></tr>";
    this.excelRowCount++;
   }
   else if(this.IsFromFresh==true)
   {
    let transformOrderdate= this.datepipe.transform(rowData.OrderDate,'MM/dd/yyyy');
    let transformDeliverydate=this.datepipe.transform(rowData.DeliveryDate,'MM/dd/yyyy');
    let transformCustomerName=rowData.customerName.toUpperCase();
    this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>" + rowData.OrderId + "</td><td>" + rowData.DeliveryMode + "</td><td>" + transformDeliverydate + "</td><td>" + rowData.MealTime + "</td><td>"+rowData.DeliveryAddress+"</td><td>"+rowData.ProductCatogeryModifiedName+"</td><td>"+  rowData.productSKU + "</td><td>" +  rowData.ProductNameModifiedName + "</td><td>" +  transformCustomerName + "</td><td>"+  rowData.customerEmail +"</td><td>"+  rowData.contactNumber + "</td><td>"+ rowData.store + "</td><td>" +transformOrderdate + "</td></tr>" ;
    this.excelRowCount++;  
   }
   
}
exportToPdf() {
    let fdate =  this.SearchForm.value.fromDate;
    let transformfdate=this.datepipe.transform(fdate,'MM/dd/yyyy');
    let tdate=this.SearchForm.value.toDate;
    let transtormtdate=this.datepipe.transform(tdate,'MM/dd/yyyy');
    if(this.temp.length!=0 && this.IsFromGourmetandGrocer==true){
        var doc = new jsPDF();
        var rows = [];
        var col = ['S.NO.','Order Id','Product SKU','Product Name','Order Amount (GST included)','Customer Name','Customer Email','store','Order Date'];
        
        for(var key in this.temp){
            let transformOrderdate= this.datepipe.transform(this.temp[key].OrderDate,'MM/dd/yyyy');
            let transformCustomerName=this.temp[key].customerName.toUpperCase();
            var temporary = [parseInt(key)+1, this.temp[key].OrderId,this.temp[key].productSKU,this.temp[key].productName,"SG$"+this.temp[key].amount,transformCustomerName,this.temp[key].customerEmail,this.temp[key].store,transformOrderdate];
            rows.push(temporary);
        }
        var reportname;
        if(!!this.IsFromGourmet==true)
        {
         reportname = "GourmetOrderReport("+ transformfdate+ "-" +transtormtdate +").pdf"
        }
        else if(!!this.IsFromGrocery==true)
        {
            reportname = "GrocerOrderReport("+ transformfdate+ "-" +transtormtdate +").pdf"
        }
        this.exportService.exportAsPdf(col,rows,reportname);
    }
    else if(this.temp.length!=0 && this.IsFromFresh==true)
    {
        var doc = new jsPDF();
        var rows = [];
        var col = ['S.NO.','Order Id','Delivery Mode','Delivery Date','Meal Time','Delivery Address','Product Catogery','Product SKU','Product Name','Customer Name','CustomerEmail','ContactNumber','Store','Order Date'];
        
        for(var key in this.temp){
            let transformOrderdate= this.datepipe.transform(this.temp[key].OrderDate,'MM/dd/yyyy');
            let transformDeliverydate=this.datepipe.transform(this.temp[key].DeliveryDate,'MM/dd/yyyy');
            let transformCustomerName=this.temp[key].customerName.toUpperCase();
            var temporary = [parseInt(key)+1, this.temp[key].OrderId,this.temp[key].DeliveryMode,transformDeliverydate,this.temp[key].MealTime,this.temp[key].DeliveryAddress,this.temp[key].ProductCatogeryModifiedName,this.temp[key].productSKU,this.temp[key].ProductNameModifiedName,transformCustomerName,this.temp[key].customerEmail,this.temp[key].contactNumber,this.temp[key].store,transformOrderdate];
            rows.push(temporary);
        }
        var reportname;
           
           reportname = "FreshOrderReport("+ transformfdate+ "-" +transtormtdate +").pdf"
        
        this.exportService.exportAsPdf(col,rows,reportname);
    }
    else {
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
}

onReset(){  
    if(this.isRegionSelected){
        this.SearchForm.removeControl('zone');
        this.filterData.dataSource=null;
        this.filterDataFresh.dataSource=null;
        this.rows=[];
        this.temp=[]; 
        this.dataForSearchReport=null;
        this.myForm.resetForm();
        this.SearchForm.reset();   
    }else{
        this.filterData.dataSource=null;
        this.filterDataFresh.dataSource=null;
        this.rows=[];
        this.temp=[]; 
        this.dataForSearchReport=null;
        this.myForm.resetForm();
        this.SearchForm.reset();
    }
}

clkFromDate(picker: MatDatepicker<Date>){
   picker.open();
}

clkToDate(picker: MatDatepicker<Date>){
    picker.open();
 }
 onChangeFromDate(selectedDate){
   if( this.SearchForm.value.fromDate == "" ){
    this.SearchForm.get('toDate').disable();
   }
   this.SearchForm.get('toDate').enable();
    
      this.minDate=selectedDate;
    this.filterData.dataSource=null;
    this.filterDataFresh.dataSource=null;
    this.rows=[];
    this.temp=[]; 
    this.dataForSearchReport=null;
}
onChangeToDate(selectedDate){
    this.maxDate=selectedDate;
    this.filterData.dataSource=null;
    this.filterDataFresh.dataSource=null;
    this.rows=[];
    this.temp=[]; 
    this.dataForSearchReport=null;
}
onChangeOrderStatus()
{
    this.filterData.dataSource=null;
    this.filterDataFresh.dataSource=null;
    this.rows=[];
    this.temp=[]; 
    this.dataForSearchReport=null;
}
onChangeOrderStatusSelect()
{
    this.filterData.dataSource=null;
    this.filterDataFresh.dataSource=null;
    this.rows=[];
    this.temp=[]; 
    this.dataForSearchReport=null;
}



}

