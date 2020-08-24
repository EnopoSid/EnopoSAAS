import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { LogoutService } from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import { AppInfoService } from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { MemberService } from '../members/member.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, EnquiryModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';
import { CloseScrollStrategy } from '@angular/cdk/overlay';
import { DatePipe } from '@angular/common';
declare var jsPDF :any; 
import {ExportService } from '../../services/common/exportToExcel.service';
import * as $ from 'jquery';
import * as XLSX from 'xlsx';


@Component({
    selector: 'memberSubscription-table',
    templateUrl: './memberSubscription.component.html',
    encapsulation: ViewEncapsulation.None
})

export class GetMemberSubscriptionComponent implements OnInit {
    displayedColumns = ['sno','memberId','fullName','planName','email','SubscriptionDate','nextDueDate']
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
                public service: MemberService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public exportService:ExportService,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService,  
                private actRoute: ActivatedRoute,
                private activatedRoute: ActivatedRoute,
                public datepipe : DatePipe,
                ) {
    }



    ngOnInit() {
            document.getElementById('preloader-div').style.display = 'block';
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'memberId',"Value":" "},
              {"Key":'fullName',"Value":" "},
              {"Key":'planName',"Value":" "},
              {"Key":'email',"Value":" "},
              {"Key":'SubscriptionDate',"Value":" "},
              {"Key":'nextDueDate',"Value":" "},
              {"Key":'isActive',"Value":" "},
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
            this.service.getAllCustomers().subscribe((response) => {
                this.temp=response;
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
                  document.getElementById('preloader-div').style.display = 'none';
            }, (error) => {
                  document.getElementById('preloader-div').style.display = 'none';
            });
    }
    updatePagination(){
       this.filterData.dataSource=this.filterData.dataSource;
       this.filterData.dataSource.paginator = this.paginator;
      } 
      
    actionAfterError () {
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this.router.navigate(['/sessions/signin']);
        });
    }

  
    exportToExcel() {
       
        if(this.temp.length!=0){
            this.drawTable(this.temp);
            var rows=[];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].memberId,this.temp[key].fullName,this.temp[key].planName,this.temp[key].email,(this.datepipe.transform(this.temp[key].SubscriptionDate, 'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].nextDueDate, 'yyyy/MM/dd'))];
                rows.push(temporary);
            }
            var createXLSLFormatObj = [];
            var xlsHeader = ['sno','CustomerNo ','CustomerName','Subscribedplan','EmailId','SubscriptionDate','DueDate'];
            createXLSLFormatObj.push(xlsHeader);
         $.each(rows, function(index, value) {
                var innerRowData = [];
                $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.FullName +  "</td><td>" + value.Email + "</td><td>" + value.PlanName +  "</td><td>" +value.SubscriptionDate+ "</td></tr>"+value.nextDueDate+ "</td></tr>");

            
                $.each(value, function(ind, val) {
        
                    innerRowData.push(val);
                });
               createXLSLFormatObj.push(innerRowData);

            });
                
            var filename = "MemberSubscription.xlsx";
           
            var  ws = XLSX.utils.aoa_to_sheet( createXLSLFormatObj);
            const workbook: XLSX.WorkBook = { Sheets: { 'MemberSubscription': ws }, SheetNames: ['MemberSubscription'] };
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
        this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>" + rowData.memberId + "</td><td>" + rowData.fullName + "</td><td>" + rowData.planName + "</td><td>" + rowData.email + "</td><td>"+(this.datepipe.transform(rowData.SubscriptionDate, 'dd/MM/yyyy')) + "</td><td>"+(this.datepipe.transform(rowData.nextDueDate,  'dd/MM/yyyy')) +"</td></tr>";
     this.excelRowCount++;
     }
      drawTable(data) {  
        this.excelRowCount = 1;
        for (var i = 0; i < data.length; i++) {
            if (i == 0) {
                this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
                this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
                this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>Memberid </th><th>FullName</th><th>PlanName</th><th>Email</th><th>SubscriptionDate</th><th>NextDueDate</th></tr></thead><tbody>";
            }
            this.drawRow(data[i]);
            if (i == data.length - 1) {
                this.tableFormatofData = this.tableFormatofData + "</tbody></table>";
                this.tableFormatofData = this.tableFormatofData + "</body></html>";
            }
        }
      
    }
















}