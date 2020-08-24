import { Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort, MatDialogRef, MatDialog } from '@angular/material';
import { OurStoryModel, IPageLevelPermissions, AboutUsModel } from 'src/app/helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';

import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { ComfirmComponent } from '../../app-dialogs/confirmation_dialogue/confirmation_dialogue.component';
import { environment } from 'src/environments/environment';
import { AboutUsComponentService } from './aboutUs.service';
import * as $ from "jquery";
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { DatePipe } from '@angular/common';
import { ExportService } from 'src/app/services/common/exportToExcel.service';


@Component({
  selector: 'app-aboutUs',
  templateUrl: './aboutUs.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AboutUsComponent implements OnInit {
  appURL = environment.url;
  displayedColumns = ['sno', 'TitleName', 'SubTitleName', 'image', 'ShortDescription', 'IsActive', 'Actions']
  dataSource: MatTableDataSource<AboutUsModel>;
  gridData = [];
  temp=[];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  dialogRef: MatDialogRef<any>;
  filterData;
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  constructor(private spinner: NgxSpinnerService,
    private router: Router,
    public service: AboutUsComponentService,
    public ref: ChangeDetectorRef,
    public dialog: MatDialog,
    public viewContainerRef: ViewContainerRef,
    public logoutService: LogoutService,
    public datepipe:DatePipe,
    public exportService:ExportService,
    public appInfoService: AppInfoService,
    public sendReceiveService: SendReceiveService,
    public translate: TranslateService,
  ) {
  }

  ngOnInit() {
    this.filterData = {
      filterColumnNames: [
        { "Key": 'sno', "Value": " " },
        { "Key": 'TitleName', "Value": " " },
        { "Key": 'SubTitleName', "Value": " " },
        { "Key": 'image', "Value": " " },
        { "Key": 'ShortDescription', "Value": " " },
        { "Key": 'IsActive', "Value": " " },
      ],
      gridData: this.gridData,
      dataSource: this.dataSource,
      paginator: this.paginator,
      sort: this.sort
    };
    this.getAllAboutUs();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
      this.pagePermissions = pageLevelPermissions.response;
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
  }
  getAllAboutUs() {
      document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllAboutUs().subscribe((response) => {
      this.temp=response;
       document.getElementById('preloader-div').style.display = 'none';;
      const userData: any = [];
      for (let i = 0; i < response.length; i++) {
        response[i].sno = i + 1;
        response[i].IsActive = response[i].IsActive.toString();
        userData.push(response[i]);
      }
      this.filterData.gridData = userData;
      this.dataSource = new MatTableDataSource(userData);
      this.filterData.dataSource = this.dataSource;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
       document.getElementById('preloader-div').style.display = 'none';;

    }, (error) => {

       document.getElementById('preloader-div').style.display = 'none';;

    });
  }
  editStory(id: number) {
    this.service.getAboutUsListById(id)
    this.router.navigate(['landingpage/aboutUs/addAboutUs/' + id]);
    this.service.UserView(false);
  }
  deleteStory(id) {
    const dialogRef = this.dialog.open(ComfirmComponent, {
      width: '300px',
      height: "180px",
      data: "Do you want to Delete?"

    });
    dialogRef.afterClosed().subscribe(result => {
      if (!!result) {
        this.service.deleteAboutUs(id).subscribe((data) => {
          this.getAllAboutUs();
        }, error => {

        });
      }
    });
  }

  updatePagination(){
   this.filterData.dataSource=this.filterData.dataSource;
   this.filterData.dataSource.paginator = this.paginator;
  }

  activateRecord(event, id) {

    if (event == true) {
      const dialogRef = this.dialog.open(ComfirmComponent, {
        width: '300px',
        height: "180px",
        data: "Do you want to change Status ?"

      });
      dialogRef.afterClosed().subscribe(result => {
        if (!!result) {
          this.service.activateAboutUsRecord(id).subscribe((data) => {
            this.getAllAboutUs();
          }, error => {

          });
        }
        this.getAllAboutUs();
      });
    }
    else {
      this.sendReceiveService.showDialog('Atleast one status is to be Active');
      this.getAllAboutUs();
    }
  }

  exportToPdf() {
  
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
       
          var col = ['sno','TitleName','SubTitle Name','ImagePath','ShortDescription','Status'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].TitleName,this.temp[key].SubTitleName,this.temp[key].ImagePath,this.temp[key].ShortDescription,this.temp[key].IsActive];
                rows.push(temporary);
            }
            let reportname = "Aboutus.pdf"
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
          var temporary = [(parseInt(key) +1), this.temp[key].TitleName,this.temp[key].SubTitleName,this.temp[key].ImagePath,this.temp[key].ShortDescription,this.temp[key].IsActive];
          rows.push(temporary);
      }
       var createXLSLFormatObj = [];
        var xlsHeader = ['sno','TitleName','SubTitle Name','ImagePath','ShortDescription','Status'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.TitleName +  "</td><td>" + value.SubTitleName +  "</td><td>" + value.ImagePath +  "</td><td>" + value.ShortDescription +   "</td><td>" +value.IsActive + "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "Aboutus.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'Aboutus': ws }, SheetNames: ['Aboutus'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }






}
