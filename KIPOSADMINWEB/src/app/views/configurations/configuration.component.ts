import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { ConfigurationService } from './configuration.service';
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
import { DatePipe } from '@angular/common';
import { ExportService } from 'src/app/services/common/exportToExcel.service';



@Component({
    selector: 'configuration-table',
    templateUrl: './configuration.component.html',
    styleUrls: ['./configuration.component.css']
})
export class ConfigurationComponent implements OnInit {
    displayedColumns = ['sno','KeyName','KeyValue','Actions']
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
    AddConfigurationFlag:boolean=false;
    ConfigurationForm:FormGroup;
    userId: number = 0;
    idOnUpdate: number =0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};

    constructor(private router: Router,
        private spinner: NgxSpinnerService,
        public service: ConfigurationService,
        public ref: ChangeDetectorRef,
        public dialog: MatDialog,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public datepipe:DatePipe,
        public exportService :ExportService,
        public translate: TranslateService,
        private formBuilder:FormBuilder) {
            this.formErrors = {
                key: {},
                Value: {}
            };
}

    
ngOnInit(){
    this.userId=this.sendReceiveService.globalUserId;
    this.filterData={
        filterColumnNames:[
        {"Key":'sno',"Value":" "},
        {"Key":'Key',"Value":" "},
        {"Key":'Value',"Value":" "},
        ],
        gridData:  this.gridData,
        dataSource: this.dataSource,
        paginator:  this.paginator,
        sort:  this.sort
      };
    this.getAllConfigurations();
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: {response: IPageLevelPermissions})=>{
        this.pagePermissions =pageLevelPermissions.response;  
        this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });
    this.ConfigurationForm = this.formBuilder.group({
        id: 0,
        'key':  [null, Validators.compose([Validators.required, Validators.minLength(3),,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicateKeyName.bind(this)],
        'Value': ['', Validators.required]
    });
    
}

updatePagination(){
    this.filterData.dataSource=this.filterData.dataSource;
    this.filterData.dataSource.paginator = this.paginator;
    }
getAllConfigurations(){
    document.getElementById('preloader-div').style.display = 'block';
    this.service.getAllConfigurations().subscribe((response) => {
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

actionAfterError() {
    this.dialogRef.afterClosed().subscribe(result => {
        this.appInfoService.setSecurity();
        this.sendReceiveService.setSequence(1);
        this.router.navigate(['/sessions/signin']);
    });
}

processEditAction(id){
    this.idOnUpdate= id;

    this.service.getConfigurationById(id)
        .subscribe(resp => {
            this.ConfigurationForm.patchValue({
                id: resp.ConfigId,
                key: resp.Key,
                Value: resp.Value,
            });
        },  error => this.formErrors = error);
}

updateConfiguration(id){
        this.AddConfigurationFlag = true;
        
        this.processEditAction(id);
        this.title = "Update";
        this.ConfigurationForm = this.formBuilder.group({
            id:0,
            'key':  [null, Validators.compose([Validators.required,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicateKeyName.bind(this)],
            'Value' : ['', Validators.required]
           });
         
    }
    duplicateKeyName(){        
    const q = new Promise((resolve, reject) => {
        this.service.duplicateConfiguration({
            'ModifiedBy': null,
            'CreatedBy': null,
            'Key':this.ConfigurationForm.controls['key'].value,
            'Value': null,
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

addConfiguration(){
    this.AddConfigurationFlag=true;
    this.title="Save"
}
onConfigurationFormSubmit() {
    let var_id: string = this.ConfigurationForm.value.id;
    let Key: string = this.ConfigurationForm.value.key;
    let Value: string = this.ConfigurationForm.value.Value;
    if (!this.ConfigurationForm.valid) {
        return;
    }
    if (this.title == "Save") { 
        
        this.service.saveConfiguration({
            'Key': Key,
            'Value': Value,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
        }).subscribe((data) => {
            this.getAllConfigurations();
            this.ConfigurationForm.reset();
        }, error =>  error => {
            this.formErrors = error;
        });
        
    }
    else if (this.title == "Update") {
        this.idOnUpdate=0;
        this.service.updateConfiguration({
            "ConfigId":var_id,
            'Key': Key,
            'Value': Value,
            'IsActive': 1,
            'CreatedBy': this.userId,
            'ModifiedBy': this.userId,
        }, var_id)
            .subscribe((data) => {
               this.title = "Save";
               this.ConfigurationForm = this.formBuilder.group({
                id: 0,
                'Key':  [null, Validators.compose([Validators.required,,Validators.pattern(MyAppHttp.HTML_VALIDATION_PATTERNS.onlyAlphabets)]),this.duplicateKeyName.bind(this)],
               'Value' : ['', Validators.required]
            });
               this.getAllConfigurations();
               this.ConfigurationForm.reset();
            }, error => {
                this.formErrors = error;
            });
            
    }
    this.AddConfigurationFlag=false;
}
deleteConfiguration(id) {
    this.appInfoService.confirmationDialog().subscribe(result=>{
        if(!!result){
    this.service.deleteConfiguration(id)
        .subscribe((data) => {
            this.getAllConfigurations();
        }, error => {
            this.formErrors = error
        });
        
    }
});
}

onCancel(){
    this.idOnUpdate=0;
    this.ConfigurationForm.reset();
    this.AddConfigurationFlag=false;
}



exportToPdf() {
  
    if(this.temp.length!=0){
        var doc = new jsPDF();
        var rows = [];
       
          var col = ['sno','KeyName','KeyValue'];
            for(var key in this.temp){
                var temporary = [(parseInt(key) +1), this.temp[key].Key,this.temp[key].Value];
                rows.push(temporary);
            }
            let reportname = "Configuration.pdf"
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
            var temporary = [(parseInt(key) +1), this.temp[key].Key,this.temp[key].Value];
            rows.push(temporary);
        }
        var createXLSLFormatObj = [];
        var xlsHeader =  ['sno','KeyName','KeyValue'];
        createXLSLFormatObj.push(xlsHeader);
     $.each(rows, function(index, value) {
            var innerRowData = [];
           $("#t1").append("<tr id='t1' style='border: 1px solid black;border-collapse: collapse;' ><td>" + value.sno + "</td><td>" + value.Key +  "</td><td>" + value.Value+ "</td></tr>");
  
        
            $.each(value, function(ind, val) {
    
                innerRowData.push(val);
            });
            createXLSLFormatObj.push(innerRowData);
        });
        var filename = "Configuartion.xlsx";
        var  ws = XLSX.utils.aoa_to_sheet(createXLSLFormatObj);
        const workbook: XLSX.WorkBook = { Sheets: { 'Configuartion': ws }, SheetNames: ['Configuartion'] };
        XLSX.writeFile(workbook, filename, { bookType: 'xlsx', type: 'buffer' });
    }
    else{
      this.sendReceiveService.showDialog('There is No Data Available to Export');
  }
  }










}
