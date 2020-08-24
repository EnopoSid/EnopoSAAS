import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class POSOrderService { 
    userData:any;
    constructor(private sendReceiveService: SendReceiveService) { }

    getAllPOSorders() {
        this.userData = JSON.parse(localStorage.getItem('userData'));
     return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, 'api/posorders/GetAllPOSOrders?storeid='+this.userData.StoreId,{});
    }

    getOrderListItems(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/orderstatus',{})
    }
    updateCancleStatus(data){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/updateorderstatus', data)
    }
}
