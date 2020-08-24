import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { LogoutService } from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import { AppInfoService } from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { MemberService } from './member.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, EnquiryModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';
import { CloseScrollStrategy } from '@angular/cdk/overlay';
import { FormGroup, Validators } from '@angular/forms';
import { FormBuilder } from '@angular/forms';
declare var jsPDF: any;  
import { DatePipe, NgIf } from '@angular/common';
import {ExportService } from '../../services/common/exportToExcel.service';
import { Template } from '@angular/compiler/src/render3/r3_ast';
import * as $ from 'jquery';
import * as XLSX from 'xlsx';







@Component({
    selector: 'member-table',
    templateUrl: './member.component.html',
    encapsulation: ViewEncapsulation.None
})

export class GetMemberComponent implements OnInit {
    displayedColumns ;
    dataSource: MatTableDataSource<EnquiryModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    MemberForm:FormGroup;
    filterData;
    private currentComponentWidth: number;
    rows = [];
    columns = [];
    temp = [];
    title: string;
    MemberInfor:any= {};
    showMemberDetails:boolean=false;

    dialogRef: MatDialogRef<any>;
    usersFromServiceExists ;
    AddMenuFlag:boolean=false;
    sample : string;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    urlpath : string;
    isShow = true ;
    nonmember:boolean;
    additionalkpoints: boolean=false;
    navservice: any;
    idOnUpdate: any;
    formErrors: any;

    constructor(private spinner: NgxSpinnerService,
                private router: Router,
                private route:ActivatedRoute,
                public service: MemberService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService,  
                private actRoute: ActivatedRoute,
                private activatedRoute: ActivatedRoute,
                private formBuilder:FormBuilder ,
                public datepipe: DatePipe,        
                public exportService:ExportService

            ) {
    }



    ngOnInit() {
      
        this.urlpath = this.router.url.replace('/', '');
       document.getElementById('preloader-div').style.display = 'block';
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'MemberId',"Value":" "},
              {"Key":'FullName',"Value":" "},
              {"Key":'MobileNumber',"Value":" "}, 
              {"Key":'PlanName',"Value":" "},
              {"Key":'CreatedDate',"Value":" "},
              {"Key":'Email',"Value":" "},
              {"Key":'TotalKPoints',"Value":""},
              {"Key":'TotalAmount',"Value":""}
              
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
          if(this.urlpath == "members"){
            this.displayedColumns  =  ['sno','MemberId','FullName','Email','MobileNumber','PlanName','CreatedDate','TotalKPoints','TotalAmount'] ;
            this.getAllMembers();
          }
          else{
              this.nonmember = true
          this.displayedColumns  = ['sno','MemberId','FullName','Email','MobileNumber','CreatedDate','TotalKPoints','TotalAmount']
              this.getAllNonMembers();
          }
   
        this.sendReceiveService.globalPageLevelPermission = new Subject;
        this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
       this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    }   

    updateMainmenu(id){
        this.AddMenuFlag = true;
        this.processEditAction(id);
        this.title = "Update";
        this.MemberForm = this.formBuilder.group({
            id:0,
            'MemberId':  [null, Validators.compose([Validators.required, Validators.minLength(3)])],
            'TotalRewardPoints' : ['', Validators.required]
           });
           this.service.getAllMembers();
    }
    processEditAction(id){
        this.idOnUpdate=id;
        this.service.getMembersByMemberId(id)
            .subscribe(resp => {
                this.MemberForm.patchValue({
                    MemberId: resp.MemberId,
                });
            },  error => this.formErrors = error);
    }

    handlePageChange (event: any): void {
    }

    openResume(filePath) {
        window.open(filePath);
    }
    add_months(dt, n) 
    {
        var date =  new Date(dt)
      return new Date(date.setMonth(date.getMonth() + n));      
    }
    openDialog(): void {
     
        const dialogRef = this.dialog.open(GetMemberComponent, {
           
            
            
        });
    
        dialogRef.afterClosed().subscribe(result => {
          console.log('The dialog was closed');
         
        });
      }

   
      
    getAllMembers() {
        document.getElementById('preloader-div').style.display = 'block';
            this.service.getAllMembers().subscribe((response) => {
                this.temp=response;
                const memberData: any = [];
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1;
                    response[i].nonmember = false;
                    memberData.push(response[i])
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

    getAllNonMembers() {
        document.getElementById('preloader-div').style.display = 'block';
             this.service.getAllNonMembers().subscribe((response) => {
                 this.temp=response;
                 const memberData: any = [];
                 for (let i = 0; i < response.length; i++) {
                     response[i].sno = i + 1;
                     response[i].notamember = true;
                     if(response[i].CreatedDate == null){
                        response[i].CreatedDate = response[i].MemberCreatedDate;
                     }
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

    

    MemberDetails(MemberId){
        this.service.MemberDetails(MemberId).subscribe(success =>{
        this.MemberInfor=success;
        this.showMemberDetails=true;
})
    }





    
    exportToExcel() {
        if(this.nonmember==true){
        if(this.temp.length!=0 ){
            var rows = [];

            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].Email,this.temp[key].MobileNumber,(this.datepipe.transform(this.temp[key].CreatedDate, 'dd/MM/yyyy'))];
                rows.push(temporary);
            }

            var createXLSLFormatObj = [];
            var xlsHeader = ['sno','MemberId','Member Name','EmailID','PhoneNumber','RegistrationDate'];
            createXLSLFormatObj.push(xlsHeader);
         $.each(rows, function(index, value) {
                var innerRowData = [];
               $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.FullName +  "</td><td>" + value.Email +  "</td><td>" + value.MobileNumber +   "</td><td>" +value.CreatedDate+ "</td></tr>");

            
                $.each(value, function(ind, val) {
        
                    innerRowData.push(val);
                });
                createXLSLFormatObj.push(innerRowData);
            });
            var filename = "NonMemberDetails.xlsx";
            var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
            const workbook: XLSX.WorkBook = { Sheets: { 'NonMemberDetails': ws }, SheetNames: ['NonMemberDetails'] };
            XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
        }
        else
        {
            this.sendReceiveService.showDialog('There is No Data Available to Export');

        }
    }

        else if(this.temp.length!=0 ){
            var rows = [];

            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].Email,this.temp[key].MobileNumber,this.temp[key].PlanName,(this.datepipe.transform(this.temp[key].CreatedDate, 'dd/MM/yyyy')), this.temp[key].TotalKPoints, this.temp[key].TotalAmount];
                rows.push(temporary);
            }

            var createXLSLFormatObj = [];
            var xlsHeader = ['sno','MemberId','MemberName','EmailID','PhoneNumber','Subscribed Plan','RegistrationDate','Total kPoints','Total OrderedAmount(GST included)'];
            createXLSLFormatObj.push(xlsHeader);
         $.each(rows, function(index, value) {
                var innerRowData = [];
                $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.FullName +  "</td><td>" + value.Email +  "</td><td>" + value.MobileNumber +  "</td><td>" + value.PlanName +  "</td><td>" +value.CreatedDate+  "</td><td>" +value.TotalKPoints+ +  "</td><td>" +value.TotalAmount+"</td></tr>");
                $.each(value, function(ind, val) {
        
                    innerRowData.push(val);
                });
                createXLSLFormatObj.push(innerRowData);

            });      
            
            var filename = "MemberDetails.xlsx"; 
            var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
            const workbook: XLSX.WorkBook = { Sheets: { 'MemberDetails': ws }, SheetNames: ['MemberDetails'] };
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
          if (this.nonmember==true){
        this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>" + rowData.MemberId + "</td><td>" + rowData.FullName + "</td><td>" + rowData.Email + "</td><td>" + rowData.MobileNumber + "</td><td>"+(this.datepipe.transform(rowData.CreatedDate, 'dd/MM/yyyy')) + "</td><td>";
          }
          else{
            this.tableFormatofData = this.tableFormatofData + "<tr style='border: 1px solid black;border-collapse: collapse;'><td>" + this.excelRowCount + "</td><td>" + rowData.MemberId + "</td><td>" + rowData.FullName + "</td><td>" + rowData.Email + "</td><td>" + rowData.MobileNumber + "</td><td>" + rowData.PlanName + "</td><td>"+(this.datepipe.transform(rowData.CreatedDate, 'dd/MM/yyyy')) + "</td><td>";
                 }
        this.excelRowCount++;
    }
      drawTable(data) { 
        this.excelRowCount = 1;
        for (var i = 0; i < data.length; i++) {
            if (i == 0) {
                if(this.nonmember==true)
                {
                    this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
                    this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
                    this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>MemberId </th><th>FullName</th><th>Email</th><th>MobileNumber</th><th>CreatedDate</th></tr></thead><tbody>";
                }
                else{
                this.tableFormatofData = "<html><head><style> #tblreportdata123 tr th, #tblreportdata123 tr td { text-align: center; }</style></head><body>";
                this.tableFormatofData = this.tableFormatofData + "<table id='tblreportdata123' class='hide'>";
                this.tableFormatofData = this.tableFormatofData + "<thead style='border: 1px solid black;border-collapse: collapse;'><tr style='border: 1px solid black;border-collapse: collapse;' ><th>S.No.</th><th>MemberId </th><th>FullName</th><th>Email</th><th>MobileNumber</th><th>PlanName</th><th>CreatedDate</th></tr></thead><tbody>";
                }
            }
            this.drawRow(data[i]);
            if (i == data.length - 1) {
                this.tableFormatofData = this.tableFormatofData + "</tbody></table>";
                this.tableFormatofData = this.tableFormatofData + "</body></html>";
            }
        }
      
    }

    onCancel(){
        this.idOnUpdate=0;
        this.MemberForm.reset();
        this.AddMenuFlag=false;
    }
}