import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class deliveryordersService {
    HMACKey: string;
    userData: any;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllPlans() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"Api/Client/GetPickupAddress",{});
    }
    getPickupAddress() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'GetAllStorePickUpPointsAdmin', 
        {});
    }
    
    activateRecord(storeObj,id){
        let apiuRL ="UpdateStorePickUpPoint";
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,apiuRL+"?Id="+id,storeObj);
     }  

     assignordertostore(orderId,storeId,assignedstatus){
        let apiuRL ="AssignStoreToDeliveryOrders";
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,apiuRL+"?orderId="+orderId+"&storeId="+storeId+"&storeAssignedStatus="+assignedstatus,{});
     } 

     getAllActiveStores()
     {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'GetAllIsActiveStorePickUpPoints',{})
     }

     getAllDeliveryOrders()
     {
        this.userData = JSON.parse(localStorage.getItem('userData'));
        if(this.userData.StoreId==null)
        {
            this.userData.StoreId=0;
        }
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'GetAllOnlineDeliveryOrders?storeId='+this.userData.StoreId,{})
     }
     getAllConfigurations() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Admin/Configuration/Get",{});
    }

}