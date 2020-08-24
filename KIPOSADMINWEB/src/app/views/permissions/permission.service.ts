import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class PermissionService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllPermissions() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Permission/Get",{});
    }

    getPermissionById(id: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Permission/GetById?id="+id,{});
    }

    
    updatePermission(Permission,id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Permission/Put?id='+id,Permission,MyAppHttp.REQUEST_TYPES.UPDATE);
    }

    deletePermission(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Permission/Delete?id="+id,{},MyAppHttp.REQUEST_TYPES.DELETE);

    }
    duplicatePermission(Permission){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Permission/Duplicate',Permission);
    }
    savePermission(Permission) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Permission/Post',Permission,MyAppHttp.REQUEST_TYPES.ADD);   
    }
}