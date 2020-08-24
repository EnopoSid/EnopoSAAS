import { Component, OnInit, ViewEncapsulation, ChangeDetectorRef, ViewContainerRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material';
import { LogoutService } from 'src/app/services/logout/logout.service';
import { AppInfoService } from 'src/app/services/common/appInfo.service';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';

import { Subject } from 'rxjs';
import { IPageLevelPermissions } from 'src/app/helpers/common.interface';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { environment } from 'src/environments/environment';
import { NewsLetterComponentService } from '../news-letter.service';

@Component({
  selector: 'app-addNewsLetter',
  templateUrl: './addNewsLetter.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AddNewsLetterComponent implements OnInit {
  updateText: any;
  submitted: boolean = false;
  appURL = environment.url;
  editImage;
  userId: number = 0;
  selectedFiles: FileList;
  NewsLetter: FormGroup;
  id: number = 0;
  idOnUpdate: number = 0;
  pattern = "[a-zA-Z][a-zA-Z ]*";
  newsLetterTitle: string = "News Letter";
  saveUser: boolean = false;
  updateUser: boolean = false;
  checkInput: boolean = false;
  addNewsLetterFormErrors = {
    titleName: {},
    fullDescription: {},
  };
  pagePermissions: IPageLevelPermissions = { View: false, Edit: false, Delete: false, Add: false };
  isOnView: boolean = false;


  fullDescriptionEditor: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: '7rem',
    minHeight: '5rem',
    placeholder: 'Enter News Letter',
    translate: 'no',
    defaultFontName: 'Arial',
    customClasses: [
      {
        name: "quote",
        class: "quote",
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: "titleText",
        class: "titleText",
        tag: "h1",
      },
    ]
  };
  constructor(
    private spinner: NgxSpinnerService,
    private router: Router,
    public ref: ChangeDetectorRef,
    public dialog: MatDialog,
    public viewContainerRef: ViewContainerRef,
    public logoutService: LogoutService,
    public appInfoService: AppInfoService,
    public service: NewsLetterComponentService,
    public sendReceiveService: SendReceiveService, private formBuilder: FormBuilder,
    public translate: TranslateService,
    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    this.userId = this.sendReceiveService.globalUserId;
    this.id = +this.route.snapshot.params['id'];
    if (isNaN(this.id)) {
      this.newsLetterTitle = "Add News Letter"
    }
    if (!!this.id) {
      this.sendReceiveService.getKeyValueName(this.id).subscribe(resp => {
        if (!!this.service.isOurStoryView) {
          this.newsLetterTitle = "View News Letter - " + this.id;
        }
        else {
          this.newsLetterTitle = "Edit News Letter - " + this.id;
        }
      })
    }
    this.sendReceiveService.globalPageLevelPermission = new Subject;
    this.sendReceiveService.globalPageLevelPermission.subscribe((pageLevelPermissions: { response: IPageLevelPermissions }) => {
      this.pagePermissions = pageLevelPermissions.response;
      var temproute = this.router.url;
      if (temproute.indexOf('add') > -1) {
        this.NewsLetter.enable();
        if (!this.pagePermissions.Add)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('update') > -1) {
        this.NewsLetter.enable();
        if (!this.pagePermissions.Edit)
          this.sendReceiveService.logoutService();
      }

      if (temproute.indexOf('view') > -1) {
        this.NewsLetter.disable();
        this.isOnView = true;
        if (!this.pagePermissions.View)
          this.sendReceiveService.logoutService();
      }
      this.sendReceiveService.globalPageLevelPermission.unsubscribe();
    });


    if (!isNaN(this.id)) {
      this.NewsLetter = this.formBuilder.group({
        'id': 0,
        'titleName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'fullDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
      });
      this.idOnUpdate = this.id;
      this.saveUser = false;
      this.updateUser = true;
      this.checkInput = true;
      this.service.getNewsLetterListById(this.id).subscribe(resp => {        
        this.NewsLetter.patchValue({
          fullDescription: resp.NewsLetter,
          titleName: resp.title         
        });

      });
    } else {
      this.NewsLetter = this.formBuilder.group({
        'titleName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'fullDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
      });
      this.saveUser = true;
      this.updateUser = false;
      this.checkInput = false;
    }
    if (this.service.isOurStoryView) {
      this.NewsLetter.disable();
      this.isOnView = true;
    }
    else {
      this.isOnView = false;
    }

  }
  get f() { return this.NewsLetter.controls; }
  onNewsLetterSubmit() {  
    this.submitted = true;
    let var_titleName: string = this.NewsLetter.value.titleName;
    let var_fullDescription: string = this.NewsLetter.value.fullDescription;
    let var_id: number = this.id;

    if (this.saveUser) {
      if (!this.NewsLetter.valid) {
        return;
      }
      this.service.saveNewsLetterList({
        'CreatedDate': new Date(),
        'ModifiedDate': new Date(),
        "ModifiedBy": this.userId,
        "CreatedBy": this.userId,
        "LastsendDate": null,
        "IsActive": true,
        "title":var_titleName,
        "NewsLetter": var_fullDescription,
      }).subscribe((data) => {
        this.router.navigate(['/landingpage/newsLetter']);
      },
        error => this.addNewsLetterFormErrors = error
      )
    }
    else if (this.updateUser) {
      this.idOnUpdate = 0;
      this.NewsLetter = this.formBuilder.group({
        id: 0,
        'titleName': [null, Validators.compose([Validators.required, Validators.minLength(3), Validators.pattern(this.pattern)])],
        'fullDescription': [null, Validators.compose([Validators.required, Validators.minLength(3)])],
      });
      this.service.editNewsLetterList({
        'CreatedDate': new Date(),
        'ModifiedDate': new Date(),
        "ModifiedBy": this.userId,
        "CreatedBy": this.userId,
        "LastsendDate": null,
        "IsActive": true,
        "title":var_titleName,
        "NewsLetter": var_fullDescription,
      }, var_id)
        .subscribe((data) => {
          this.editImage = "";
          this.router.navigate(['/landingpage/newsLetter']);
        }, error => {
          this.sendReceiveService.showDialog('An error occured');
        })
    }
  }
  onGoBack() {
    this.editImage = "";
    this.idOnUpdate = 0;
    this.router.navigate(['/landingpage/newsLetter']);
  }

}
