import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { MatDatepicker, MatDialogRef, MatTableDataSource, MatPaginator, MatInput } from '@angular/material';
import { FormGroup ,FormControl,Validators, FormBuilder } from '@angular/forms';
import { IPageLevelPermissions } from 'src/app/helpers/common.interface';
import { Subject } from 'rxjs';
import * as XLSX from 'xlsx';
import * as $ from 'jquery';
import { DatePipe } from '@angular/common';
import { analyzeAndValidateNgModules } from '@angular/compiler';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { IngradientCountReportService } from './IngradientCountReport.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';
import { MenuObservableService } from 'src/app/services/common/MenuObservableService';
import { ExcelService } from 'src/app/services/common/ExcelService';
declare var jsPDF: any;  
@Component({
  selector: 'app-all-reports',
  templateUrl: './IngradientCountReport.component.html',
  styleUrls: ['./IngradientCountReport.component.css']
})
export class IngradientCountReportComponent implements OnInit {
  displayedColumns = ['sno','IngradientName','ProductAttributeValueCount']
  @ViewChild('FromDate', { static: false }) searchElement: ElementRef;
  SearchForm :FormGroup;  
  rows = [];
  columns = [];
  temp = [];
  filterData;
  dialogRef: MatDialogRef<any>;
  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
  pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
  isAdvanceSearchValid: boolean = true;
  allComplaintStatuses=[];
  CatogeryList =[];
  serviceProviderList=[];
  serviceCategoryList=[];
  complaintTypeList=[];
  gridData =[];
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
  dataSource: MatTableDataSource<unknown>;
  sort: any;
  StorePickupPontsList: any;
  reqResponse: any;
  allMenus;
  ActiveMenuID: any;
  allProducts: [];
  pageNumber: number;
  categoryName: any;
  MenuID: any;
  SubMenus: any;
  subMenuActiveId: any;
  fromDate: string;
  toDate: string;
  bowlsCount:any[]=[];
  public submitted: boolean = false;
  excelSalesData: any;
  noData: boolean=false;
  bowlsData: boolean = true;
  storeId: any;
  reportname: any;
  AllMainCate: number;
  tempstoreobj: { Id: number; Name: string; Description: string; AddressId: string; PickupFee: string; OpeningHours: string; DisplayOrder: string; StoreId: string; StorePickupPoint1: string; ContactAddress: string; ContactMobile: string; };
  responsedata: any;
  selectedcategorydata: any;
  catetegoryID: any;
  selectedsubcategorydata: any;
  subcategoryName: any;
  subcatetegoryID: any;
  subcategoryEnable:boolean=false;
  excelcategoryName: any;
  excelsubcategoryName: any;
  Id: any;


  constructor( 
    public datepipe:DatePipe,
   public service:IngradientCountReportService,
    private formBuilder:FormBuilder,
    public sendReceiveService: SendReceiveService,
    private menuObservableService: MenuObservableService,
    private excelService: ExcelService

  ) { }

  ngOnInit() {
     
        this.bowlsData=false;
        this.maxDate = new Date();
        this.minDate = new Date();
        var temp = new Date();
        temp.setDate(temp.getDate()-1);
        this.maxDate=temp;
        this.getAllIsActiveStorePickupPoints();
        this.filterData={
          filterColumnNames:[
            {"Key":'sno',"Value":" "},
            {"Key":'IngradientName',"Value":" "},
            {"Key":'ProductAttributeValueCount',"Value":" "},
          ],
          gridData:  this.gridData,
          dataSource: this.dataSource,
          paginator:  this.paginator,
        };
        this.getAllTopMenus();
            this.menuObservableService.menus.subscribe(() => {
              this.changeFilterSelection(this.allMenus[0].Id, this.allMenus[0].Name);
            });
        this.SearchForm = this.formBuilder.group({
          'fromDate':[null,Validators.required],
          'toDate':[null,Validators.required],
          'storedata':[null,Validators.required],
          'category':[null,Validators.required],
          'subCategory':[null]
        });
           this.tempstoreobj={
          'Id':0,
     'Name':'ALL Stores',
     'Description':'null',
    'AddressId':'null',
      'PickupFee' :'null',
    'OpeningHours' :'null',
    'DisplayOrder' :'null',
    'StoreId':'null',
     'StorePickupPoint1': 'null',
    'ContactAddress' :'null',
    'ContactMobile' :'null',
   }
  
  }


  clkFromDate(picker: MatDatepicker<Date>) {
    picker.open();
  }

  clkToDate(picker: MatDatepicker<Date>) {
    picker.open();
  }
  onChangeFromDate(selectedDate) {
    this.minDate = selectedDate;
  }
  onChangeToDate(selectedDate) {
    this.maxDate = selectedDate;
    var temp = new Date();
    temp.setDate(temp.getDate()-1);
    this.maxDate=temp;
  }

  resetFn()
  {
      this.submitted=false;
      this.SearchForm.reset();
  }

getAllTopMenus() {
  this.service.getTopMenu().subscribe(response => {
    document.getElementById('preloader-div').style.display = 'none';
    if (response.Categories.length!=0) {
             
       this.reqResponse = response.Categories;
      this.allMenus = this.reqResponse;

      if (this.reqResponse[0].SubCategories.length > 0) {
        localStorage.setItem('menuId', this.reqResponse[0].SubCategories[0].Id);
      }
      this.ActiveMenuID = this.allMenus[0].Id;
    }
  }, error => {
    document.getElementById('preloader-div').style.display = 'none';
    this.sendReceiveService.showDialog("Something went wrong");
  });
}

changeFilterSelection(Id, categoryName) {
  this.allProducts = [];
  this.pageNumber = 1;
  this.categoryName = categoryName;
  for (var sb = 0; sb < this.allMenus.length; sb++) {
    this.MenuID = this.allMenus[sb].Id;
    if (Id === this.MenuID) {
      this.ActiveMenuID = Id;
      this.SubMenus = this.allMenus[sb].SubCategories;
      if (this.SubMenus.length > 0) {
        this.subcategoryEnable=true;
      }
      else{
        this.subcategoryEnable=false;
        this.SearchForm.controls['subCategory'].reset();
      }
    }
  }
};

changeCategorySelection(selectedcategory)
{ 
  if(selectedcategory!=undefined)
  {
  this.selectedcategorydata=selectedcategory;
  this.catetegoryID=this.selectedcategorydata.Id;
  this.categoryName=this.selectedcategorydata.Name;
  this.changeFilterSelection(this.catetegoryID,this.categoryName)
  }
}


changeSubCategorySelection(selectedsubcategory)
{
  if(selectedsubcategory!=undefined)
  {
    this.selectedsubcategorydata=selectedsubcategory;
    this.subcatetegoryID=this.selectedsubcategorydata.Id;
    this.subcategoryName=this.selectedsubcategorydata.Name;
    this.changeFilterSeleDetails(this.subcatetegoryID);
  }
}
changeFilterSeleDetails(categoryId) {
  if (this.SubMenus.length > 0) {
    for (var sc = 0; sc < this.SubMenus.length; sc++) {
      var SubMenuID = this.SubMenus[sc].Id;
      if (categoryId === SubMenuID) {
        this.subMenuActiveId = SubMenuID;
      }
    }
  }
}
getBowlsCount(Id){
  this.rows=[];
  this.temp=[];
  let fromdate=this.SearchForm.value.fromDate;
  let todate=this.SearchForm.value.toDate;
  let transformfdate=this.datepipe.transform(fromdate,'yyyy-MM-dd');
  let transformtdate=this.datepipe.transform(todate,'yyyy-MM-dd');
  this.fromDate=fromdate
  this.toDate=todate;
  let storeId=this.SearchForm.value.storedata;
  let categoryId=Id;
  this.excelcategoryName=this.SearchForm.value.category.Name;
  if(!!this.SearchForm.value.subCategory)
  {
    this.excelsubcategoryName=this.SearchForm.value.subCategory.Name;
  }
  else
  {
    this.excelsubcategoryName=null;
  }
  let mainCategoryFlag=this.AllMainCate;
  if(!storeId)
  {
    storeId=0;
  }
 this.storeId=storeId;
  document.getElementById('preloader-div').style.display = 'block';
      this.service.getIngradientReport(storeId,transformfdate,transformtdate,categoryId,mainCategoryFlag).subscribe((response) => { 
        console.log(response);
        this.bowlsCount=response;
        this.excelSalesData = response;
        const datafortable: any = [];
        for (let i = 0; i < response.length; i++) {
            response[i].sno = i + 1;
            datafortable.push(response[i]);
        }
        this.filterData.gridData = datafortable;
        this.dataSource = new MatTableDataSource(datafortable);
        this.filterData.dataSource=this.dataSource;
        this.dataSource.paginator = this.paginator;
        document.getElementById('preloader-div').style.display = 'none';
     
       if(!!this.bowlsCount)
       {
        if(this.bowlsCount.length>0){
          this.noData=false;
          this.exportToExcel();
        }else{
          this.noData=true;
          this.sendReceiveService.showDialog('There is no data to export');
        }
        console.log(this.bowlsCount);

       }
   
      })
      
}


getAllIsActiveStorePickupPoints()
{
  document.getElementById('preloader-div').style.display = 'block';
  this.service.getAllIsActiveStorePickupPoints().subscribe((response) => {
      this.StorePickupPontsList=response;
      this.StorePickupPontsList.push(this.tempstoreobj);
        document.getElementById('preloader-div').style.display = 'none';
  }, (error) => {
        document.getElementById('preloader-div').style.display = 'none';
        this.sendReceiveService.showDialog("Something went wrong");
  });
}



onChangeStoreStatusSelect()
{
    this.filterData.dataSource=[];
    this.paginator=null;
    this.filterData.dataSource.paginator = [];
    this.filterData.dataSource.filteredData=[];
    this.rows=[];
    this.temp=[]; 
    this.excelSalesData=[];
    this.bowlsCount=[];
}


getIngradientReport()
{
    this.submitted=true;
    if (this.SearchForm.valid) {
      this.fromDate = this.datepipe.transform(this.SearchForm.value.fromDate, 'yyyy-MM-dd');
      this.toDate = this.datepipe.transform(this.SearchForm.value.toDate, 'yyyy-MM-dd');
      var categoryName="";
      this.ActiveMenuID = this.allMenus[0].Id;
      this.bowlsData=true;

      if(this.SubMenus.length>0 && !!this.SearchForm.value.subCategory)
      {
        this.subcatetegoryID=this.SearchForm.value.subCategory.Id;
        this.Id=this.subcatetegoryID;
        this.AllMainCate=0;
      }
      else
      {
        if(!!this.subcategoryEnable)
        {
          this.Id=this.SearchForm.value.category.Id;
          this.AllMainCate=1;
        }
        else
        {
          this.Id=this.SearchForm.value.category.Id;
          this.AllMainCate=0;
        }
      }
      this.getBowlsCount(this.Id);
  } else {
    Object.keys(this.SearchForm.controls).forEach(field => { 
        const control = this.SearchForm.get(field);         
        control.markAsTouched({ onlySelf: true });       
      });
  }
 
}


exportToExcel() {
  if (this.excelSalesData.length > 0) {
    var createXLSLFormatObj = [];
    var xlsCompany=['KIPOS Ingredients Count Report']
    createXLSLFormatObj.push(xlsCompany);
    for(var i=0;i<this.StorePickupPontsList.length;i++)
    {
      if(this.storeId==this.StorePickupPontsList[i].Id)
      {
       var store=['Outlet:',this.StorePickupPontsList[i].Name];
       var StoreName=this.StorePickupPontsList[i].Name;
       break;
      }
    }
    if(!store || !StoreName)
    {
      store=['Outlet:',"AllStores"];
      StoreName="AllStores";
    }
    let transformfdate=this.datepipe.transform(this.fromDate,'MM/dd/yyyy');
    let trandfromtdate=this.datepipe.transform(this.toDate,'MM/dd/yyyy');
    this.reportname= "IngredientsCountReport("+ StoreName + "_FROM_"+ transformfdate+ "_TO_" +trandfromtdate +").xlsx"
    createXLSLFormatObj.push(store);
    if(!!this.excelsubcategoryName)
    {
      var categoryandsubcategorydata=['CategoryName',this.excelcategoryName,'SubCategoryName',this.excelsubcategoryName];
      createXLSLFormatObj.push(categoryandsubcategorydata);
    }
    else
    {
      var categorydata=['CategoryName',this.excelcategoryName];
      createXLSLFormatObj.push(categorydata);
    }
    var store1=['DATE','From',transformfdate,'TO',trandfromtdate]
    createXLSLFormatObj.push(store1);
    var headersforexceldata=['S.NO.','IngredientName','IngredientCount']
    createXLSLFormatObj.push(headersforexceldata);
    $.each(this.excelSalesData, function(index, value) {
      var innerRowData = [];
      $.each(value, function(ind, val) {
          innerRowData.push(val);
      });
      innerRowData=[value.sno,value.IngradientName,value.ProductAttributeValueCount]
      createXLSLFormatObj.push(innerRowData);
  });

    createXLSLFormatObj.push(this.excelSalesData);
    var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);     

    const workbook: XLSX.WorkBook = { Sheets: { "IngredientsCountReport"  : ws }, SheetNames: ['IngredientsCountReport'] };
    XLSX.writeFile(workbook, this.reportname , { bookType: 'xlsx', type: 'buffer' });
  } else {
    alert("There is no data to export");
  }


}
exportToPdf() {
if (this.excelSalesData.length > 0) {
    var rows = [];
      var col = ['S.NO.','IngredientName','IngredientCount'];
        for(var key in this.excelSalesData){
            var temporary = [parseInt(key)+1, this.excelSalesData[key].IngradientName,this.excelSalesData[key].ProductAttributeValueCount];
            rows.push(temporary);
        }
      for(var i=0;i<this.StorePickupPontsList.length;i++)
      {
        if(this.storeId==this.StorePickupPontsList[i].Id)
        {
        var store=['Outlet:',this.StorePickupPontsList[i].Name];
        var StoreName=this.StorePickupPontsList[i].Name;
        break;
        }
      }
      if(!store || !StoreName)
      {
        store=['Outlet:',"AllStores"];
        StoreName="AllStores";
      }
      var reportname;
      let transformfdate=this.datepipe.transform(this.fromDate,'MM/dd/yyyy');
      let trandfromtdate=this.datepipe.transform(this.toDate,'MM/dd/yyyy');
      reportname= "IngredientCountReport("+ StoreName + "_FROM_"+ transformfdate+ "_TO_" +trandfromtdate +").pdf"
      this.excelService.exportAsPdf(col,rows,reportname);
  } else {
    alert("There is no data to export");
  }
}

}