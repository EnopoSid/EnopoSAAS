import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class SubMenuService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllSubMenus() {   
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/SubMenu/Get",{});
    }
    getSubMenuByMenuId(MenuId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/SubMenu/GetByMenuId?id="+MenuId,{});
       
    }
    getSubMenuById(id: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/SubMenu/Get?id="+id,{});
    }

    saveSubMenu(SubMenu) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/SubMenu/Post",SubMenu,MyAppHttp.REQUEST_TYPES.ADD); 
    }
    updateSubMenu(SubMenu,id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/SubMenu/Put?id="+id,SubMenu,MyAppHttp.REQUEST_TYPES.UPDATE);
    }

     deleteSubMenu(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/SubMenu/Delete?id=" + id, {},MyAppHttp.REQUEST_TYPES.DELETE);

    }
    duplicateSubMenu(SubMenu){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/SubMenu/Duplicate",SubMenu);
           
    }

}