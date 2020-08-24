import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class MenuService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllMenus() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Menu/Get",{});
    }

    getMainMenuById(id: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Menu/Get?id="+id,{});
    }

    saveMainMenu(MainMenu) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post ,'api/Menu/Post', MainMenu,MyAppHttp.REQUEST_TYPES.ADD)   
    }
    updateMainMenu(MainMenu,id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Menu/Put?id='+id, MainMenu,MyAppHttp.REQUEST_TYPES.UPDATE)
    }

    deleteMainMenu(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Menu/Delete?id=" + id, {},MyAppHttp.REQUEST_TYPES.DELETE)
    }
    duplicateMenuName(MainMenu){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Menu/Duplicate", MainMenu)
    }

}