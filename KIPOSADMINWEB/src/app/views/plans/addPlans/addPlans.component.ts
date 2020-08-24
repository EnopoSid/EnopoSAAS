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

import { DatePipe } from '@angular/common';
import { plansService } from '../plans.service';


@Component({
  selector: 'app-addPlans',
  templateUrl: './addPlans.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AddPlans implements OnInit {
  submitted: boolean = false;
  appURL = environment.url;
  userId: number = 0;
  posPlanFormGroup: FormGroup;
  id: number = 0;
  idOnUpdate: number = 0;
  title: string = "Featured Articles";
  saveUser: boolean = false;
  updateUser: boolean = false;
  checkInput: boolean = false;
  minDate;
  maxDate;
  endDate;

  updateRespose: any;
  addStoryFormErrors = {
    plans: {},
    targetBowlsCount: {},
    targetSalesAmt: {},

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
    public service: plansService,
    public sendReceiveService: SendReceiveService, private formBuilder: FormBuilder,
    public translate: TranslateService,
    fb: FormBuilder, public datepipe: DatePipe,

    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    this.maxDate = new Date();
    this.userId = this.sendReceiveService.globalUserId;
    this.id = +this.route.snapshot.params['id'];
    if (isNaN(this.id)) {
      this.title = "Add Plans"
    }
    if (!!this.id) {
      this.title = "Edit Plans - " + this.id;
    }
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
      this.pagePermissions = pageLevelPermissions.response;
      var temproute = this.router.url;
      if (temproute.indexOf('add') > -1) {
        this.posPlanFormGroup.enable();
        if (!this.pagePermissions.Add)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('update') > -1) {
        this.posPlanFormGroup.enable();
        if (!this.pagePermissions.Edit)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('view') > -1) {
        this.posPlanFormGroup.disable();
        this.isOnView = true;
        if (!this.pagePermissions.View)
          this.sendReceiveService.logoutService();
      }
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });


    if (!isNaN(this.id)) {
      this.posPlanFormGroup = this.formBuilder.group({
        'id': 0,
        'targetBowlsCount': [null, Validators.compose([Validators.required])],
        'targetSalesAmt': [null, Validators.compose([Validators.required])],

        'plans': [null, Validators.required],
      });
      this.idOnUpdate = this.id;
      this.saveUser = false;
      this.updateUser = true;
      this.checkInput = true;
      this.service.getPlansById(this.id).subscribe(resp => {
        console.log(JSON.stringify(resp))
        this.updateRespose = resp;
        this.posPlanFormGroup.patchValue({
          targetBowlsCount: resp.SubscriptionAmt,
          targetSalesAmt: resp.DiscountPercentage,

          plans: resp.PlanName
        });
      });
    } else {
      this.posPlanFormGroup = this.formBuilder.group({
        id: 0,
        'targetBowlsCount': [null, Validators.compose([Validators.required])],
        'targetSalesAmt': [null, Validators.compose([Validators.required])],

        'plans': [null, Validators.required],
      });
      this.saveUser = true;
      this.updateUser = false;
      this.checkInput = false;
    }

  }

  get f() { return this.posPlanFormGroup.controls; }
  onFeaturedArticlesSubmit() {

    if (!this.posPlanFormGroup.valid) {
      this.submitted = true;
      return false;
    }
    this.submitted = false;

    let var_id: number = this.posPlanFormGroup.value.id;
    let subscriptionAmt: string = this.posPlanFormGroup.value.targetBowlsCount;
    let discountPercentage: string = this.posPlanFormGroup.value.targetSalesAmt;

    let planName = this.posPlanFormGroup.value.plans;
    if (this.saveUser) {

      this.service.savePlans({
        "PlanId": 0,
        "PlanName": planName,
        "SubscriptionAmt": subscriptionAmt,
        "StoreCredits": 0,
        "CreatedDate": new Date(),
        "ModifiedDate": null,
        'CreatedBy': this.userId,
        'ModifiedBy': this.userId,
        "IsActive": true,
        "DiscountPercentage": discountPercentage,
        "MembershipFee": 0,
        "PlanDuration": 0
      }).subscribe((data) => {
        this.router.navigate(['/plans']);

      },
        error => this.addStoryFormErrors = error
      )
    }
    else if (this.updateUser) {
      this.idOnUpdate = 0;
      this.posPlanFormGroup = this.formBuilder.group({
        id: 0,
        'targetBowlsCount': [null, Validators.compose([Validators.required])],
        'targetSalesAmt': [null, Validators.compose([Validators.required])],

        'plans': [null, Validators.required]
      });
      this.service.updatePlans({
        "PlanId": this.id,
        "PlanName": planName,
        "SubscriptionAmt": subscriptionAmt,
        "StoreCredits": 0,
        "CreatedDate": this.updateRespose.CreatedDate,
        "ModifiedDate": new Date(),
        'CreatedBy': this.userId,
        'ModifiedBy': this.userId,
        "IsActive": true,
        "DiscountPercentage": discountPercentage,
        "MembershipFee": 0,
        "PlanDuration": 0
      }, this.id)
        .subscribe((data) => {
          this.router.navigate(['/plans']);
        }, error => {
          this.sendReceiveService.showDialog('An error occured');
        })
    }
  }
  onGoBack() {
    this.idOnUpdate = 0;
    this.router.navigate(['/plans']);

  }

}
