import {Injectable} from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import * as $ from 'jquery';
import { Router } from '@angular/router';

import MyAppHttp from '../common/myAppHttp.service';
import { IPageLevelPermissions } from '../../helpers/common.interface';
import { SendReceiveService } from '../common/sendReceive.service';


interface IMenuItem {
    type: string,       // Possible values: link/dropDown/icon/separator/extLink
    name?: string,      // Used as display text for item and title for separator type
    state?: string,     // Router state
    icon?: string,      // Item icon name
    tooltip?: string,   // Tooltip text
    disabled?: boolean, // If true, item will not be appeared in sidenav.
    sub?: IChildItem[]  // Dropdown items
}

interface IChildItem {
    name: string,       // Display text
    state: string       // Router state
}

@Injectable()
export class NavigationService {
    constructor(private router: Router, private sendReceive: SendReceiveService) {
    }
    pagePermissions:IPageLevelPermissions={View: false, Edit: false, Delete: false,Add: false};
    mainMenuItems:any[];
    subMenuItems: any[];

    localMenuItems = JSON.parse(localStorage.getItem('userMenuList'));


    sampleMenuItems = new BehaviorSubject(this.localMenuItems);
    menuItems$ = this.sampleMenuItems.asObservable();


    activateMenu(){
        setTimeout(() => {
          $('.menu-active').removeClass('menu-active');
          $('.submenu-active').removeClass('submenu-active');
          var tempMenuUrl = this.router.url.substring(1, this.router.url.length).split('/')[0];
    
          if(MyAppHttp.MENUS_WITHOUT_SUBMENUS.indexOf(tempMenuUrl) > -1){
    
          }
          else{
              if(MyAppHttp.MASTER_MENU_SUBMENUS.indexOf(tempMenuUrl)>-1){
                $('#menu_master').addClass('menu-active');
              }
              else if(MyAppHttp.MEMBER_MENU_SUBMENUS.indexOf(tempMenuUrl)>-1){
                $('#menu_members').addClass('menu-active');
              }
              else if(MyAppHttp.ORDERS_MENU_SUBMENUS.indexOf(tempMenuUrl)>-1){
                $('#menu_orders').addClass('menu-active');
              }
              else if(MyAppHttp.CMS_SUBMENU_NAME.indexOf(tempMenuUrl)>-1){
                $('#menu_anonymousemail').addClass('menu-active');
              }
              else if(MyAppHttp.Delivey_SUB_MENU_Name.indexOf(tempMenuUrl)>-1){
                $('#menu_delivery').addClass('menu-active');
              }
              else if(MyAppHttp.DASHBOARD_SUBMENU_NAMES.indexOf(tempMenuUrl)>-1){
                $('#menu_dashboard').addClass('menu-active');
              }
              else if(MyAppHttp.REPORT_SUBMENU_NAME.indexOf(tempMenuUrl)>-1){
                $('#menu_orderreport').addClass('menu-active');
              }
              else if(MyAppHttp.POSTargets_SUBMENU_NAME.indexOf(tempMenuUrl)>-1){
                $('#menu_posreport').addClass('menu-active');
              }
              else if(tempMenuUrl==MyAppHttp.ViewProfileRouteUrl){

              }else if(tempMenuUrl==MyAppHttp.ChangepasswordRouteUrl){
              }
              else
              {
                $('#menu_reports').addClass('menu-active');
              }
            }
          $('#menu_'+ tempMenuUrl).addClass('menu-active');
         
            $('#submenu_'+ tempMenuUrl).addClass('submenu-active');
        }, 10);
       
      }

      getPageLevelPermisisons(roleId, menuId, subMenuId){
         if(subMenuId != null) {
            this.sendReceive.send(this.sendReceive.httpVerb.Get ,'api/Login/PageLevelPermissions?roleId='+this.sendReceive.globalRoleId+'&menuId='+menuId+'&subMenuId='+subMenuId+'',{}).subscribe((response) =>{
              var rolePermissionCount = [];
              $.each( response, function( key, value ) {
                 if(!!value)
                 rolePermissionCount.push(key);
               });  
               if(rolePermissionCount.length == 0){
                this.sendReceive.logoutService();
          }else{
            if(!this.sendReceive.globalPageLevelPermission.closed){
              this.sendReceive.globalPageLevelPermission.next(
              {
                response
              });
            }
          }
            });
        this.sendReceive.globalMenuId = menuId;
        this.sendReceive.globalSubmenuId =subMenuId;
       }
      }

      getAllMenuItems(roleId){
            this.sendReceive.send(this.sendReceive.httpVerb.Get ,'api/Login/GetMenusByRoleId?roleId='+roleId+'',{}).subscribe((response) =>{
                this.sendReceive.findMenusAndSubMenus(response);
              });
      }
    
}
