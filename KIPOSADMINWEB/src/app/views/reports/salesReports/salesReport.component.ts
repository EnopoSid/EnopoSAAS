import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort, MatDatepicker} from '@angular/material';
import {LogoutService} from '../../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../../services/common/appInfo.service';
import {SendReceiveService} from '../../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl, FormGroupDirective } from '@angular/forms';
import { IPageLevelPermissions, ComplaintReportModel } from '../../../helpers/common.interface';
import { SalesReportService } from './salesReport.service';
import {DatePipe} from "@angular/common";
import {ExportService } from '../../../services/common/exportToExcel.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import * as XLSX from 'xlsx';

import * as $ from 'jquery';
declare var jsPDF: any;  

@Component({
    selector:'',
    templateUrl: './salesReport.component.html',
    styleUrls: ['./salesReport.component.css']
  })
export class SalesReportComponent implements OnInit {
    displayedColumns = ['sno','OrderId','amount','customerName','customerEmail','contactNumber','OrderDate','DeliveryMode','store']
    displayedFreshColumns = ['sno','OrderId','DeliveryMode','amount','customerName','customerEmail','contactNumber','store','OrderDate']
    dataSource: MatTableDataSource<ComplaintReportModel>;
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
    FromGrocery:boolean=false;
    IsFromGrocery:boolean;
    IsFromGourmet:boolean;
    IsFromFresh:boolean;
    filterDataFresh;
    OrderStatusList=[];
    defaultValue = 'Completed';
    OrderStatusListFiltered: string[];
    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public ref: ChangeDetectorRef,
        public service: SalesReportService,
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
    this.filterData={
        filterColumnNames:[
           { "Key":'sno',"Value":" "},
          {"Key":'OrderId',"Value":" "},   
          {"Key":'amount',"Value":" "},
          {"Key":'customerName',"Value":" "},
          {"Key":'customerEmail',"Value":" "},
          {"Key":'contactNumber',"Value":" "},
          {"Key":'OrderDate',"Value":" "},
          {"Key":'DeliveryMode',"Value":" "},
          {"Key":'store',"Value":" "}
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
          {"Key":'amount',"Value":" "},
          {"Key":'customerName',"Value":" "},
          {"Key":'customerEmail',"Value":" "},
          {"Key":'contactNumber',"Value":" "},
          {"Key":'store',"Value":" "},
          {"Key":'OrderDate',"Value":" "}
        ], 
        gridData:  this.gridData,
        dataSource: this.dataSource,
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
             "Gourmet",
             "Grocery",
             "Fresh"
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

updatePagination(){
    this.filterData.dataSource=this.filterData.dataSource;
    this.filterData.dataSource.paginator = this.paginator;
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
    let reportSearchBy = { FromDate: null,ToDate: null,Category:null,orderstatus: null};
 
    
     let fdate =  this.SearchForm.value.fromDate;
    let transformfdate=this.datepipe.transform(fdate,'MM/dd/yyyy');
    reportSearchBy.FromDate=transformfdate;
    let tdate=this.SearchForm.value.toDate;
    let transtormtdate=this.datepipe.transform(tdate,'MM/dd/yyyy');
    reportSearchBy.ToDate = transtormtdate;
    let Category = this.SearchForm.value.Category;
    if(Category=="Gourmet")
    {
        reportSearchBy.Category=1;
        this.IsFromGourmet=true;
        this.IsFromGrocery=false;
        this.IsFromFresh=false;
    }
    else if(Category=="Grocery")
    {
        reportSearchBy.Category=2;
        this.IsFromGrocery=true;
        this.IsFromGourmet=false;
        this.IsFromFresh=false;
    }
    else if(Category=="Fresh")
    {
        reportSearchBy.Category=3;
        this.IsFromFresh=true;
        this.IsFromGrocery=false;
        this.IsFromGourmet=false;
    }
    else
    {
        reportSearchBy.Category=null;
    }
    let orderstatus=this.SearchForm.value.orderstatus;
    if(orderstatus=="All")
    {
        reportSearchBy.orderstatus=0;
    }
    else if(orderstatus=="Pending")
    {
        reportSearchBy.orderstatus=10;
    }
    else if(orderstatus=="Processing")
    {
        reportSearchBy.orderstatus=20;
    }
    else if(orderstatus=="Completed")
    {
        reportSearchBy.orderstatus=30;
    }
    else if(orderstatus=="Cancelled")
    {
        reportSearchBy.orderstatus=40;
    }
    else{
        reportSearchBy.orderstatus=30;
    }
    if(!!reportSearchBy.FromDate || !!reportSearchBy.ToDate || !!reportSearchBy.Category|| !!reportSearchBy.orderstatus)
    {
        if(reportSearchBy.Category==3)
        {
            this.service.getFreshOrderSales(reportSearchBy).subscribe((response) => {
                this.temp = response;
                this.freshFlag = true;
                this.IsFromFresh=true;
                this.IsFromGourmetandGrocer=false;
                const reportSearchData: any = [];
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1;
                    response[i].OrderDate =  this.datepipe.transform(response[i].OrderDate,'yyyy/MM/dd'); 
                    reportSearchData.push(response[i]);
                }
                this.filterDataFresh.gridData = reportSearchData;
                this.dataSource = new MatTableDataSource(reportSearchData);
                this.filterDataFresh.dataSource=this.dataSource;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
            });
        }
        else
        {
            this.service.getGourmentOrderSales(reportSearchBy).subscribe((response) => {
                this.temp = response;
                this.freshFlag = false;
                this.IsFromGourmetandGrocer=true;
                this.IsFromFresh=false;
                const reportSearchData: any = [];
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1; 
                    response[i].OrderDate =  this.datepipe.transform(response[i].OrderDate,'yyyy/MM/dd'); 
                    reportSearchData.push(response[i]);
                }
                this.filterData.gridData = reportSearchData;
                this.dataSource = new MatTableDataSource(reportSearchData);
                this.filterData.dataSource=this.dataSource;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
         });
       } 
    }
    else
    {
        this.isAdvanceSearchValid= false;
    }
}  

onChangeRegion(selectedRegionId){
    this.filterData.dataSource=null;
    this.filterDataFresh.dataSource=null;
    this.rows=[];
    this.temp=[];
    if(selectedRegionId > 0){
        this.isRegionSelected = true;
        let control: FormControl = new FormControl(null, Validators.required);
        this.SearchForm.addControl('zone', control);
        this.regionId=selectedRegionId;
        this.service.getZonesByRegionId(this.regionId).subscribe((zone)=>{
            this.zoneList=zone;
        })
    }else if(selectedRegionId==0||selectedRegionId==undefined|| selectedRegionId==null){
       this.isRegionSelected=false; 
       this.SearchForm.removeControl('zone');
    }
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
            let transformDeliverydate=this.datepipe.transform(this.temp[key].DeliveryDate,'MM/dd/yyyy');
            let transformCustomerName=this.temp[key].customerName.toUpperCase();
            var temporary = [parseInt(key)+1, this.temp[key].OrderId,"SG$"+this.temp[key].amount,transformCustomerName,this.temp[key].customerEmail,this.temp[key].contactNumber,transformOrderdate,this.temp[key].DeliveryMode,this.temp[key].store];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader = ['S.NO.','Order Id','Order Amount','Customer Name','Customer Email','Contact Number','Order Date','Delivery Mode','Store'];
        createXLSLFormatObj.push(xlsHeader);
      
        var reportname;

        if(!!this.IsFromGourmet==true)
        {
         reportname = "GourmetSalesReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        else if(!!this.IsFromGrocery==true)
        {
            reportname = "GrocerSalesReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        else if(!!this.IsFromFresh==true)
        {
            reportname = "FreshSalesReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.OrderId +  "</td><td>" + value.amount +  "</td><td>" + value.customerName +  "</td><td>" + value.customerEmail +   "</td><td>" +value.contactNumber +   "</td><td>" +value.OrderDate +   "</td><td>" +value.DeliveryMode  +  "</td><td>" +value.store  + "</td></tr>");
           
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'SalesReport'   : ws }, SheetNames: ['SalesReport'] };
        XLSX.writeFile(workbook, reportname , { bookType: 'xlsx', type: 'buffer' });
    }
     
    
    else if(this.temp.length!=0 && this.IsFromFresh==true)
    {
       var rows = [];
       
       
       for(var key in this.temp){
           let transformOrderdate= this.datepipe.transform(this.temp[key].OrderDate,'MM/dd/yyyy');
           let transformCustomerName=this.temp[key].customerName.toUpperCase();
           var temporary = [parseInt(key)+1, this.temp[key].OrderId,this.temp[key].DeliveryMode,"$SG"+this.temp[key].amount,transformCustomerName,this.temp[key].customerEmail,this.temp[key].contactNumber,this.temp[key].store,transformOrderdate];
           rows.push(temporary);
       }
       var createXLSLFormatObj = [];
       var xlsHeader = ['S.NO.','Order Id','DeliveryMode','Order Amount','Customer Name','Customer Email','Contact Number','Store','Order Date'];
       createXLSLFormatObj.push(xlsHeader);
     
      
        var reportname;
        if(!!this.IsFromGourmet==true)
        {
         reportname = "GourmetSalesReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        else if(!!this.IsFromGrocery==true)
        {
            reportname = "GrocerSalesReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        else if(!!this.IsFromFresh==true)
        {
            reportname = "FreshSalesReport("+ transformfdate+ "-" +transtormtdate +").xlsx"
        }
        $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.OrderId +  "</td><td>" + value.DeliveryMode +  "</td><td>" + value.amount +  "</td><td>" + value.customerName +   "</td><td>" +value.customerEmail +   "</td><td>" +value.contactNumber +   "</td><td>" +value.store  +  "</td><td>" +value.OrderDate  + "</td></tr>");
           
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'SalesReport'   : ws }, SheetNames: ['SalesReport'] };
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
    this.excelRowCount = 1;
    if(this.IsFromGourmetandGrocer==true)
    {
    for (var i = 0; i < data.length; i++) {
        if (i == 0) {
            this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
            this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
            this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>Order Id</th><th>Order Amount</th><th>Customer Name</th><th>Customer Email</th><th>Contact Number</th><th>Order Date</th><th>Delivery Mode</th><th>Store</th></tr></thead><tbody>";
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
    for (var i = 0; i < data.length; i++) {
        if (i == 0) {
            this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
            this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
            this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>Order Id</th><th>Delivery Mode</th><th>Order Amount</th><th>Customer Name</th><th>Customer Email</th><th>Contact Number</th><th>Store</th><th>Order Date</th></tr></thead><tbody>";
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
    let transformDeliverydate=this.datepipe.transform(rowData.DeliveryDate,'MM/dd/yyyy');
    let transformCustomerName=rowData.customerName.toUpperCase();
    this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>" + rowData.OrderId + "</td><td>" + "SG$" + rowData.amount + "</td><td>" + transformCustomerName + "</td><td>"+rowData.customerEmail + "</td><td>"+rowData.contactNumber+"</td><td>"+transformOrderdate+"</td><td>"+rowData.DeliveryMode+"</td><td>"+rowData.store+"</td></tr>";
   }
   else if(this.IsFromFresh==true)
   {
    let transformOrderdate= this.datepipe.transform(rowData.OrderDate,'MM/dd/yyyy');
    let transformCustomerName=rowData.customerName.toUpperCase();
    this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>" + rowData.OrderId + "</td><td>" +  rowData.DeliveryMode + "</td><td>"+ '$SG'+rowData.amount + "</td><td>" + transformCustomerName + "</td><td>" + rowData.customerEmail + "</td><td>"+rowData.contactNumber+"</td><td>"+rowData.store+"</td><td>" +  transformOrderdate + "</td></tr>";
   }
   this.excelRowCount++;
}
exportToPdf() {

    let fdate =  this.SearchForm.value.fromDate;
    let transformfdate=this.datepipe.transform(fdate,'MM/dd/yyyy');
    let tdate=this.SearchForm.value.toDate;
    let transtormtdate=this.datepipe.transform(tdate,'MM/dd/yyyy');
    if(this.temp.length!=0 && this.IsFromGourmetandGrocer==true){
        var doc = new jsPDF();
        var rows = [];
        var col = ['S.NO.','Order Id','Order Amount','Customer Name','Customer Email','Contact Number','Order Date','Delivery Mode','Store'];
        
        for(var key in this.temp){
            let transformOrderdate= this.datepipe.transform(this.temp[key].OrderDate,'MM/dd/yyyy');
            let transformDeliverydate=this.datepipe.transform(this.temp[key].DeliveryDate,'MM/dd/yyyy');
            let transformCustomerName=this.temp[key].customerName.toUpperCase();
            var temporary = [parseInt(key)+1, this.temp[key].OrderId,"SG$"+this.temp[key].amount,transformCustomerName,this.temp[key].customerEmail,this.temp[key].contactNumber,transformOrderdate,this.temp[key].DeliveryMode,this.temp[key].store];
            rows.push(temporary);
        }
        var reportname;
        if(!!this.IsFromGourmet==true)
        {
         reportname = "GourmetSalesReport("+ transformfdate+ "-" +transtormtdate +").pdf"
        }
        else if(!!this.IsFromGrocery==true)
        {
            reportname = "GrocerSalesReport("+ transformfdate+ "-" +transtormtdate +").pdf"
        }
        this.exportService.exportAsPdf(col,rows,reportname);
    }
    else if(this.temp.length!=0 && this.IsFromFresh==true)
    {
        var doc = new jsPDF();
        var rows = [];
        var col = ['S.NO.','Order Id','DeliveryMode','Order Amount','Customer Name','Customer Email','Contact Number','Store','Order Date'];
        
        for(var key in this.temp){
            let transformOrderdate= this.datepipe.transform(this.temp[key].OrderDate,'MM/dd/yyyy');
            let transformCustomerName=this.temp[key].customerName.toUpperCase();
            var temporary = [parseInt(key)+1, this.temp[key].OrderId,this.temp[key].DeliveryMode,"$SG"+this.temp[key].amount,transformCustomerName,this.temp[key].customerEmail,this.temp[key].contactNumber,this.temp[key].store,transformOrderdate];
            rows.push(temporary);
        }
        var reportname;
           
           reportname = "FreshSalesReport("+ transformfdate+ "-" +transtormtdate +").pdf"
        
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
        this.myForm.resetForm();
        this.SearchForm.reset();   
    }else{
        this.filterData.dataSource=null;
        this.filterDataFresh.dataSource=null;
        this.rows=[];
        this.temp=[]; 
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
}
onChangeToDate(selectedDate){
    this.maxDate=selectedDate;
    this.filterData.dataSource=null;
    this.filterDataFresh.dataSource=null;
    this.rows=[];
    this.temp=[]
}
onChangeOrderStatus()
{
    this.filterData.dataSource=null;
    this.filterDataFresh.dataSource=null;
    this.rows=[];
    this.temp=[]; 
}
onChangeOrderStatusSelect()
{
    this.filterData.dataSource=null;
    this.filterDataFresh.dataSource=null;
    this.rows=[];
    this.temp=[]; 
}
orderdetailsMethod(item){
    
    console.log(item);
    this.sendReceiveService.SendGlobalParams(item);
    this.router.navigate(['/orderdetails/'+item.OrderId], { queryParams: { OrderType: "NotFresh" } });
}
}

