import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { NavigationService } from "../../../services/navigation/navigation.service";
import { environment } from '../../../../environments/environment';
import { Router} from '@angular/router';
import { SendReceiveService } from '../../../services/common/sendReceive.service';
import MyAppHttp from '../../../services/common/myAppHttp.service';
import { AuthService } from '../../../services/auth/auth.service';
import { NgxSpinnerService } from 'ngx-spinner';
import * as $ from 'jquery';


@Component({
  selector: 'navigation',
  templateUrl: './navigation.component.html'
})
export class NavigationComponent {
  menuItems:any[];
  appVersion: string;
  subMenuItems: any[];
  masterMenuUrl: string;
  @Output() outputDrawerToggle = new EventEmitter();
  constructor(
    private spinner: NgxSpinnerService, 
    private auth:AuthService,
    private navService: NavigationService,
    private router: Router,
    private sendReceive: SendReceiveService
  ) {}
  ngOnInit() {
    this.sendReceive.backflag1 =  false;
    this.appVersion = environment.appVersion;
    this.navService.menuItems$.subscribe(menuItem => {
      this.menuItems = menuItem;
    });
    this.sendReceive.globalIsPasswordChangedFlag=this.auth.getLoggedInIsPasswordChangedFlag();

    if(!!this.sendReceive.globalRoleId)
    {
    this.getAllMenuItems(this.sendReceive.globalRoleId);
    }
      
    this.subMenuItems = []; 
    this.sendReceive.globalMenuList.subscribe((data: {Menus: any}) =>{
      if(this.router.url.indexOf('sessions') == -1){
       
        if(MyAppHttp.ANONYMOUS_ROUTES.indexOf(this.router.url) > -1){
          return false;
        }
      this.menuItems = data.Menus;
      var tempRoute = this.router.url.split('/')[1];
      if(tempRoute == MyAppHttp.ViewProfileRouteUrl){

      }
     else if(tempRoute == MyAppHttp.ChangepasswordRouteUrl){

      }
     else if( MyAppHttp.MENUS_WITHOUT_SUBMENUS.indexOf(tempRoute) > -1){
        var tempMenuId =  !!this.menuItems.find(x=>x.MenuURL == tempRoute) ? this.menuItems.find(x=>x.MenuURL == tempRoute).MenuId : 0;
        this.navService.getPageLevelPermisisons(this.sendReceive.globalRoleId, tempMenuId, 0)
        this.sendReceive.globalMenuId = tempMenuId;
        this.sendReceive.globalSubmenuId = 0;
    }
      else{
        this.subMenuItems = [];
        var masterMenuUrl = '';
        if(MyAppHttp.MASTER_MENU_SUBMENUS.indexOf(tempRoute)>-1){
          masterMenuUrl=MyAppHttp.MASTER_MENU_NAME;
        }else if(MyAppHttp.DASHBOARD_SUBMENU_NAMES.indexOf(tempRoute)>-1){
          masterMenuUrl= MyAppHttp.DASHBOARD_MENU_NAME;
        }else if(MyAppHttp.MEMBER_MENU_SUBMENUS.indexOf(tempRoute)>-1){
          masterMenuUrl= MyAppHttp.MEMBER_MENU_NAME;
        }
        else if(MyAppHttp.ORDERS_MENU_SUBMENUS.indexOf(tempRoute) >-1){
          masterMenuUrl = MyAppHttp.ORDERS_MENU_NAME        }
          else if(MyAppHttp.CMS_SUBMENU_NAME.indexOf(tempRoute) >-1){
            masterMenuUrl = MyAppHttp.CMS_MENU_NAME        }
        else if(MyAppHttp.REPORT_SUBMENU_NAME.indexOf(tempRoute)>-1){
          masterMenuUrl = MyAppHttp.REPORT_MENU_NAME;
        } 
        else if(MyAppHttp.Delivey_SUB_MENU_Name.indexOf(tempRoute)>-1){
          masterMenuUrl = MyAppHttp.DELIVERY_MENU_NAME;
        } 
        else if(MyAppHttp.PosTargets_Menu_Name.indexOf(tempRoute)>-1){
          masterMenuUrl = MyAppHttp.PosTargets_Menu_Name;
        }     
        this.masterMenuUrl= masterMenuUrl;
        this.subMenuItems =  !!this.menuItems.find(x=>x.MenuURL == masterMenuUrl) ?  
        this.menuItems.find(x=>x.MenuURL == masterMenuUrl).subMenus : [];
        for(let i = 0;i< this.subMenuItems.length; i++ ){
          if(this.subMenuItems[i].SubMenuName=="SubMenu")
          {
            this.subMenuItems[i].tooltip="Click on Menu for Submenus";
          }
          else if(this.subMenuItems[i].SubMenuName=="Role Permission")
          {
            this.subMenuItems[i].tooltip="Click on Role for Role Permissions";
          }
          else if(this.subMenuItems[i].SubMenuName==MyAppHttp.OrderDetailsRoute)  
          {
            this.subMenuItems[i].tooltip="Click on report orderID for Order Details";
          }
        }
        tempMenuId  = 0;
        var tempSubmenuId =0;      
        if(!!this.menuItems.find(x=>x.MenuURL == masterMenuUrl)){
          tempMenuId =  this.menuItems.find(x=>x.MenuURL == masterMenuUrl).MenuId;
          if(!!this.menuItems.find(x=>x.MenuURL == masterMenuUrl).subMenus.find(y=>y.SubMenuUrl == tempRoute)){
            tempSubmenuId = this.menuItems.find(x=>x.MenuURL == masterMenuUrl).subMenus.find(y=>y.SubMenuUrl == tempRoute).SubMenuId;
          }
          else{
            this.sendReceive.logoutService();
          }
        }
        this.navService.getPageLevelPermisisons(this.sendReceive.globalRoleId, tempMenuId, tempSubmenuId);

      }
      this.navService.activateMenu();
    }
    });

  }

  onMenuClick(menuItem){
    this.sendReceive.backflag1 =  false;
    if(this.sendReceive.globalIsPasswordChangedFlag){
      return false;
    }else{
      this.subMenuItems = [];
      if(!menuItem.subMenus.length){       
        this.navService.getPageLevelPermisisons(menuItem.RoleTypeId,menuItem.MenuId, null );
        this.router.navigate(['/' + menuItem.MenuURL]);
        this.outputDrawerToggle.emit();
      }
      else{
        this.subMenuItems = menuItem.subMenus;
        this.onSubmenuClick (this.subMenuItems[0]);
      }
    }
   
  }

  onSubmenuClick(submenuItem){
   this.sendReceive.backflag1 =  false;
    this.router.navigate(['/'+submenuItem.SubMenuUrl+'']);
    this.navService.getPageLevelPermisisons(submenuItem.RoleTypeId,submenuItem.MenuId, submenuItem.SubMenuId );
    this.outputDrawerToggle.emit();
  }

  

  getAllMenuItems(roleId){
      this.navService.getAllMenuItems(roleId);
  }
  

}
