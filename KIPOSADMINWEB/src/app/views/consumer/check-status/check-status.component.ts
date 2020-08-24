import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import {LogoutService} from '../../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import {AppInfoService} from '../../../services/common/appInfo.service';
import {SendReceiveService} from '../../../services/common/sendReceive.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import MyAppHttp from '../../../services/common/myAppHttp.service';
import { IPageLevelPermissions, ComplaintModel } from '../../../helpers/common.interface';
import { ConsumerService } from '../consumer.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'checkstatus-table',
    templateUrl: './check-status.component.html',
     styleUrls: ['./check-status.component.css'],
     encapsulation: ViewEncapsulation.None
})
export class CheckStatusComponent implements OnInit {
    displayedColumns = ['ComplaintNumber','UserName','ComplaintType','ComplaintStatus','Serviceprovider']
    dataSource: MatTableDataSource<ComplaintModel>;
    gridData =[];
    @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
    @ViewChild(MatSort, {static: false}) sort: MatSort;
    messageFlag1:boolean=false;
    messageFlag2:boolean=false;
    filterData;
    checkstatusId=0;
    cloneselected: boolean;
    updatedata: boolean;
    formErrors: any;
    status: boolean;
    title: string;
    rows = [];
    columns = [];
    temp = [];
    checkstatusForm:FormGroup;
    userId: number = this.sendReceiveService.globalUserId;
    idOnUpdate:number=0;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    AddComplaintFlag:boolean=false;
    checkedComplaints: number[] = [];
    allComplaintStatuses = []; 
    complaintFlag:boolean=true;

    constructor(
        private spinner: NgxSpinnerService,
        public service: ConsumerService,
        public viewContainerRef: ViewContainerRef,
        public logoutService: LogoutService,
        public appInfoService: AppInfoService,
        public sendReceiveService: SendReceiveService,
        public translate: TranslateService,
        private _router: Router,
        private route:ActivatedRoute,
        private formBuilder:FormBuilder
    ) {
            this.formErrors = {
                complaintNumber:{}
            };
        }
    
    ngOnInit(){
        this.filterData={  
            filterColumnNames:[
              {"Key":'ComplaintNum',"Value":" "},
              {"Key":'UserName',"Value":" "},
              {"Key":'ComplaintTypeName',"Value":" "},
              {"Key":'ComplaintStatusName',"Value":" "},
              {"Key":'ServiceProviderName',"Value":" "},
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
        this.checkstatusForm = this.formBuilder.group({
            'id': 0,
            'complaintNumber':  ['', Validators.compose([Validators.required])],
          }); 
          this.getAllComplaints(this.complaintFlag);
        }
          getAllComplaints(complaintFlag){
            document.getElementById('preloader-div').style.display = 'block';
            if(!complaintFlag){
                this.service.ComplaintNum =   this.checkstatusForm.controls['complaintNumber'].value;
                if(this.checkstatusForm.invalid){
                    return this.complaintFlag=false;
                }
            }
            let complaintnumber = this.service.ComplaintNum;
            this.checkstatusForm.patchValue({
                complaintNumber : complaintnumber,
            });
            this.checkstatusForm.value.complaintNumber = complaintnumber;
                if (this.checkstatusForm.touched == true) {
                    complaintnumber = this.checkstatusForm.value.complaintNumber
                }
                if(complaintnumber!=null){
                    this.service.getComplaintByComplaintNumber(complaintnumber).subscribe((response) => {
                        const complaintStatusData: any = [];
                        for (let i = 0; i < response.length; i++) {
                            complaintStatusData.push(response[i]);
                        }
                            this.filterData.gridData = complaintStatusData;
                            this.dataSource = new MatTableDataSource(complaintStatusData);
                            this.filterData.dataSource=this.dataSource;
                            this.dataSource.paginator = this.paginator;
                            this.dataSource.sort = this.sort;
                              document.getElementById('preloader-div').style.display = 'none';
                    }, (error) => {
                          document.getElementById('preloader-div').style.display = 'none';
                    });
                }
                else{
                    this._router.navigate(['../../'], { relativeTo: this.route });
                }
            
        }
        onGoBack(){
            this._router.navigate(['../../'], { relativeTo: this.route });
        }
   
             
    }
