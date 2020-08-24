import {Component, OnInit, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogRef, MatTableDataSource, MatPaginator, MatSort, MatDatepicker} from '@angular/material';
import { PlanConversionService } from './planConversion.service';
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
import * as $ from 'jquery';
import * as XLSX from 'xlsx';
declare var jsPDF :any;
import { ExportService } from 'src/app/services/common/exportToExcel.service';
import { DatePipe } from '@angular/common';



@Component({
    templateUrl: './planConversion.component.html',
})
export class PlanConversionComponent implements OnInit {
    displayedColumns = ['Actions','sno','MemberId','FullName','EmailId','MobileNumber',"SubscriptionDate","EndDate"]
    dataSource: MatTableDataSource<ConfigurationModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    filterData;
    formErrors: any;x
    status: boolean;
    title: string;
    @ViewChild('myTable', {static: false}) table: any;
    @ViewChild('tableWrapper', {static: false}) tableWrapper;
    rows = [];
    columns = [];
    temp = [];
    dialogRef: MatDialogRef<any>;
    AddPlanConversionFlag:boolean=false;
    PlanConversionForm:FormGroup;
    userId: number = 0;
    idOnUpdate: number =0;
    mode =-1;
    model = 0;
    memberFlag:boolean=false;
    nonmemberFlag:boolean=false;
    freeMembersFlag:boolean =false;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    selectedFiles: FileList;
    minDate;
    maxDate;
    SelcetedMemberId;
    maxDateforToDate=null;
    minDateforToDate;
    oneDay = 24*60*60*1000;
    diffDays;
    generatedPassword;
    userData;
    dataCount=0;
    existingEmails=[];
    existingMobileNumbers=[];
    selectedConvertion = [];
    freeMemberShipArray = [];
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
        public service: PlanConversionService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        public  exportService: ExportService,
        public datepipe:DatePipe,
        private formBuilder:FormBuilder) {
            this.formErrors = {
                Email: {},
                Duration: {}
            };
}


ngOnInit(){
        document.getElementById('preloader-div').style.display = 'block';
    this.userId=this.sendReceiveService.globalUserId;
    this.filterData={
        filterColumnNames:[
        {"Key":'sno',"Value":" "},
        {"Key":'MemberId',"Value":" "},
        {"Key":'FullName',"Value":" "},
        {"Key":'Email',"Value":" "},
        {"Key":'MobileNumber',"Value":" "},
        {"Key":'SubscriptionDate',"Value":""},
        {"Key":'EndDate',"Value":""}
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
    this.PlanConversionForm = this.formBuilder.group({
        id: 0,
        'Duration' : ['',Validators.compose([Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyNumbers)])],
        'fromDate':[null,Validators.required],
        'toDate':[null],
    });

    this.PlanConversionForm.get('toDate').disable();

}
getAllMembers(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllMembers().subscribe((response) => {
            this.temp=response;
        const configurationData: any = [];
        this.memberFlag = true;
           for (let i = 0; i < response.length; i++) {
                response[i].sno = i + 1;
                configurationData.push(response[i]);
            }
            this.filterData.gridData = configurationData;
            this.dataSource = new MatTableDataSource(configurationData);
            this.filterData.dataSource=this.dataSource;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
               document.getElementById('preloader-div').style.display = 'none';;
    }, (error) => {
          document.getElementById('preloader-div').style.display = 'none';;
        });
}
 getAllNonMembers() {
             this.service.getAllNonMembers().subscribe((response) => {
                 this.temp=response;
                 const memberData: any = [];
                 this.nonmemberFlag = true;
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
            getAllfreeMembershipMembers(){
                this.service.getAllfreeMembershipMembers().subscribe((response) => {
                    this.temp=response;
                    const configurationData: any = [];
                    this.freeMembersFlag=true;
            for (let i = 0; i < response.length; i++) {
                            response[i].sno = i + 1;
                            configurationData.push(response[i]);
                        }
                        this.filterData.gridData = configurationData;
                        this.dataSource = new MatTableDataSource(configurationData);
                        this.filterData.dataSource=this.dataSource;
                        this.dataSource.paginator = this.paginator;
                        this.dataSource.sort = this.sort;
                           document.getElementById('preloader-div').style.display = 'none';;
                }, (error) => {
                       document.getElementById('preloader-div').style.display = 'none';;
                    });
            }

actionAfterError() {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this.router.navigate(['/sessions/signin']);
    });
}

addPlanConversion(){
    this.AddPlanConversionFlag=true;
    this.minDate=null;
}
updatePagination(){
   this.filterData.dataSource=this.filterData.dataSource;
   this.filterData.dataSource.paginator = this.paginator;
  }
onPlanConversionFormSubmit() {
    let StartDate: string = this.PlanConversionForm.value.fromDate;
    let duration: string = this.PlanConversionForm.value.Duration;
    
    if (!this.PlanConversionForm.valid) {
        return;
    }
       document.getElementById('preloader-div').style.display = 'block';
if( this.title="Save"){
    for( var i=0; i<this.selectedConvertion.length;i++){
        var temp = {
            "MemberId":this.selectedConvertion[i],
        "MemberShipDuration":duration,
        'StartDate':new Date(StartDate).toLocaleDateString(),
            'EndDate':this.maxDateforToDate,
        };
        this.freeMemberShipArray.push(temp);
    }
    
        if(this.mode==0){
        this.service.convertMemberToFreeMember({
            "MemberToFreeMemberList":this.freeMemberShipArray
            }).subscribe((data) => {
                if(data!=null){
                    console.log(data);
                     document.getElementById('preloader-div').style.display = 'none';;
                    this.selectedConvertion=[];
                    this.freeMemberShipArray= [];
                    this.PlanConversionForm.reset();
                    this.AddPlanConversionFlag=false;
                    this.maxDateforToDate=null;
                    this.minDateforToDate=null;
                    this.minDate=new Date();
                    this.getAllMembers();
                   
                }
              
    
        },error=>{
            
        });
     }else if(this.mode==1){
        this.service.covertNonMemberToFreeMember({
            "MemberToFreeMemberList":this.freeMemberShipArray
            }).subscribe((data) => {
                if(data!=null){
                    console.log(data);
                     document.getElementById('preloader-div').style.display = 'none';;
                    this.selectedConvertion=[];
                    this.freeMemberShipArray = [];
                    this.PlanConversionForm.reset();
                    this.AddPlanConversionFlag=false;
                    this.maxDateforToDate=null;
                    this.minDateforToDate=null;
                    this.minDate=new Date();
                    this.getAllNonMembers();

                }
              
    
        },error=>{
             document.getElementById('preloader-div').style.display = 'none';;
        });
      
     }
}
     
    
    this.AddPlanConversionFlag=false;

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
            "Duration":parseInt(this.PlanConversionForm.value.Duration)
        };
            this.service.registerInMemberShip(memberShipObj).subscribe((formData)=>{
                this.dataCount++;
                if(this.dataCount<this.userData.length){
                    this.registration(this.userData);
                }else{
                    this.PlanConversionForm.reset();
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
        this.PlanConversionForm.reset();
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
    this.PlanConversionForm.reset();
    this.AddPlanConversionFlag=false;
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
     if(!!this.PlanConversionForm.value.Duration){
        var tempMinDate=new Date(selectedDate);
        this.maxDateforToDate = this.minDateforToDate = new Date(tempMinDate.setDate(tempMinDate.getDate() +  parseInt( this.PlanConversionForm.value.Duration)*30)).toLocaleDateString();
     }

 }
 clkRadioBtn(model){
    this.selectedConvertion =[];
    this.freeMemberShipArray = [];
    this.memberFlag=false;
    this.nonmemberFlag=false;
    this.freeMembersFlag =false;
     this.PlanConversionForm.reset();
    this.AddPlanConversionFlag=false;
    this.maxDateforToDate=null;
    this.minDateforToDate=null;
    this.minDate=new Date();
   if(model == 0){
           this.getAllMembers();
          }
          else if(model == 1){
              this.getAllNonMembers();
          }else if(model==2){
            this.getAllfreeMembershipMembers();
          }
     
 }
 planConvertSelected(id: number,event){
    this.AddPlanConversionFlag=false;
    if( event.checked == true){
        this.selectedConvertion.push(id);
        }else{
            this.selectedConvertion.splice($.inArray(id,this.selectedConvertion), 1);
       }
       console.log(this.selectedConvertion);
       this.SelcetedMemberId = id;
 }
 convertToMem(convertTo){
     if(convertTo == 0){
        console.log("from nonmem to member");
          document.getElementById('preloader-div').style.display = 'block';
        this.service.covertNonMemberToMember(this.selectedConvertion).subscribe(resp => {
            console.log(resp);
             document.getElementById('preloader-div').style.display = 'none';;
             this.selectedConvertion = [];
            this.getAllNonMembers();
     },
      (error) => {
        console.log(error);
       
   });

     }else if(convertTo == 1){
        console.log("from freemem to member");
          document.getElementById('preloader-div').style.display = 'block';
        this.service.covertFreeMemberToMember(this.selectedConvertion).subscribe(resp => {
            console.log(resp);
             document.getElementById('preloader-div').style.display = 'none';;
            this.selectedConvertion=[];
            this.getAllfreeMembershipMembers();
     },
      (error) => {
        console.log(error);
       
   });
     }
    
    
}
convertToNonMem(convertTo){
    if(convertTo == 0){
        console.log("from mem to nonmember");
          document.getElementById('preloader-div').style.display = 'block';
        this.service.covertMemberToNonMember(this.selectedConvertion).subscribe(resp => {
            console.log(resp);
             document.getElementById('preloader-div').style.display = 'none';;
            this.selectedConvertion = [];
            this.getAllMembers();
     },
      (error) => {
        console.log(error);
       
   });
     }else if(convertTo == 1){
        console.log("from freemem to nonmember");
          document.getElementById('preloader-div').style.display = 'block';
        this.service.covertFreeMemberToNonMember(this.selectedConvertion).subscribe(resp => {
            console.log(resp);
             document.getElementById('preloader-div').style.display = 'none';;
            this.selectedConvertion = [];
            this.getAllfreeMembershipMembers();
     },
      (error) => {
        console.log(error);
       
   });
     }
}
convertTofreeMem(convertTo){
   
    this.mode = convertTo;
    if(convertTo == 0){
        this.AddPlanConversionFlag=true;
        console.log("from mem to freemember");
       this.minDate=null;
   
 
     }else if(convertTo == 1){
        this.AddPlanConversionFlag=true;
        console.log("from nonmem to freemember");
      
    this.minDate=null;
    
     }
     
     
  
}
onChangeDuration(selectedDuration){
    if(this.minDate!=null){
       var tempMinDate=new Date(this.PlanConversionForm.value.fromDate); 
       this.maxDateforToDate = this.minDateforToDate = new Date(tempMinDate.setDate(tempMinDate.getDate() + (parseInt(selectedDuration)* 30))).toLocaleDateString();
    }
}


exportToPdf() {   
    if(this.memberFlag==true){

        if(this.temp.length!=0){
            var doc = new jsPDF();
            var rows = [];
            
            var col =['sno','MemberId','MemberName','Email','Mobile','startDate','DueDate'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].Email,this.temp[key].MobileNumber,(this.datepipe.transform(this.temp[key].SubscriptionDate, 'yyyy-MM-dd')),(this.datepipe.transform(this.temp[key].EndDate, 'yyyy-MM-dd'))];
                rows.push(temporary);
            }
            let reportname = "Members.pdf"
            this.exportService.exportAsPdf(col,rows,reportname);
        }

    }
    else if (this.nonmemberFlag==true){
        if(this.temp.length!=0){
            var doc = new jsPDF();
            var rows = [];
            
            var col =['sno','MemberId','MemberName','Email','Mobile','Registered Date'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].Email,this.temp[key].MobileNumber,(this.datepipe.transform(this.temp[key].SubscriptionDate, 'yyyy-MM-dd'))];
                rows.push(temporary);
            }
            let reportname = "NonMembers.pdf"
            this.exportService.exportAsPdf(col,rows,reportname);
        }
    }
    else if (this.freeMembersFlag===true){
        if(this.temp.length!=0){
            var doc = new jsPDF();
            var rows = [];
            
            var col =['sno','MemberId','MemberName','EmailId','Mobile','startDate','DueDate'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].Email,this.temp[key].MobileNumber,(this.datepipe.transform(this.temp[key].SubscriptionDate, 'yyyy-MM-dd')),(this.datepipe.transform(this.temp[key].EndDate, 'yyyy-MM-dd'))];
                rows.push(temporary);
            }
            let reportname = "FreeMembers.pdf"
            this.exportService.exportAsPdf(col,rows,reportname);
        }
    }
   
    else {
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
}
exportToExcel(event) {
    if (this.memberFlag==true){
    if(this.temp.length!=0){
 
        var rows = [];

      
        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].Email,this.temp[key].MobileNumber,(this.datepipe.transform(this.temp[key].SubscriptionDate, 'yyyy-MM-dd')),(this.datepipe.transform(this.temp[key].EndDate, 'yyyy-MM-dd'))];
            rows.push(temporary);
        }
    
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','MemberId','MemberName','Email','Mobile','startDate','DueDate'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.FullName +  "</td><td>" + value.Email +  "</td><td>" + value.MobileNumber +   "</td><td>" +value.SubscriptionDate + "</td><td>" +value.EndDate + "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "Members.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'Members': ws }, SheetNames: ['Members'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    
 
    }
}
else if(this.nonmemberFlag==true){
    if(this.temp.length!=0){
 
        var rows = [];

            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].Email,this.temp[key].MobileNumber,(this.datepipe.transform(this.temp[key].SubscriptionDate, 'yyyy-MM-dd'))];
                rows.push(temporary);
            }
       
    
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','MemberId','MemberName','Email','Mobile','Registered Date'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.FullName +  "</td><td>" + value.Email +  "</td><td>" + value.MobileNumber +   "</td><td>" +value.SubscriptionDate + "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "NonMembers.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'NonMembers': ws }, SheetNames: ['NonMembers'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    
 
    }
}

else if (this.freeMembersFlag==true){
    if(this.temp.length!=0){
 
        var rows = [];

      
        for(var key in this.temp){
            var temporary = [(parseInt(key) +1), this.temp[key].MemberId,this.temp[key].FullName,this.temp[key].Email,this.temp[key].MobileNumber,(this.datepipe.transform(this.temp[key].SubscriptionDate, 'yyyy-MM-dd')),(this.datepipe.transform(this.temp[key].EndDate, 'yyyy-MM-dd'))];
            rows.push(temporary);
        }
    
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','MemberId','MemberName','Email','Mobile','startDate','DueDate'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.MemberId +  "</td><td>" + value.FullName +  "</td><td>" + value.Email +  "</td><td>" + value.MobileNumber +   "</td><td>" +value.SubscriptionDate + "</td><td>" +value.EndDate + "</td></tr>");
  
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "FreeMember.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'FreeMember': ws }, SheetNames: ['FreeMember'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    
 
    }
}
    else{
        this.sendReceiveService.showDialog('There is No Data Available to Export');
    }
  }





}
