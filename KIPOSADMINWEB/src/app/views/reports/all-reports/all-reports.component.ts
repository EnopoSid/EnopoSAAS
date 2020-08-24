import { Component, OnInit } from '@angular/core';
import { MatDatepicker, MatDialogRef } from '@angular/material';
import { FormGroup ,FormControl,Validators, FormBuilder } from '@angular/forms';
import { IPageLevelPermissions } from 'src/app/helpers/common.interface';
import { Subject } from 'rxjs';
import * as XLSX from 'xlsx';
import * as $ from 'jquery';
import { DatePipe } from '@angular/common';
import { AllReportsService } from './all-reports.service';
import { analyzeAndValidateNgModules } from '@angular/compiler';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
declare var jsPDF: any;  
@Component({
  selector: 'app-all-reports',
  templateUrl: './all-reports.component.html',
  styleUrls: ['./all-reports.component.css']
})
export class AllReportsComponent implements OnInit {
  
  SearchForm :FormGroup;  
  rows = [];
  columns = [];
  temp = [];
  filterData;
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
  storeList : any;
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

  constructor( 
    public datepipe:DatePipe,
   public service:AllReportsService,
    private formBuilder:FormBuilder,
    public sendReceiveService: SendReceiveService


  ) { }

  ngOnInit() {
    this.maxDate  = new Date();


 this.service.getStoreDetails().subscribe((stores) => {
   var storelist=[];
 this.storeList=stores;
  
})
 this.SearchForm = this.formBuilder.group({
  'fromDate':[null,Validators.required],
  'toDate':[null,Validators.required],
  'store':[null,Validators.required] ,
});

  }


  clkFromDate(picker: MatDatepicker<Date>){
    picker.open();
 }
 onChangeFromDate(selectedDate){

  if( this.SearchForm.value.fromDate == "" ){
 this.SearchForm.get('toDate').disable();
}
    
this.SearchForm.get('toDate').enable();
 this.minDate=selectedDate;
 this.rows=[];
 this.temp=[];
}


onChangeToDate(selectedDate){
  this.maxDate=selectedDate;

  this.rows=[];
  this.temp=[]
}
onChangeOrderStatusSelect()
{
    this.rows=[];
    this.temp=[]; 
}

getAllSearchedOrders()
{
    this.isAdvanceSearchValid = true;
    let reportSearchBy = { fDate: null,tDate: null,storeId: null};
 
    let fdate =  this.SearchForm.value.fromDate;
   let transformfdate=this.datepipe.transform(fdate,'MM/dd/yyyy');
   reportSearchBy.fDate=transformfdate;
   let tdate=this.SearchForm.value.toDate;
   let transtormtdate=this.datepipe.transform(tdate,'MM/dd/yyyy');
   reportSearchBy.tDate = transtormtdate;
  let orderstatus=this.SearchForm.value.store;
    reportSearchBy.storeId=orderstatus
    
  
   
        this.service.Salesreport(reportSearchBy ).subscribe((response) => {
            this.temp=response;
   let fdate =  this.SearchForm.value.fromDate;
   let transformfdate=this.datepipe.transform(fdate,'MM/dd/yyyy');
   let tdate=this.SearchForm.value.toDate;
   let transtormtdate=this.datepipe.transform(tdate,'MM/dd/yyyy');

    var rows = [];
  
      
      for(var key in this.temp){
      
          var temporary = [ (this.datepipe.transform(this.temp[key].Date, 'dd/MM/yyyy')),this.temp[key].GrossSale,this.temp[key].GstAmount,this.temp[key].Discounts,this.temp[key].Shipping,this.temp[key].NetSales,this.temp[key].kPoints,this.temp[key].Stripe,this.temp[key].CreditCard,this.temp[key].Cash,this.temp[key].Nets,this.temp[key].FavPay,this.temp[key].GrabPay,this.temp[key].PayNow,this.temp[key].BDXMAX,this.temp[key].Deliveroo,this.temp[key].FoodPanda,this.temp[key].GrabFood,this.temp[key].Waiter,this.temp[key].Waitrr,this.temp[key].MealPal,this.temp[key].CaterSpot,this.temp[key].DishDash,this.temp[key].Online,this.temp[key].Others,this.temp[key].FreeBill,this.temp[key].Nomnomby,this.temp[key].NoPayment,this.temp[key].Media,this.temp[key].POSOnline,this.temp[key].Total,this.temp[key].Difference];
          
          rows.push(temporary);
      }
      var createXLSLFormatObj = [];
      var xlsCompany=['KIPOS Sales Report']
      createXLSLFormatObj.push(xlsCompany);

   for(var index of this.storeList)
   {
     if(orderstatus==index.StoreId)
     {

     
          var store=['Outlet:',index.StoreName,'','','online pickUp orders + pos orders']
          break;
     }

   }
     
    
createXLSLFormatObj.push(store);

var store1=['DATE','From',transformfdate,'TO',transtormtdate]
createXLSLFormatObj.push(store1);
var header1=['','','Sales Summary','','','','','','','','','Sales Collected','','']
 createXLSLFormatObj.push(header1);
 
var xlsHeader = ['Date','GrossSale (SG$)','GstAmount (SG$)','Discounts (SG$)','Shipping (SG$)','NetSales (SG$)','k-Points (SG$)','Stripe (SG$)','CreditCard (SG$)','Cash (SG$)','Nets (SG$)','FavPay (SG$)','GrabPay (SG$)','PayNow (SG$)','BDXMAX (SG$)','Deliveroo (SG$)','FoodPanda (SG$)','GrabFood (SG$)','Waiter (SG$)','Waitrr (SG$)','MealPal (SG$)','CaterSpot (SG$)','DishDash (SG$)','(SG$) Online','(SG$) Others','(SG$) FreeBill','(SG$) Nomnomby','(SG$) NoPayment','(SG$) Media','(SG$) POSOnline','Total (SG$)','Difference (SG$)'];
      createXLSLFormatObj.push(xlsHeader);
  
      var reportname= "SalesReport("+ index.StoreName + "_FROM_"+ transformfdate+ "_TO_" +transtormtdate +").xlsx"
     
      $.each(rows, function(index, value) {
          var innerRowData = [];
         $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>"  + value.Date +  "</td><td>" + value.GrossSale +  "</td><td>" + value.GstAmount +  "</td><td>" + value.NetSales +   "</td><td>" +value.CreditCard +   "</td><td>" +value.Cash +   "</td><td>" +value.FavPay  +  "</td><td>" +value.BDXMAX  +   "</td><td>" +value.Deliveroo  +   "</td><td>" +value.FoodPanda  +   "</td><td>" +value.GrabFood  +   "</td><td>" +value.Waiter  +  "</td><td>" +value.MealPal  + "</td><td>" +value.CaterSpot  + "</td><td>" +value.DishDash  +  "</td><td>" +value.Total  +   "</td><td>" +value.Difference  + "</td></tr>");
         
      
          $.each(value, function(ind, val) {
  
              innerRowData.push(val);
          });
          createXLSLFormatObj.push(innerRowData);
      });
      var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);     
      const workbook: XLSX.WorkBook = { Sheets: { "SalesSummaryReport"  : ws }, SheetNames: ['SalesSummaryReport'] };

      XLSX.writeFile(workbook, reportname , { bookType: 'xlsx', type: 'buffer' });
  
 

});

}


}