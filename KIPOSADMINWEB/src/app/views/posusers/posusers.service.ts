import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class PosUsersService {
    HMACKey: string;
    isUserView:boolean;

    constructor(private sendReceiveService: SendReceiveService) { }

    getAllPosUsers() {
        let inputData = {
            
        };

     return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/POSUser/GetPOSUser",{});
    }
    getPosUsersListById(ID: number) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/POSUser/GetPOSUserById?id="+ID,{});
          }
    getAllRegions(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Region/Get',{});
    }

    AddPosUserNOP(User){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'Api/Client/POSRegister', User,null,true)
    }

    savePosUsersList(User){

        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/POSUser/Register", User,MyAppHttp.REQUEST_TYPES.ADD);
    }

    editPosUserInNOP(User){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'Api/Client/InfoEdit', User,null,true);
    }

    editPosUserList(UserList,ID){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/POSUser/EditPOSUser?id="+ID,UserList,MyAppHttp.REQUEST_TYPES.UPDATE);
    }
    roletypeMaster(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/POSUser/GetPOSRolesIsActive",{});   
     }
     getStoreDetails(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Common/GetAllPickUpPoints",{});    
     }
     getZonesByRegionId(regionId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Zone/GetByRegionId?id='+regionId,{})
    }
    deletePOSUserInNOP(CustomerGuid){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'Api/Client/DeletePosUser', CustomerGuid,null,true);
    }
     deletePOSUser(CustomerId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/POSUser/Delete?CustomerId="+ CustomerId,{},MyAppHttp.REQUEST_TYPES.DELETE);
     }
     duplicateEmail(UserList){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/User/Duplicate",UserList)
     }
     UserView(userView : boolean)
     {
        this.isUserView = userView
     }
     getPosUsersByRoleIdOrDeptId(roleId: number,deptId: number) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Users/GetUsersByRoleIdOrDepartmentId?roleId='+roleId+'&deptId='+deptId+'&loggedInRoleId='+ this.sendReceiveService.globalRoleId,{});
    }
}
