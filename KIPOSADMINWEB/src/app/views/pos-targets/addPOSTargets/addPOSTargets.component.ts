import { Component, OnInit, ViewEncapsulation, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog, MatDatepicker } from '@angular/material';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';

import { Subject } from 'rxjs';
import { IPageLevelPermissions } from 'src/app/helpers/common.interface';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';

import { environment } from 'src/environments/environment';
import { POSTargetsService } from '../pos-targets.service';
import { DatePipe } from '@angular/common';


@Component({
  selector: 'app-addPOSTargets',
  templateUrl: './addPOSTargets.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AddPOSTargetsComponent implements OnInit {
  submitted: boolean = false;
  appURL = environment.url;
  userId: number = 0;
  posTargetsFormGroup: FormGroup;
  id: number = 0;
  idOnUpdate: number = 0;
  title: string = "Featured Articles";
  saveUser: boolean = false;
  updateUser: boolean = false;
  checkInput: boolean = false;
  minDate;
  maxDate;
  endDate;
  storeList: any;
  updateRespose: any;
  addStoryFormErrors = {

    targetBowlsCount: {},
    targetSalesAmt: {},
    shortDescription: {},
    fullDescription: {},
    File: {}
  };
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  isOnView: boolean = false;


  constructor(private spinner: NgxSpinnerService,
    private router: Router,

    public ref: ChangeDetectorRef,
    public dialog: MatDialog,
    public viewContainerRef: ViewContainerRef,
    public logoutService: LogoutService,
    public appInfoService: AppInfoService,
    public service: POSTargetsService,
    public sendReceiveService: SendReceiveService, private formBuilder: FormBuilder,
    public translate: TranslateService,
    fb: FormBuilder, public datepipe:DatePipe,

    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    this.maxDate = new Date();
    this.userId = this.sendReceiveService.globalUserId;
    this.id = +this.route.snapshot.params['id'];
    if (isNaN(this.id)) {
      this.title = "Add POS Targets"
    }
    if (!!this.id) {
      this.title = "Edit POS Targets - " + this.id;
    }
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
      this.pagePermissions = pageLevelPermissions.response;
      var temproute = this.router.url;
      if (temproute.indexOf('add') > -1) {
        this.posTargetsFormGroup.enable();
        if (!this.pagePermissions.Add)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('update') > -1) {
        this.posTargetsFormGroup.enable();
        if (!this.pagePermissions.Edit)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('view') > -1) {
        this.posTargetsFormGroup.disable();
        this.isOnView = true;
        if (!this.pagePermissions.View)
          this.sendReceiveService.logoutService();
      }
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });

    this.service.getStoreDetails()
      .subscribe((stores) => {
        this.storeList = stores;
      })
    if (!isNaN(this.id)) {
      this.posTargetsFormGroup = this.formBuilder.group({
        'id': 0,
        'targetBowlsCount': [null, Validators.compose([Validators.required])],
        'targetSalesAmt': [null, Validators.compose([Validators.required])],
        'startDate': [null, Validators.required],
        'endDate': [null, Validators.required],
        'store': [null, Validators.required],
      });
      this.idOnUpdate = this.id;
      this.saveUser = false;
      this.updateUser = true;
      this.checkInput = true;
      this.service.getPOSTargetsListById(this.id).subscribe(resp => {
        console.log(JSON.stringify(resp))
        this.updateRespose = resp;
        this.posTargetsFormGroup.patchValue({
          id: resp[0].Id,
          isCanView: resp[0].IsCanview,
          targetBowlsCount: resp[0].TargetBowlsCount,
          targetSalesAmt: resp[0].TargetSalesAmt,
          startDate: resp[0].StartDate,
          endDate: resp[0].EndDate,
          store: resp[0].StoreId
        });
      });
    } else {
      this.posTargetsFormGroup = this.formBuilder.group({
        id: 0,
        'targetBowlsCount': [null, Validators.compose([Validators.required])],
        'targetSalesAmt': [null, Validators.compose([Validators.required])],
        'startDate': [null, Validators.required],
        'endDate': [null, Validators.required],
        'store': [null, Validators.required],
      });
      this.saveUser = true;
      this.updateUser = false;
      this.checkInput = false;
    }

  }
  clkFromDate(picker: MatDatepicker<Date>) {
    picker.open();
  }

  clkToDate(picker: MatDatepicker<Date>) {
    picker.open();
  }
  onChangeToDate(selectedDate) {
    this.maxDate = selectedDate;
  }
  onChangeFromDate(selectedDate) {
    if (this.posTargetsFormGroup.value.fromDate == "") {
      this.posTargetsFormGroup.get('endDate').disable();
    }
    this.endDate = '';
    this.minDate = selectedDate;
  }
  get f() { return this.posTargetsFormGroup.controls; }
  onFeaturedArticlesSubmit() {

    if (!this.posTargetsFormGroup.valid) {
      this.submitted = true;
      return false;
    }
    this.submitted = false;
    let endDate :  string = this.posTargetsFormGroup.value.endDate;
    let transendDate=this.datepipe.transform(endDate,'yyyy/MM/dd');
    let startDate:  string = this.posTargetsFormGroup.value.startDate;
    let transstartDate=this.datepipe.transform(startDate,'yyyy/MM/dd');
    let var_id: number = this.posTargetsFormGroup.value.id;
    let targetBowlsCount: string = this.posTargetsFormGroup.value.targetBowlsCount;
    let targetSalesAmt: string = this.posTargetsFormGroup.value.targetSalesAmt;
   
    let storeid = this.posTargetsFormGroup.value.store;
    if (this.saveUser) {

      this.service.savePOSTarget({
        "targetBowlsCount": targetBowlsCount,
        "targetSalesAmt": targetSalesAmt,
        "createdBy": null,
        "createdDate": null,
        "modifiedBy": null,
        "modifiedDate": null,
        "status": true,
        "storeId": storeid,
        "isCanview": 1,
        "startDate": transstartDate,
        "endDate": transendDate
      }).subscribe((data) => {
        this.router.navigate(['/postargets']);

      },
        error => this.addStoryFormErrors = error
      )
    }
    else if (this.updateUser) {
      this.idOnUpdate = 0;
      this.posTargetsFormGroup = this.formBuilder.group({
        id: 0,
        'targetBowlsCount': [null, Validators.compose([Validators.required])],
        'targetSalesAmt': [null, Validators.compose([Validators.required])],
        'startDate': [null, Validators.required],
        'endDate': [null, Validators.required],
        'store': [null, Validators.required]
      });
      this.service.updatePOSTarget({
        "targetBowlsCount": targetBowlsCount,
        "targetSalesAmt": targetSalesAmt,
        "createdBy": this.updateRespose[0].createdBy,
        "createdDate": this.updateRespose[0].createdDate,
        "modifiedBy": this.updateRespose[0].createdBy,
        "modifiedDate": null,
        "status": this.updateRespose[0].Status,
        "storeId": storeid,
        "isCanview": this.updateRespose[0].isCanview,
        "startDate": transstartDate,
        "endDate": transendDate
      }, var_id)
        .subscribe((data) => {
          this.router.navigate(['/postargets']);
        }, error => {
          this.sendReceiveService.showDialog('An error occured');
        })
    }
  }
  onGoBack() {
    this.idOnUpdate = 0;
    this.router.navigate(['/postargets']);

  }

}
