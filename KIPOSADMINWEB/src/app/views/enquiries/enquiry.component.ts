import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { LogoutService } from '../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import { AppInfoService } from '../../services/common/appInfo.service';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import { EnquiryService } from './enquiry.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { IPageLevelPermissions, EnquiryModel } from '../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';


@Component({
    selector: 'enquiry-table',
    templateUrl: './enquiry.component.html',
    encapsulation: ViewEncapsulation.None
})

export class GetEnquiryComponent implements OnInit {
    displayedColumns = ['sno','EnquiryNum','FirstName','EnquiryDetails','CreatedDate']
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
                public service: EnquiryService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService,  
                private actRoute: ActivatedRoute,
                private activatedRoute: ActivatedRoute,) {
    }



    ngOnInit() {
          document.getElementById('preloader-div').style.display = 'block';
        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'EnquiryNum',"Value":" "},
              {"Key":'FirstName',"Value":" "},
              {"Key":'EnquiryDetails',"Value":" "},
              {"Key":'CreatedDate',"Value":" "},
            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
        this.getAllEnquiry();
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

    getAllEnquiry() {
        document.getElementById('preloader-div').style.display = 'block';
            this.service.getAllEnquiry().subscribe((response) => {
                const enquiryData: any = [];
                for (let i = 0; i < response.length; i++) {
                    response[i].sno = i + 1;
                    enquiryData.push(response[i]);
                }
                this.filterData.gridData = enquiryData;
                this.dataSource = new MatTableDataSource(enquiryData);
                this.filterData.dataSource=this.dataSource;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                 document.getElementById('preloader-div').style.display = 'none';
            }, (error) => {
                 document.getElementById('preloader-div').style.display = 'none';
            });
    }

    actionAfterError () {
        this.dialogRef.afterClosed().subscribe(result => {
            this.appInfoService.setSecurity();
            this.sendReceiveService.setSequence(1);
            this.router.navigate(['/sessions/signin']);
        });
    }
}