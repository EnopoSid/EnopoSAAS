import {Component, OnInit, ViewChild, ChangeDetectorRef, Output, EventEmitter} from '@angular/core';
import {ComplaintsService} from'../complaints/complaints.service';
import {MatDialog, MatDialogConfig, MatDialogRef, MatDatepicker} from '@angular/material';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import {Router} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import Utils from '../../services/common/utils';
import { AmChartsService, AmChart  } from "@amcharts/amcharts3-angular";
import MyAppHttp from '../../services/common/myAppHttp.service';
import { DashboardService } from './dashboard.service';
import { TranslateService } from '@ngx-translate/core';
import { DatePipe, getLocaleMonthNames } from '@angular/common';
import { Subject } from 'rxjs';
import { IPageLevelPermissions } from 'src/app/helpers/common.interface';
@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css'],
})

export class DashboardComponent implements OnInit {
    minDate;
    maxDate;
    ProviderDurationChange: boolean=false;
    CategoryDurationChange: boolean=false;
    calldetailsDurationChange: boolean=false;
    SampleFlag: boolean=false;
    @ViewChild('myTable', {static: false}) table: any;
    @ViewChild('tableWrapper', {static: false}) tableWrapper;
    private chart: any;
    private pie: any;
    private chartdata = [];
    private statusData= [];
    private categoryData = [];
    private providerdata = [];
    private providerStatus = [];
    private categoryStatus = [];
    serviceProviderList=[];
    serviceCategoryList=[];
    userdetailsList = [];
    durationOptions = MyAppHttp.DURATION;
    chartnames = MyAppHttp.chartnames;
    statusduration: Number;
    categoryduration: Number;
    serviceduration: Number;
    StoreSalesOrdersForm:FormGroup;
    PickUpDeliveryForm:FormGroup;
    ServiceProviderChartForm:FormGroup;
    OrdersAndSalesForm:FormGroup;  
    monthId=new Date().getMonth();
    monthName;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    CatogeryList: any;
    MonthsList:any;
    yearsList:any;
    pickupserviceflag1:boolean=false;
    pickupserviceflag2:boolean=false;
    pickupserviceflag3:boolean=false;
    OrdersAndSalesflag1: boolean=false;
    OrdersAndSalesflag2: boolean=false;
    yearDefaultForOrderSales:number;
    yearDefaultForPickUp:number;
    monthDefaultForOrderSales:number;
    monthDefaultForPickUp:number;
    CatogeryDefault:number;
    flag: number;
    StoreOrdersSales=false;
    TotalOrdersSales=true;
    OnlineOrderSalesShipping=false;
    TotalOrdersforPie={};
    TotalOrdersForBar={};
    StoreOrdersAndSalesflag1: boolean=false;
    StoreOrdersAndSalesflag2: boolean=false;
    OnlineOrdersAndSalesflag1: boolean=false;
    OnlineOrdersAndSalesflag2: boolean=false;
    OrdersAndSalesSubmittedFlag=false;
    showFiller = false;
    StoreSalesOrdersFormSubmitedFlag=false;
    PickUpDeliveryFormSubitedFlag=false;


    constructor(public translate: TranslateService,
                private router: Router,
                public service: DashboardService,
                public dialog: MatDialog,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService,
                private AmCharts: AmChartsService,
                private formBuilder:FormBuilder,
                private datePipe: DatePipe,
                private detectorRef:ChangeDetectorRef
               ) {}


    ngOnInit() {   
        this.maxDate  = new Date();
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
            this.pagePermissions =pageLevelPermissions.response;  
            this.sendReceiveService.globalPageLevelPermission.unsubscribe();
        });
    
        var year = new Date().getFullYear();
        var date=new Date();
        var monthname=date.getMonth();
        
        var range = [];
      
        for (var i = 0; i <=20; i++) {
            range.push(year - i);
        }
        this.yearsList = range;
        this.yearDefaultForOrderSales=this.yearsList[0];
        this.yearDefaultForPickUp=this.yearsList[0];
       
        this.MonthsList=MyAppHttp.Months;
        this.monthDefaultForOrderSales=this.MonthsList[monthname].ID;
        this.monthDefaultForPickUp=this.MonthsList[monthname].ID;
        this.OrdersAndSalesForm=this.formBuilder.group({
            'fromDate':[null,Validators.required],
            'toDate':[null,Validators.required],
        });

        this.CatogeryList=MyAppHttp.PARENTCATEGORIESDATA;
        this.CatogeryDefault=this.CatogeryList[0].CategoryId;

        this.StoreSalesOrdersForm = this.formBuilder.group({
            'fromDateForStore':[null,Validators.required],
            'toDateForStore':[null,Validators.required],
        });
       
        this.PickUpDeliveryForm = this.formBuilder.group({
            'fromDateForOnline':[null,Validators.required],
            'toDateForOnline':[null,Validators.required],
            
        }); 
        this.flag = 1
        var date=new Date();
        let transformfdate=this.datePipe.transform(date,'yyyy/MM/dd'); 
        this.getTotalSalesOrdersDefault(this.flag,transformfdate,transformfdate);
    }
    private TotalOrders= {};
    

    getTotalSalesOrdersDefault(flag,fdate,tdate){
        this.service.getTotalSalesOrders(flag,fdate,tdate).subscribe((reponse)=>{
            this.TotalOrders = reponse;
        });  
    }

    getTotalSalesOrdersForPie(flag,fdate,tdate){
        this.service.getPosSalesOrders(flag,fdate,tdate).subscribe((reponse)=>{
            this.TotalOrdersforPie = reponse;
            this.getNumberOfOrdersPieChart(this.TotalOrdersforPie);
            this.getsalesPieChart(this.TotalOrdersforPie);
        });  
    }


   defaultOrderSalesFromSubmit()
   {
    var year = new Date().getFullYear();
    var date=new Date();
    var monthname=date.getMonth();
    
    var range = [];
    for (var i = 0; i <=20; i++) {
        range.push(year - i);
    }
    this.yearsList = range;
    let yearDefault=this.yearsList[0];
    
    this.MonthsList=MyAppHttp.Months;
    let monthDefault=this.MonthsList[monthname].ID;

    this.service.sendMonthandyear(monthDefault,yearDefault).subscribe(response=>{
        let tempData =response;
        this.service.getOrderscount(monthDefault,yearDefault).subscribe((reponse)=>{
            this.TotalOrders = reponse[0];
        });
       
        this.getNumberOfOrdersPieChart(tempData);
        this.getsalesPieChart(tempData);
   });
}

onChangeMonth(month)
{
    if(month!="")
    {
        this.OrdersAndSalesflag1=true;
        this.OrdersAndSalesFormSubmit();
    }
  
}
onChangeYear(year)
{
    if(year!="")
    {
        this.OrdersAndSalesflag2=true;
        this.OrdersAndSalesFormSubmit();
    }
   
}

OrdersAndSalesFormSubmit(){
    this.flag=1;
     let fDate=this.OrdersAndSalesForm.value.fromDate;
    let tDate=this.OrdersAndSalesForm.value.toDate;
    let transformfdate=this.datePipe.transform(fDate,'yyyy/MM/dd'); 
    let transtofdate=this.datePipe.transform(tDate,'yyyy/MM/dd'); 
    if(fDate=="" || tDate=="")
    {
        var date=new Date();
        let transformfdate=this.datePipe.transform(date,'yyyy/MM/dd'); 
        let transtofdate=this.datePipe.transform(date,'yyyy/MM/dd'); 
      
    }
    if(this.OrdersAndSalesflag1==true && this.OrdersAndSalesflag2==true )
    {
        this.flag=2;
        this.OrdersAndSalesSubmittedFlag=true;
        this.getTotalSalesOrdersDefault(this.flag,transformfdate,transtofdate);
}
}
    


onPickUpDeliveryFormSubmit()
{
    this.flag=1;
    let fDate=this.PickUpDeliveryForm.value.fromDateForOnline;
    let tDate=this.PickUpDeliveryForm.value.toDateForOnline;
    let transformfdate=this.datePipe.transform(fDate,'yyyy/MM/dd'); 
    let transtofdate=this.datePipe.transform(tDate,'yyyy/MM/dd'); 
    if(fDate=="" || tDate=="")
    {
        var date=new Date();
        let transformfdate=this.datePipe.transform(date,'yyyy/MM/dd'); 
        let transtofdate=this.datePipe.transform(date,'yyyy/MM/dd'); 
      
    }
   
    if(!!this.OnlineOrdersAndSalesflag2 && !!this.OnlineOrdersAndSalesflag1)
    {
        this.flag=2; 
        this.PickUpDeliveryFormSubitedFlag=true;
        this.getSalesOrdersByShippingType(this.flag,transformfdate,transtofdate);
    }
}
 
GenerateBar(data,chartName,title){
    for(var i=0;i<data.length;i++){
        data[i].Color= this.getRandomColor();
    }
     this.chart = this.AmCharts.makeChart(chartName, {
            "hideCredits":true,
            "type": "serial",
            "theme": "light",
            "dataProvider": data,
            "valueAxes": [{
                "title":title
            }],
            "categoryAxis": {
                "title": "Amount of Sales",
                "gridPosition": "start",
                "labelRotation": 50
              },
            
            "categoryField": "TotalSales",
            "startDuration": 1,
            "graphs": [
                {
                "valueField": "TotalOrders",
                "titleField": "TotalSales",
                "fillColorsField": "Color",
                "type": "column",
                "fillAlphas": 0.8,
                "angle": 30,
                "depth3D": 5,
                "balloonText": "[[ShippingType]]: <b>[[value]]</b>"
            }
        ],
            "export": {
                "enabled": false
            }
        });
    }
    


GeneratePie(data, chartName) {
    data=data.filter(x=>x.StoreName!="Total");
    this.AmCharts.makeChart(chartName, {
        "hideCredits":true,
        "type": "pie",
        "theme": "light",
        "labelRadius": -25,
        "labelText": "[[percents]]%",
        "dataProvider": data,
        "valueField": "TotalOrders",
        "titleField": "ShippingType",
        "balloon": {
            "fixedPosition": true
        },
        "export": {
            "enabled": false
        },
        "titles": [{
            "size": 18,
            "text":  "No. of Orders",
            }],
       
        "legend":{
        "position":"right",
         "marginRight":50,
         "autoMargins":true
       },
    })
  
}

GeneratePieChart(data, chartName) {
   data=data.filter(x=>x.StoreName!="Total");
    this.AmCharts.makeChart(chartName, {
        "hideCredits":true,
        "type": "pie",
        "theme": "light",
        "labelRadius": -25,
        "labelText": "[[percents]]%",
        "dataProvider": data,
        "valueField": "TotalSales",
        "titleField": "ShippingType",
        "outlineColor": "#FFFFFF",
        "balloon": {
            "fixedPosition": true
        },
        "export": {
            "enabled": false
        },
        "titles": [{
            "size": 18,
            "text":  "Sales",
            }],
        "legend":{
            "position":"right",
         "marginRight":50,
         "autoMargins":true
       },
    })
    
}

getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

getOnlineOrderSalesByShippingTypeCharts(data) {
    this.GenerateBar(data,"categorychart","No.Of Orders")
}
getNumberOfOrdersPieChart(data) {
    this.pie = this.GeneratePie(data, "statuschart");
}
getsalesPieChart(data) {
    this.pie = this.GeneratePieChart(data, "saleschart");
}

getCatogeryPiechartFromSubmit() {

    this.flag=1;
    let fDate=this.StoreSalesOrdersForm.value.fromDateForStore;
    let tDate=this.StoreSalesOrdersForm.value.toDateForStore;
    let transformfdate=this.datePipe.transform(fDate,'yyyy/MM/dd'); 
    let transtofdate=this.datePipe.transform(tDate,'yyyy/MM/dd'); 
    if(fDate=="" || tDate=="")
    {
        var date=new Date();
        let transformfdate=this.datePipe.transform(date,'yyyy/MM/dd'); 
        let transtofdate=this.datePipe.transform(date,'yyyy/MM/dd'); 
      
    }
   
    if(!!this.StoreOrdersAndSalesflag2 && !!this.StoreOrdersAndSalesflag1)
    {
        this.flag=2; 
        this.StoreSalesOrdersFormSubmitedFlag=true;
        this.getTotalSalesOrdersForPie(this.flag,transformfdate,transtofdate);
    }
}
   

    onChangeProviderduration(providerpiechart){
        if(providerpiechart> 0){
            this.ProviderDurationChange=true;
        this.serviceduration = providerpiechart;
        this.getgraphdata(this.serviceduration, this.chartnames[1].chartname );
        }
        else {
            this.ProviderDurationChange=false;
            this.sendReceiveService.showDialog("Select The Duration")
        }
    }
    onUserduration(providerpiechart){
        if(providerpiechart> 0){
            this.ProviderDurationChange=true;
        this.serviceduration = providerpiechart;
        this.getgraphdata(this.serviceduration, this.chartnames[1].chartname );
        }
        else {
            this.ProviderDurationChange=false;
            this.sendReceiveService.showDialog("Select The User")
        }
    }
 
    getgraphdata(selectedData, chartname){
        let FromDays, todays
        let selectedDuration = this.durationOptions.filter(x => x.id === selectedData)
        if (selectedDuration != null && selectedDuration.length != 0) {
            this.calldetailsDurationChange=true;
            let statusSearchBy = { FromDays: selectedDuration[0].fromvalue, ToDays: selectedDuration[0].tovalue, Category: 0, Service: 0, chartname: chartname, userId:0 };
            if(chartname == this.chartnames[5].chartname){
                let fromdate =  new Date();
                let tdate =  new Date();
                let frmdate = fromdate.setDate(fromdate.getDate() - selectedDuration[0].fromvalue);
                let fromdt = this.datePipe.transform(frmdate,"yyyy-MM-dd");
                let date =   tdate.setDate(tdate.getDate() - selectedDuration[0].tovalue);
                let todate = this.datePipe.transform(date,"yyyy-MM-dd");
            }
            else{
            }
        }
        else {
            this.calldetailsDurationChange=false;
            if (selectedDuration.length == 0) {
                if(chartname == this.chartnames[5].chartname){
                }
            }
            return false
        }
    }

    onproviderchange(selectedprovider){
        let FromDays, todays
        let selectedDuration = this.durationOptions.filter(x => x.id === this.serviceduration)
        if (selectedDuration != null) {
            if(selectedprovider> 0){
            let statusSearchBy = { FromDays: selectedDuration[0].fromvalue, ToDays: selectedDuration[0].tovalue, Category: 0, Service: selectedprovider, chartname: this.chartnames[3].chartname };
            }
            else{
                let serviceduration = {value : this.serviceduration}
                this.onChangeProviderduration(serviceduration.value);
            }
        }
        else {
            return false
        }
    }

    appenColourToBar(data){
        this,this.chartdata.forEach(data => { 
            let colour = MyAppHttp.StatusColorCode.filter(code => code.statusName == "Lunch");
            data.color = colour[0].colour;
            })
     return data;
    }

    
    getSalesOrdersByShippingType(flag,fdate,tdate){
        this.service.getOnlineSalesOrdersByShipping(flag,fdate,tdate).subscribe((reponse)=>{
            this.TotalOrdersForBar = reponse;
            this.getOnlineOrderSalesByShippingTypeCharts(this.TotalOrdersForBar);
        });  
    }
    onDashboardMenuClk(clkedMenuId){
        this.OrdersAndSalesSubmittedFlag=false;
        this.StoreSalesOrdersFormSubmitedFlag=false;
        this.PickUpDeliveryFormSubitedFlag=false;
        this.flag = 1
        var date=new Date();
        this.maxDate  = new Date();
        let transformfdate=this.datePipe.transform(date,'yyyy/MM/dd'); 
        if(clkedMenuId==1){
            this.OrdersAndSalesForm.reset();
           this.TotalOrdersSales=true;
           this.StoreOrdersSales=false;
           this.OnlineOrderSalesShipping=false;
           this.getTotalSalesOrdersDefault(this.flag,transformfdate,transformfdate);
        }
        else if(clkedMenuId==2){
        this.StoreSalesOrdersForm.reset();
           this.getTotalSalesOrdersForPie(this.flag,transformfdate,transformfdate);
            this.StoreOrdersSales=true;
            this.TotalOrdersSales=false;
            this.OnlineOrderSalesShipping=false;
        }
        else if(clkedMenuId==3){
            this.PickUpDeliveryForm.reset();
            this.getSalesOrdersByShippingType(this.flag,transformfdate,transformfdate);
            this.OnlineOrderSalesShipping=true;
            this.StoreOrdersSales=false;
            this.TotalOrdersSales=false;
        }
        this.detectorRef.detectChanges();
    }

    clkFromDate(picker: MatDatepicker<Date>){
        picker.open();
     }
     
    clkToDate(picker: MatDatepicker<Date>){
         picker.open();
      }
    
     onChangeFromDate(selectedDate,fromChart){
         if(fromChart==1){
            if(selectedDate!="")
            {
                this.OrdersAndSalesflag2=true;
                this.OrdersAndSalesFormSubmit();
            }
         }
         else if(fromChart==2){
            if(selectedDate!="")
            {
                this.StoreOrdersAndSalesflag2=true;
                this.getCatogeryPiechartFromSubmit();
            }
         }else if(fromChart==3){
            if(selectedDate!="")
            {
                this.OnlineOrdersAndSalesflag2=true;
                this.onPickUpDeliveryFormSubmit();
            }
         }
        
        this.minDate=selectedDate;
     }
     onChangeToDate(selectedDate,fromChart){
         if(fromChart==1){
            if(selectedDate!="")
            {
                this.OrdersAndSalesflag1=true;
                this.OrdersAndSalesFormSubmit();
            }
         }
        else if(fromChart==2){
            if(selectedDate!="")
            {
                this.StoreOrdersAndSalesflag1=true;
                this.getCatogeryPiechartFromSubmit();
            }
         }else if(fromChart==3){
            if(selectedDate!="")
            {
                this.OnlineOrdersAndSalesflag1=true;
                this.onPickUpDeliveryFormSubmit();
            }
         }
        
     }
}
