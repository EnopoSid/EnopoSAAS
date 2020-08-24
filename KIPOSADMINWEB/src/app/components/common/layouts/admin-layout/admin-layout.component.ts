import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from "rxjs";
import {filter} from "rxjs/Operators"
import { MatSidenav } from '@angular/material';
import { SendReceiveService } from 'src/app/services/common/sendReceive.service';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../../services/navigation/navigation.service';

import * as domHelper from '../../../../helpers/dom.helper';
import * as $ from 'jquery';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html'
})
export class AdminLayoutComponent implements OnInit {
  private isMobile;
  screenSizeWatcher: Subscription;
  isSidenavOpen: Boolean = false;
  url;
  isMenuNeeded: Boolean = false;
  @ViewChild(MatSidenav, {static: false}) private sideNave: MatSidenav;
  @ViewChild('drawer', {static: false}) drawer;

  constructor(
    private router: Router,
    public translate: TranslateService,
    public navigationService: NavigationService,
    public sendReceive: SendReceiveService,
    private spinner:NgxSpinnerService
  ) {
    router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe((routeChange: NavigationEnd) => {
      this.url = routeChange.url;
      
     this.navigationService.activateMenu();

  
     if(!!this.sendReceive.globalMenuId)
{  
      this.navigationService.getPageLevelPermisisons(this.sendReceive.globalRoleId, this.sendReceive.globalMenuId, this.sendReceive.globalSubmenuId);
}    

    if(!!this.sendReceive.globalRoleId)
    {
    this.navigationService.getAllMenuItems(this.sendReceive.globalRoleId);
    }

      if (this.isNavOver()) {
        this.sideNave.close()
      }
    });

   const browserLang: string = translate.getBrowserLang();
    translate.use(browserLang.match(/en|fr/) ? browserLang : 'en');
  }
  ngOnInit() {    
    this.isMenuNeeded = !!localStorage.getItem('userMenuList');
  }
  isNavOver() {
    return window.matchMedia(`(max-width: 960px)`).matches;
  }
  getMode(): string {
    if (this.isNavOver()) {
      return 'over';
    } else {
      return 'side';
    }
  }
  checkCollapsed() {
    let appBody = document.body;
  }

  clk_DrawerToggle(){
    this.drawer.toggle();
  }
  
}