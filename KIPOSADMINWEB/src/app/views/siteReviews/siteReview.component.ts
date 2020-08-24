import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { LogoutService } from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import { AppInfoService } from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { SiteReviewService} from './siteReview.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, EnquiryModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';
import { CloseScrollStrategy } from '@angular/cdk/overlay';
import * as $ from "jquery";
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'siteReview-table',
    templateUrl: './siteReview.component.html',
    encapsulation: ViewEncapsulation.None
})

export class GetSiteReviewComponent implements OnInit {
    displayedColumns = ['sno','FullName','Rating','ReviewText','FavouriteBowls','Title','IsApproved','Actions']
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
    dialogRef: MatDialogRef<any>;
    usersFromServiceExists ;
    sample : string;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    permisssionList: any;
    selectedvalues: string;
    selectedApprovied = [];
    complatedata  : any;

    constructor(private spinner: NgxSpinnerService,
                private router: Router,
                private route:ActivatedRoute,
                public service: SiteReviewService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService,  
                public datepipe:DatePipe,
                public exportService:ExportService,
                private actRoute: ActivatedRoute,
                private activatedRoute: ActivatedRoute,) {
    }



    ngOnInit() {
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'FullName',"Value":" "},
              {"Key":'Rating',"Value":" "},
              {"Key":'ReviewText',"Value":" "},
              {"Key":'FavouriteBowls',"Value":" "},
              {"Key":'Title',"Value":" "},
              {"Key":'IsApproved',"Value":" "},
              {"Key":'Id',"Value":" "},
              
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
          
        this.getAllRevies();
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
       this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    }


    updatePagination(){
        this.filterData.dataSource=this.filterData.dataSource;
        this.filterData.dataSource.paginator = this.paginator;
        }
        
    handlePageChange (event: any): void {
    }

    openResume(filePath) {
        window.open(filePath);
    }

    getAllRevies() {
        document.getElementById('preloader-div').style.display = 'block';
            this.service.getAllSiteReviews().subscribe((apiresponse) => {
               var  response = apiresponse.CustomSiteReview;
                const memberData: any = [];
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1;
                    memberData.push(response[i]);
                }
                this.filterData.gridData = memberData;
                this.dataSource = new MatTableDataSource(memberData);
                this.filterData.dataSource=this.dataSource;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                this.complatedata =  this.filterData.dataSource
                  document.getElementById('preloader-div').style.display = 'none';
            }, (error) => {
                  document.getElementById('preloader-div').style.display = 'none';
            });
    }

  
    onCheckboxModalChange(id){
        if(!!id){
           let selectedReview =  this.filterData.gridData.find(x => x.Id == id)
            this.service.updateIsApproved(selectedReview).subscribe((response) => {
                this.sendReceiveService.showDialog("Review  Status Changed")
            });
           this.removefromselection(id);
        }
    }
    updateAllApprove(){
        if(this.selectedApprovied.length > 0){
            let  selectedReviewvalues = [] ;
            let sample = this.complatedata;
            for(let i = 0 ; i< this.selectedApprovied.length; i++)
            {
                selectedReviewvalues.push(this.filterData.gridData.find(x => x.Id == this.selectedApprovied[i])) 
            }
            this.service.UpdateAllIsApproved(selectedReviewvalues).subscribe((response) => {
                this.sendReceiveService.showDialog(response)
            });
           
        }
    }
    oncheangeisApproved(id){
        if(this.selectedApprovied.indexOf(id) < 0){
            this.selectedApprovied.push(id);
        }
    }
    removefromselection(id)
{
    if(this.selectedApprovied.indexOf(id) > 0){
        this.selectedApprovied.splice(this.selectedApprovied.indexOf(id), id);
    }
}
deletereview(id){
    this.service.deletereview(id).subscribe((response) => {
        this.getAllRevies();
      this.sendReceiveService.showDialog(response)
    });
  
}
exportToPdf() {   
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
        var col =['sno','Names','Rating','ReviewText','FavouriteBowls','Title','isAppproved'];
        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].Names,this.temp[key].Rating,this.temp[key].ReviewText,this.temp[key].FavouriteBowls,this.temp[key].Title, this.temp[key].IsAppproved];
            rows.push(temporary);
        }
        let reportname = "SiteReview.pdf"
        this.exportService.exportAsPdf(col,rows,reportname);
    }
    else {
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
}
exportToExcel(event) {
    if(this.temp.length!=0){
 
        var rows = [];

      for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].Names,this.temp[key].Rating,this.temp[key].ReviewText,this.temp[key].FavouriteBowls,this.temp[key].Title, this.temp[key].IsAppproved];
            rows.push(temporary);
        }
    
        var createXLSLFormatObj = [];
        var xlsHeader = ['sno','Names','Rating','ReviewText','FavouriteBowls','Title','isAppproved'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.Names +  "</td><td>" + value.Rating +  "</td><td>" + value.ReviewText +  "</td><td>" + value.FavouriteBowls +   "</td><td>" +value.Title + "</td><td>" +value.IsAppproved + "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "SiteReviews.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'SiteReviews': ws }, SheetNames: ['SiteReviews'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    
 
    }
    else{
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
  }

}