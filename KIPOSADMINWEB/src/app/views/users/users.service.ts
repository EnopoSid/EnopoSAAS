import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class UsersService {
    HMACKey: string;
    isUserView:boolean;

    constructor(private sendReceiveService: SendReceiveService) { }

    getAllUsers() {
        let inputData = {
            
        };

     return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/User/Get",{});
    }
    getUsersListById(ID: number) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/User/GetById?id="+ID,{});
          }
    getAllRegions(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Region/Get',{});
    }
    saveUsersList(UserList){

        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/User/Post", UserList,MyAppHttp.REQUEST_TYPES.ADD);
    }
    editUserList(UserList,ID){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/User/Put?id="+ID,UserList,MyAppHttp.REQUEST_TYPES.UPDATE);
    }
    roletypeMaster(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Role/Get",{});    
     }
     getDepartments(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Department/Get",{});    
     }
     getZonesByRegionId(regionId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Zone/GetByRegionId?id='+regionId,{})
    }
     deleteUser(ID){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/User/Delete?id="+ ID,{},MyAppHttp.REQUEST_TYPES.DELETE);
     }
     duplicateEmail(UserList){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/User/Duplicate",UserList)
     }
     UserView(userView : boolean)
     {
        this.isUserView = userView
     }
     getStoreDetails(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Common/GetAllPickUpPoints",{});    
     }
     getUsersByRoleIdOrDeptId(roleId: number,deptId: number) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Users/GetUsersByRoleIdOrDepartmentId?roleId='+roleId+'&deptId='+deptId+'&loggedInRoleId='+ this.sendReceiveService.globalRoleId,{});
    }
    appendcountrycode(event: any, phoneNumber){
                phoneNumber = MyAppHttp.PhoneNumberCountryCode +phoneNumber
        return phoneNumber;
    }
    encodePhoneNumber(phoneNumber: string)
    {
        if(!!phoneNumber){
            let phonenumber = phoneNumber.replace(MyAppHttp.COUNTRYCODE, "");
            phonenumber = phonenumber.replace(/-/g, "");     
            return parseInt(phonenumber);
        }
    }
    decodePhoenNumber(phoenNumber){
        if(!!phoenNumber){
            let newStr = '';
            let i =0;
            if(phoenNumber.length==12){
            for( i=0; i <= (Math.floor(phoenNumber.length/3) - 1); i++){
                newStr = newStr+ phoenNumber.substr(i*3, 3) + '-';
             }
             let phonnumbers = newStr;
             return phonnumbers;
             }
             else{
                for( i=0; i <= (Math.floor(phoenNumber.length/4) - 1); i++){
                    newStr = newStr+ phoenNumber.substr(i*3, 3) + '-';
                 } 
                 let phonnumbers = MyAppHttp.PhoneNumberCountryCode + newStr+ phoenNumber.substr(i*3)
                 return phonnumbers;
             }
            //  let phonnumbers = MyAppHttp.PhoneNumberCountryCode + newStr+ phoenNumber.substr(i*3)

        }
    }
}
