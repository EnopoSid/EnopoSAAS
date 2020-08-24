import {Component, OnInit, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogRef, MatTableDataSource, MatPaginator, MatSort, MatDatepicker} from '@angular/material';
import { FreeMembershipService } from './freeMembership.service';
import {Router} from '@angular/router';
import {AppInfoService} from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, ConfigurationModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { Subject } from 'rxjs';
import * as $ from "jquery";
declare var  jsPDF:any;
import * as XLSX from 'xlsx';
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'app-freeMembership',
    templateUrl: './freeMembership.component.html',
    styleUrls: ['./freeMembership.component.css']
})
export class FreeMembershipComponent implements OnInit {
    displayedColumns = ['sno','MemberId','FullName','EmailId','MobileNumber','CreatedDate','StartDate','EndDate','Duration']
    dataSource: MatTableDataSource<ConfigurationModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    formErrors: any;
    status: boolean;
    title: string;
    @ViewChild('myTable', {static: false}) table: any;
    @ViewChild('tableWrapper', {static: false}) tableWrapper;
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    AddFreeMembershipFlag:boolean=false;
    FreeMembershipForm:FormGroup;
    userId: number = 0;
    idOnUpdate: number =0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    selectedFiles: FileList;
    minDate;
    maxDate;
    maxDateforToDate=null;
    minDateforToDate;
    oneDay = 24*60*60*1000;
    diffDays;
    generatedPassword;
    userData;
    dataCount=0;
    existingEmails=[];
    existingMobileNumbers=[];
    duplicateEmails=[];
    invalidEmails=[];
    invalidMobileNumbers=[];
    durationList=[{id:0,Value:'1'},
    {id:1,Value:'3'},
    {id:2,Value:'6'},
    {id:3,Value:'9'},
    {id:4,Value:'12'}]

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: FreeMembershipService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        public exportService: ExportService,
        public datepipe:DatePipe, 
        private formBuilder:FormBuilder) {
            this.formErrors = {
                Email: {},
                Duration: {}
            };
}


ngOnInit(){
    this.userId=this.sendReceiveService.globalUserId;
    this.filterData={
        filterColumnNames:[
        {"Key":'sno',"Value":" "},
        {"Key":'FullName',"Value":" "},
        {"Key":'EmailId',"Value":" "},
        {"Key":'MobileNumber',"Value":" "},
        {"Key":'CreatedDate',"Value":" "},
        {"Key":'StartDate',"Value":" "},
        {"Key":'EndDate',"Value":" "},
        {"Key":'Duration',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.getAllfreeMembershipMembers();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{        
        this.pagePermissions =pageLevelPermissions.response;
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    this.FreeMembershipForm = this.formBuilder.group({
        id: 0,
        'Duration' : ['',Validators.compose([Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyNumbers)])],
        'File':[null,Validators.required],
        'fromDate':[null,Validators.required],
        'toDate':[null],
    });

    this.FreeMembershipForm.get('toDate').disable();
}
getAllfreeMembershipMembers(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllfreeMembershipMembers().subscribe((response) => {
        this.temp=response;
        const configurationData: any = [];
            for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                configurationData.push(response[i]);
            }
            this.filterData.gridData = configurationData;
            this.dataSource = new MatTableDataSource(configurationData);
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
               document.getElementById('preloader-div').style.display = 'none';
    }, (error) => {
          document.getElementById('preloader-div').style.display = 'none';
        });
}
selectFile(event) {
    if(event.target.files[0].type=="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"){
        this.selectedFiles = event.target.files;
    }else{
        alert('Please Upload Only Excel containing the column names : sno, fullname, email, mobilenumber');
        event.target.files=null;
        this.FreeMembershipForm.get('File').setValue(null);
    }
}
actionAfterError() {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this.router.navigate(['/sessions/signin']);
    });
}

    duplicateKeyName(){
    const q = new Promise((resolve, reject) => {
        this.service.duplicateConfiguration({
            'ModifiedBy': null,
            'CreatedBy': null,
            'EmailId':this.FreeMembershipForm.controls['Email'].value,
            'MemberShipDuration': this.FreeMembershipForm.controls['Duration'].value,
            'ConfigId': !!this.idOnUpdate ? this.idOnUpdate: 0,
            'IsActive':MyAppHttp.ACTIVESTATUS,
      }).subscribe((duplicate) => {
            if (duplicate) {
                resolve({ 'duplicateKeyName': true });

            } else {
                resolve(null);
            }
        }, () => { resolve({ 'duplicateKeyName': true }); });
    });
    return q;

}
updatePagination(){
   this.filterData.dataSource=this.filterData.dataSource;
   this.filterData.dataSource.paginator = this.paginator;
  }
addFreeMembershipMember(){
    this.AddFreeMembershipFlag=true;
    this.minDate=null;
    this.title="Save"
}
onFreeMembershipFormSubmit() {
    
    let StartDate: string = this.FreeMembershipForm.value.fromDate;
    let duration: string = this.FreeMembershipForm.value.Duration;
    let FileUpload = this.selectedFiles;
    if (!this.FreeMembershipForm.valid) {
        return;
    }
       document.getElementById('preloader-div').style.display = 'block';
    if (this.title == "Save") {

        this.service.saveFreeMemberShipMember({
            'MemberShipDuration': parseInt(duration),
            'StartDate':new Date(StartDate).toLocaleDateString(),
            'EndDate':this.maxDateforToDate,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
        },FileUpload).subscribe((data) => {
            if(data!=null){
                if(data.existingData.length !=0){
                    this.existingEmails.push(data.existingData);
                }
                if(data.duplicateData.length!=0){
                    this.duplicateEmails.push(data.duplicateData);
                }
                if(data.invalidData.length!=0){
                    this.invalidEmails.push(data.invalidData);
                }
            }
            this.service.getLatestRecords().subscribe((response) => {
                if(response.length==0){
                    if(this.existingEmails.length!=0 ||this.duplicateEmails.length!=0 || this.invalidEmails.length!=0){
                        this.alertMessages();
                    }
                }else{
                    this.userData=response;
                    this.registration(this.userData);
                }
            },error=>{

            });

    },error=>{
        this.existingEmails=error.error.existingData;
        this.duplicateEmails=error.error.duplicateData;
        this.invalidEmails=error.error.invalidData;
        if(this.existingEmails.length!=0 || this.existingMobileNumbers.length!=0 ||this.duplicateEmails.length!=0 || this.invalidEmails.length!=0||this.invalidMobileNumbers.length!=0 ){
            this.alertMessages();
        }
    });
    this.AddFreeMembershipFlag=false;
}
}

registration(userData){
    userData[this.dataCount].Password=this.generateRandomPassword();
    var requestObj={
        "EmailId":userData[this.dataCount].EmailId,
        "MobileNumber":userData[this.dataCount].MobileNumber,
        "UserName":userData[this.dataCount].FullName,
        "CustomerGUID": "",
        "FirstName": userData[this.dataCount].FullName,
        "LastName": ".",
        "Password":userData[this.dataCount].Password
    }
    this.service.registerInNop(requestObj).subscribe((success)=>{
        var memberShipObj = {
            "MemberBasicDetails": {
                "FullName": success.Username
            },
            "MemberLoginDetails": {
                "Email": success.Email,
                "MobileNumber": success.MobileNumber,
                "Password": userData[this.dataCount].Password,
                "CustomerGuid": success.CustomerGuid,
                "CustomerId": success.CustomerId,
                "IsPasswordChanged":true,
            },
            "Duration":parseInt(this.FreeMembershipForm.value.Duration)
        };
            this.service.registerInMemberShip(memberShipObj).subscribe((formData)=>{
                this.dataCount++;
                if(this.dataCount<this.userData.length){
                    this.registration(this.userData);
                }else{
                    this.getAllfreeMembershipMembers();
                    this.FreeMembershipForm.reset();
                     document.getElementById('preloader-div').style.display = 'none';;
                    if(this.existingEmails.length!=0 ||this.duplicateEmails.length!=0 || this.invalidEmails.length!=0 ){
                        this.alertMessages();
                    }
                    this.sendReceiveService.showDialog("Added Successfully");
                }
            },error=>{

            });
    },error=>{

    });
}

alertMessages(){
    let alreadyRegisteredMsg="These below records are already registered:\n"+this.existingEmails.toString().split(',').join(" \n ")+";";
    let duplicateExcelMsg="These below records are duplicate in excel::\n"+this.duplicateEmails.toString().split(',').join(" \n ")+";" 
    let invalidExcelMsg="These below records are invalid :\n"+this.invalidEmails.toString().split(',').join(" \n ")+";"
        this.FreeMembershipForm.reset();
         document.getElementById('preloader-div').style.display = 'none';;
        if(this.existingEmails.length!=0 && this.duplicateEmails.length!=0 && this.invalidEmails.length!=0){
            alert(""+alreadyRegisteredMsg+"\n"+duplicateExcelMsg+"\n"+invalidExcelMsg+"");
        }else if(this.existingEmails.length!=0 && this.duplicateEmails.length!=0){
            alert(""+alreadyRegisteredMsg+"\n"+duplicateExcelMsg+"");
        }else if(this.duplicateEmails.length!=0 && this.invalidEmails.length!=0){
            alert(""+duplicateExcelMsg+"\n"+invalidExcelMsg+"");
        }else if(this.existingEmails.length!=0 && this.invalidEmails.length!=0){
            alert(""+alreadyRegisteredMsg+"\n"+invalidExcelMsg+"")
        }else if(this.existingEmails.length!=0){
            alert(""+alreadyRegisteredMsg+"");
        }else if(this.duplicateEmails.length!=0){
            alert(""+duplicateExcelMsg+"");
        }else if(this.invalidEmails.length!=0){
            alert(invalidExcelMsg);
        }
        
        this.existingEmails=[];
        this.existingMobileNumbers=[];
        this.duplicateEmails=[];
        this.invalidEmails=[];
        this.invalidMobileNumbers=[];
        this.maxDateforToDate=null;
        this.minDateforToDate=null;
        this.minDate=new Date();
        this.userData=null;
}

generateRandomPassword() {
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  
    for (var i = 0; i < 8; i++)
      text += possible.charAt(Math.floor(Math.random() * possible.length));
  
    return text;
  }

onCancel(){
    this.idOnUpdate=0;
    this.FreeMembershipForm.reset();
    this.AddFreeMembershipFlag=false;
    this.maxDateforToDate=null;
    this.minDateforToDate=null;
    this.minDate=new Date();
}

clkFromDate(picker: MatDatepicker<Date>){
    picker.open();
    this.minDate = new Date();
 }

 clkToDate(picker: MatDatepicker<Date>){
     picker.open();
  }
  onChangeFromDate(selectedDate){
     if(!!this.FreeMembershipForm.value.Duration){
        var tempMinDate=new Date(selectedDate);
        this.maxDateforToDate = this.minDateforToDate = new Date(tempMinDate.setDate(tempMinDate.getDate() +  parseInt( this.FreeMembershipForm.value.Duration)*30)).toLocaleDateString();
     }

 }

 onChangeDuration(selectedDuration){
     if(this.minDate!=null){
        var tempMinDate=new Date(this.FreeMembershipForm.value.fromDate); 
        this.maxDateforToDate = this.minDateforToDate = new Date(tempMinDate.setDate(tempMinDate.getDate() + (parseInt(selectedDuration)* 30))).toLocaleDateString();
     }
 }


 exportToPdf() {
  
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
       
          var col = ['sno','MemberId','Member  Name','EmailId','Mobile   Number','Registration  Date','Subscription Date','DueDate','Duration'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].EmailId,this.temp[key].MobileNumber,(this.datepipe.transform(this.temp[key].CreatedDate, 'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].StartDate, 'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].Enddate, 'yyyy/MM/dd')),this.temp[key].MemberShipDuration];
                rows.push(temporary);
            }
            let reportname = "FreeMembership.pdf"
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
            var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].EmailId,this.temp[key].MobileNumber,(this.datepipe.transform(this.temp[key].CreatedDate, 'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].StartDate, 'yyyy/MM/dd')),(this.datepipe.transform(this.temp[key].Enddate, 'yyyy/MM/dd')),this.temp[key].MemberShipDuration];
            rows.push(temporary);
        }

        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','MemberId','Member  Name','EmailId','Mobile   Number','Registration  Date','Subscription Date','DueDate','Duration'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.FullName +  "</td><td>" + value.EmailId +  "</td><td>" + value.MobileNumber +   "</td><td>" +value.CreatedDate +   "</td><td>" +value.StartDate +   "</td><td>" +value.Enddate +   "</td><td>" +value.MemberShipDuration + "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "FreeMembership.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'FreeMembership': ws }, SheetNames: ['FreeMembership'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }
  



}
