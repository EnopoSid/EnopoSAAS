
import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialogRef,MatDialog, MatDialogConfig, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../../services/common/appInfo.service';
import {SendReceiveService} from '../../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../../services/common/myAppHttp.service';
import { IPageLevelPermissions, SubMenuModel } from '../../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';
import { filter } from 'rxjs/operators';
import * as $ from 'jquery';
import { ReportService } from '../report.service';

@Component({
    selector: 'app-orderDetails',
    templateUrl: './orderDetails.component.html',
    styleUrls: ['./orderDetails.component.css']
  })
export class OrderDetailsComponent implements OnInit {
    id: any;
    addSubMenuFlag: boolean;
    displayedColumns = ['Quantity','ProductName','AttributeInfo','SubTotal']
    displayedColumnsFresh = ['MealPlanName','MealPlanPrice']
    displayedColumnsFreshIngradients = ['MealNumber','MealDate','MealTime']
    dataSource: MatTableDataSource<SubMenuModel>;
    dataSourceFresh: MatTableDataSource<SubMenuModel>;
    dataSourceFreshIngradients: MatTableDataSource<SubMenuModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    filterDataFresh;
    filterDataFreshIngradients;
    formErrors: any;
    title: string;
    rows = [];
    columns = [];
    temp = [];
    tempOrderData=[];
    dialogRef: MatDialogRef<any>;
    SubMenuForm :FormGroup;
    ID: number = 0;
    userId: number = 0;
    idOnUpdate:number=0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    menuname: string;
    currentPageLimit: number = 0;
    data:any;
    limit = MyAppHttp.LIMIT;
    pageLimitOptions = MyAppHttp.PAGE_LIMIT_OPTIONS
    IsMember: boolean;
    tempdata: any;
    OrderSubTotal : number = 0.00 ;
    ParentCategoryId: number;
    tempOrderDetails: any;
    FreshFlag: boolean=false;
    GrocerGourmentFlag:boolean=false;
    mealDate: any;
    TotalPrice: any;
    OrderSummaryData: any;
    gridDataForOrderDetails:any=[];
    gridDataForFreshOrderDetails:any=[];
    gridDataForFreshOrderDetailsInGradients:any=[];
    dataForFresh: any;
    tempOrderDetailsFresh: any;
    dataobj:any;
    reportSearchBy = { FromDate: null,ToDate: null,Category:null,orderstatus: null, orderserach : false};
    sub: any;
    OrderType: string;
    CatogeryList: { CategoryId: number; CategoryName: string; }[];
    CatogeryDefault: number;
    CategoryName: string;
    orderTaxTotal: any;
    OrderTotal: any;
    OrderShipping: any;
    IsFromPOS: any;

    constructor(
        private router: Router,
        private spinner: NgxSpinnerService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public service: ReportService,
        public translate: TranslateService,
         private route:ActivatedRoute,
        private formBuilder:FormBuilder
        ) 
        {
            this.formErrors = {};
        } 

ngOnInit(){
    this.id = +this.route.snapshot.params['id'];
    this.sub = this.route
    .queryParams
    .subscribe(params => {
      this.OrderType = params.OrderType 
    });
    this.userId=this.sendReceiveService.globalUserId;
    this.filterData={
        filterColumnNames:[
          {"Key":'Quantity',"Value":" "},
          {"Key":'ProductName',"Value":" "},
          {"Key":'AttributeInfo',"Value":" "},
          {"Key":'SubTotal',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
      this.filterDataFresh={
        filterColumnNames:[
          {"Key":'MealPlanName',"Value":" "},
          {"Key":'MealPlanPrice',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSourceFresh: this.dataSourceFresh,
        paginator:  this.paginator,
        sort:  this.sort
      };
      this.filterDataFreshIngradients={
        filterColumnNames:[
          {"Key":'MealNumber',"Value":" "},
          {"Key":'MealDate',"Value":" "},
          {"Key":'MealTime',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSourceFreshIngradients: this.dataSourceFreshIngradients,
        paginator:  this.paginator,
        sort:  this.sort
      };
      
 
     this.sendReceiveService.currentMessage.subscribe(data => this.data = data);
     if(this.OrderType=="NotFresh")
     {
        document.getElementById('preloader-div').style.display = 'block';
        this.getOrderDetails();
     }
     

     this.sendReceiveService.currentMessageForFresh.subscribe(dataForFresh => this.dataForFresh = dataForFresh);

     if(this.OrderType=="Fresh")
     {
        this.getFreshOrderDetails();
     }

     this.sendReceiveService.serachOrderedData.subscribe(dataobj =>this.dataobj = dataobj);
 
  
}
 



getOrderDetails()
{
    this.dataForFresh=null;
    let obj = { OrderId: this.data.OrderId,CustomerGUID:this.data.CustomerGuid};
    if(this.data.OrderId!=undefined)
    {
        this.service.getNopOrderDetails(obj).subscribe((success) => {
            this.GrocerGourmentFlag=true;
            this.FreshFlag=false;
            //Commented for excel tax 7% by sravan 17/7/2020 start
            //this.orderTaxTotal=success.Tax.split('$')[1];
             //Commented for excel tax 7% by sravan 17/7/2020 end 
             //Added By surakshith to show 7% Gst inclusive start on 03-08-2020 
             this.orderTaxTotal=success.Tax.split('$')[1];
             //Added By surakshith to show 7% Gst inclusive end on 03-08-2020 
            this.OrderTotal=success.OrderTotal.split('$')[1];
            this.OrderShipping=success.OrderShipping.split('$')[1];
            this.service.getMemberOrderDetails(obj.OrderId).subscribe((response)=>{
            this.IsMember = response[0].IsMember;
            this.tempOrderDetails = success;
            this.tempOrderDetails.MembershipSavings = response[0].MembershipSavings;
            this.tempOrderDetails.DiscountedAmount = response[0].OrderDiscount;
            this.tempOrderDetails.RedeemAmount = response[0].RedemptionValue;
            this.IsFromPOS=response[0].MemberId.includes("POS");
            
            this.tempOrderDetails.OrderType = "";
            var OrderTypeId = success.Items[0].ParentCategoryId;
            if (!!this.tempOrderDetails.OrderSubTotalDiscount) {
                this.tempOrderDetails.OrderSubTotalDiscount = parseFloat(this.tempOrderDetails.OrderSubTotalDiscount.split('$')[1]);
            }
            for (var i = 0; i < success.Items.length; i++) {
                OrderTypeId = success.Items[i].ParentCategoryId;
                this.CatogeryList=MyAppHttp.PARENTCATEGORIESDATA;
                 this.CatogeryDefault=this.CatogeryList[0].CategoryId;
                 this.CategoryName=this.CatogeryList[0].CategoryName;
                if (OrderTypeId ==  this.CatogeryDefault) {
                   this.tempOrderDetails.OrderType = this.CategoryName;
                    if (!!this.IsMember) {
                        this.tempOrderDetails.Items[i].SubTotal = this.tempOrderDetails.Items[i].SubTotal;
                    }
                }
                if (!!this.IsMember) {
                    this.OrderSubTotal = this.OrderSubTotal + parseFloat(this.tempOrderDetails.Items[i].SubTotal.split('$')[1]);
                } else if (!this.IsMember) {
                    this.OrderSubTotal = this.OrderSubTotal + parseFloat(this.tempOrderDetails.Items[i].SubTotal.split('$')[1]);
                }
                this.gridDataForOrderDetails.push(this.tempOrderDetails.Items[i]);
                this.ParentCategoryId = OrderTypeId;
                this.filterData.gridData = this.gridDataForOrderDetails;
                this.dataSource = new MatTableDataSource(this.gridDataForOrderDetails);
                this.filterData.dataSource=this.dataSource;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                document.getElementById('preloader-div').style.display = 'none';
            }
        }, (error) => {
            document.getElementById('preloader-div').style.display = 'none';
            this.sendReceiveService.showDialog("Something went wrong");
      });
    }, (error) => {
        document.getElementById('preloader-div').style.display = 'none';
        this.sendReceiveService.showDialog("Something went wrong");
  });
    }
    this.dataForFresh=null;
   
}

getFreshOrderDetails()
{
    document.getElementById('preloader-div').style.display = 'block';
    this.data=null;
    let objFresh = { OrderId: this.dataForFresh.OrderId,CustomerGUID:this.dataForFresh.CustomerGuid};
    if(this.dataForFresh.OrderId!=undefined)
    {
        this.service.getFreshOrderDetails(objFresh).subscribe((success)=>{
            this.FreshFlag=true;
            this.GrocerGourmentFlag=false;
            this.service.getMemberOrderDetails(objFresh.OrderId).subscribe((response)=>{
                this.IsMember = response[0].IsMember;
                this.tempOrderDetailsFresh = success;
                this.tempOrderDetailsFresh.OrderType = "";
                this.tempOrderDetailsFresh.MembershipSavings = 0;
                this.tempOrderDetailsFresh.DiscountedAmount = 0;
                this.tempOrderDetailsFresh.RedeemAmount = 0;
                var OrderTypeId = success.orderMealPlanModels[0].Items[0].ParentCategoryId;
                for (var i = 0; i < success.orderMealPlanModels.length; i++) {
                    var tempPerMealPlanData = this.tempOrderDetailsFresh.orderMealPlanModels[i];
                    this.tempOrderDetailsFresh.orderMealPlanModels[i].MealPlanPrice =  (tempPerMealPlanData.Items[i].UnitPrice.split('$')[1])*(tempPerMealPlanData.Items.length);
                    if (!!this.IsMember) {
                        this.tempOrderDetailsFresh.orderMealPlanModels[i].MealPlanPrice = parseInt(this.tempOrderDetailsFresh.orderMealPlanModels[i].MealPlanPrice) + ((this.tempOrderDetailsFresh.orderMealPlanModels[i].MealPlanPrice) * (this.tempdata.Fresh))/100;
                    }
                    for (var j = 0; j < success.orderMealPlanModels[i].Items.length; j++) {
                        var text = tempPerMealPlanData.Items[j].AttributeInfo.split("<br />").join(" ");
                        this.mealDate = tempPerMealPlanData.Items[j].MealDate.replace("T00:00:00", "");
                        this.TotalPrice = tempPerMealPlanData.Items[j].UnitPrice;
                        tempPerMealPlanData.Items[j].AttributeInfo = text;
                        tempPerMealPlanData.Items[j].MealDate = this.mealDate;
                        OrderTypeId = tempPerMealPlanData.Items[j].ParentCategoryId;
                        if (OrderTypeId == 69) {
                          this.tempOrderDetailsFresh.OrderType = "Fresh";  
                            if (!!this.IsMember) {
                                tempPerMealPlanData.Items[j].SubTotal = parseInt(tempPerMealPlanData.Items[j].SubTotal.split('$')[1]) + ((tempPerMealPlanData.Items[j].SubTotal.split('$')[1])*(this.tempdata.Fresh))/100;
                            }
                        }
                        
                        this.tempOrderDetailsFresh.orderMealPlanModels[i] = tempPerMealPlanData;
                        var tempmealInnerIngradientData= this.tempOrderDetailsFresh.orderMealPlanModels[i].Items
                    }  
                }

                
                this.gridDataForFreshOrderDetailsInGradients.push(tempmealInnerIngradientData);
                this.filterDataFreshIngradients.gridData = this.gridDataForFreshOrderDetailsInGradients;
                this.dataSourceFreshIngradients = new MatTableDataSource(this.gridDataForFreshOrderDetailsInGradients);
                this.filterDataFreshIngradients.dataSourceFreshIngradients=this.dataSourceFreshIngradients;
                this.dataSourceFreshIngradients.paginator = this.paginator;
                this.dataSourceFreshIngradients.sort = this.sort;


                this.gridDataForFreshOrderDetails.push(tempPerMealPlanData);
                this.filterDataFresh.gridData = this.gridDataForFreshOrderDetails;
                this.dataSourceFresh = new MatTableDataSource(this.gridDataForFreshOrderDetails);
                this.filterDataFresh.dataSourceFresh=this.dataSourceFresh;
                this.dataSourceFresh.paginator = this.paginator;
                this.dataSourceFresh.sort = this.sort;
                document.getElementById('preloader-div').style.display = 'none';
                this.tempOrderDetailsFresh.MembershipSavings = response[0].MembershipSavings;
                this.tempOrderDetailsFresh.DiscountedAmount = response[0].OrderDiscount;
                this.tempOrderDetailsFresh.RedeemAmount = response[0].RedemptionValue;
                if (!!this.IsMember) {
                    this.tempOrderDetailsFresh.Items[i].SubTotal =  (this.tempOrderDetailsFresh.Items[i].SubTotal.split('$')[1]) * (100/(100-(this.tempdata.Fresh)));
                }
            });
        });
    }
    this.data=null;
    document.getElementById('preloader-div').style.display = 'none';
}

  
 

onGoBack(){
        this.dataobj.orderserach=true;
        if(this.dataobj.Category=="Gourmet")
        {
            this.dataobj.Category=1
        }
        else if(this.dataobj.Category=="Grocery")
        {
            this.dataobj.Category=2
        }
        else if(this.dataobj.Category=="Fresh")
        {
            this.dataobj.Category=3
        }
        if(this.dataobj.orderstatus=="All")
        {
            this.dataobj.orderstatus=0;
        }
       else if(this.dataobj.orderstatus=="Pending")
        {
            this.dataobj.orderstatus=10;
        }
        else if(this.dataobj.orderstatus=="Processing")
        {
            this.dataobj.orderstatus=20;
        }
        else if(this.dataobj.orderstatus=="Completed")
        {
            this.dataobj.orderstatus=30;
        }
        else if(this.dataobj.orderstatus=="Cancelled")
        {
            this.dataobj.orderstatus=40;
        }
        let obj=this.dataobj;
        this.sendReceiveService.SendGlobalParamsForReportOrder(obj);
        this.router.navigate(["/orderreport"]);
        this.data=null;
        this.dataForFresh=null;
        this.filterData.dataSource=null;
        this.filterDataFresh.dataSource=null;
}


  
 
onCancel(){
    this.idOnUpdate=0;
    this.addSubMenuFlag = false;
    this.SubMenuForm.reset();
}
}