import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator,MatSort} from '@angular/material';

import {Route, ActivatedRoute, Router} from '@angular/router';

import { NewsLetterMailService } from './news-letter-mail.service';


import {NgxSpinnerService} from 'ngx-spinner';
import {TranslateService} from '@ngx-translate/core';
import {Subject} from 'rxjs/internal/Subject';
import { CloseScrollStrategy } from '@angular/cdk/overlay';

declare var jsPDF: any;  
import { DatePipe } from '@angular/common'
import { EnquiryModel, IPageLevelPermissions } from 'src/app/helpers/common.interface';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
@Component({
    selector: 'news-letter-mail-table',
    templateUrl:"./news-letter-mail.component.html",
    encapsulation: ViewEncapsulation.None
})

export class NewsLetterMailComponent implements OnInit{
    displayedColumns = ['sno','EmailID','CreatedDate','CreatedTime']
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
    constructor(private spinner: NgxSpinnerService,
        private router: Router,
        private route:ActivatedRoute,
        public service: NewsLetterMailService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public exportService:ExportService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,  
        private actRoute: ActivatedRoute,
        private activatedRoute: ActivatedRoute,
        public datepipe: DatePipe) {
}
ngOnInit() {
    this.filterData={
        filterColumnNames:[
          {"Key":'sno',"Value":" "},
          {"Key":'EmailID',"Value":" "},
          {"Key":'CreatedDate',"Value":" "},
          {"Key":'CreatedTime',"Value":" "}
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.getAllMembers();
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

getAllMembers() {  
    document.getElementById('preloader-div').style.display = 'block'; 
        this.service.getAllEmails().subscribe((response) => {
            console.log(JSON.stringify(response))
            this.temp = response;
            const memberData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                var date = response[i].CreatedDate;
                response[i].time = this.datepipe.transform(new Date(date),"HH:mm");
                response[i].CreatedDate = this.datepipe.transform(new Date(response[i].CreatedDate),"dd-MM-yyyy");
                
                memberData.push(response[i]);
            }
            this.filterData.gridData = memberData;
            this.dataSource = new MatTableDataSource(memberData);
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;

              document.getElementById('preloader-div').style.display = 'none';
        }, (error) => {
              document.getElementById('preloader-div').style.display = 'none';
        });
}


exportToPdf() {
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
        var col = ["S.No",'Email','Created Date','Created Time'];
        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].EmailId,this.temp[key].CreatedDate,this.temp[key].time];
            rows.push(temporary);
        }
        let reportname = "news-letter-emails.pdf"
        this.exportService.exportAsPdf(col,rows,reportname);
    }
    else {
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
}
exportToExcel(event) {
    if(this.temp.length!=0){
        this.drawTable(this.temp);
        this.exportService.exportAsExcelFile(this.tableFormatofData, 'news-letter-emails.xls');
    }
    else{
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
  }
  tableFormatofData = "";
excelRowCount = 0;
borderClass='border: 1px solid black;border-collapse: collapse;'

  drawRow(rowData) {
    this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>" + rowData.EmailId + "</td><td>"+rowData.CreatedDate + "</td><td>"+rowData.time +"</td></tr>";
    this.excelRowCount++;
}
  drawTable(data) {
    this.excelRowCount = 1;
    for (var i = 0; i < data.length; i++) {
        if (i == 0) {
            this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
            this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
            this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>Email </th><th>CreatedDate</th><th>CreatedTime</th></tr></thead><tbody>";
        }
        this.drawRow(data[i]);
        if (i == data.length - 1) {
            this.tableFormatofData = this.tableFormatofData + "</tbody></table>";
            this.tableFormatofData = this.tableFormatofData + "</body></html>";
        }
    }
  
}
}