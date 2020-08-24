import {Component, OnInit, ViewEncapsulation, ViewChild, ChangeDetectorRef, ViewContainerRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef, MatTableDataSource, MatPaginator, MatSort} from '@angular/material';
import { LogoutService } from '../../../services/logout/logout.service';
import {Router, ActivatedRoute} from '@angular/router';
import { AppInfoService } from '../../../services/common/appInfo.service';
import {SendReceiveService} from '../../../services/common/sendReceive.service';
import { MemberService } from '../../members/member.service';
import MyAppHttp from '../../../services/common/myAppHttp.service';
import { IPageLevelPermissions, EnquiryModel } from '../../../helpers/common.interface';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs/internal/Subject';
import { CloseScrollStrategy } from '@angular/cdk/overlay';


@Component({
    selector: 'memberDetails-table',
    templateUrl: './memberDetails.component.html',
    encapsulation: ViewEncapsulation.None
})

export class GetMemberDetailsComponent implements OnInit {
    displayedColumns =['sno','OrderId','StoreName','PickUpInStore','OrderDate','CouponCode','AppliedKpoints','OrderAmount','GainedKpoints'];

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
    memberinformation: any = {};
MemberOrders:any={};

    dialogRef: MatDialogRef<any>;
    usersFromServiceExists ;
    sample : string;
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    memberId : string;

    constructor(private spinner: NgxSpinnerService,
                private router: Router,
                public service: MemberService,
                public ref: ChangeDetectorRef,
                public dialog: MatDialog,
                public viewContainerRef: ViewContainerRef,
                public logoutService: LogoutService,
                public appInfoService: AppInfoService,
                public sendReceiveService: SendReceiveService,
                public translate: TranslateService,  

                private activatedRoute: ActivatedRoute) {
    }



    ngOnInit() {
      this.activatedRoute.paramMap.subscribe(params => {
        this.memberId = params.get("id")
      })

        this.filterData={
            filterColumnNames:[
              {"Key":'sno',"Value":" "},
              {"Key":'OrderId',"Value":" "},
              {"Key":'StoreName',"Value":" "},
              {"Key":'PickUpInStore',"Value":" "},
              {"Key":'OrderDate',"Value":" "},
              {"Key":'CouponCode',"Value":" "},
              {"Key":'AppliedKpoints',"Value":" "},
              {"Key":'OrderAmount',"Value":" "},
              {"Key":'GainedKpoints',"Value":" "}
        

            ],
            gridData:  this.gridData,
            dataSource: this.dataSource,
            paginator:  this.paginator,
            sort:  this.sort
          };
        this.getAllMembers(this.memberId);
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

    getAllMembers(memberId) {
        document.getElementById('preloader-div').style.display = 'block';
            this.service.MemberDetails(memberId).subscribe((response) => {
                this.memberinformation=response.CustomerDetails;
                var MemberOrders=[];
                   var obj =[];
                    obj=response.CustomOrders;
                for (let i = 0; i < obj.length; i++) {
                    obj[i].sno = i + 1;
                    MemberOrders.push(obj[i]);
                }
                this.filterData.gridData = MemberOrders;
                this.dataSource = new MatTableDataSource(MemberOrders);
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
}