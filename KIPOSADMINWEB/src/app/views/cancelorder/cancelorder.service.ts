import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { Observable } from 'rxjs';

@Injectable()
export class cancelorderService {
    HMACKey: string;
    userData:any;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllRoles() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Role/Get',{});  
    }
    getRoleById(id){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Role/Get?id='+id,{});
    }
    SaveRole(role) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Role/Post',role);   
    }
    UpdateRole(role,id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Role/Put?id='+id,role,MyAppHttp.REQUEST_TYPES.UPDATE);   
    }
    saveRolePermission(userId: number,clonedRoleId: number, createdRoleId: number){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Role/CreateRolePermissions?UserId='+userId+'&RoleId='+clonedRoleId+'&CreatedRoleId='+createdRoleId+'',{},MyAppHttp.REQUEST_TYPES.ADD)
    }
    deleteRole(id){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Role/Delete?Id=" +id,{},MyAppHttp.REQUEST_TYPES.DELETE)
    }
    getPermissions(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Permission/Get',{})
    } 

    updateCancelStatus(data){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/updaterefundstatus', data)
    } 

    deleteRolePermission(id){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/Permissions/Delete?id=" +id,{},MyAppHttp.REQUEST_TYPES.DELETE)
    }
    getRolePermissonsById(id){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/RolePermission/GetRolePermissionbyid?ID='+id,{})
    }
    duplicateRole(role){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Role/Duplicate',role)
    }
    errorHandler(error: Response) {
        return Observable.throw(error);
    }
    getCancelorders(){
        this.userData = JSON.parse(localStorage.getItem('userData'));
         return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/GetAllPOSCancelOrders?storeid='+this.userData.StoreId,{});
    }
    getRefundOrderListItems(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/refundstatus',{})
    }
    
}