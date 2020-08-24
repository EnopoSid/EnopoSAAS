import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class paymentService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllPayments() {
        let inputData = {
        };

        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/PaymentOptions/Get",{});
    }
    getMainMenuById(id: number) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/PaymentOptions/GetById?id="+id,{});
        }
    
        saveMainMenu(MainMenu) {
            return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post ,'api/PaymentOptions/Post', MainMenu,MyAppHttp.REQUEST_TYPES.ADD)   
        }
        updateMainMenu(MainMenu,id) {
            return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/PaymentOptions/Put?id='+id, MainMenu,MyAppHttp.REQUEST_TYPES.UPDATE)
        }
    
        deleteMainMenu(id) {
            return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/PaymentOptions/Delete?id=" + id, {},MyAppHttp.REQUEST_TYPES.DELETE)
        }
        duplicateMenuName(MainMenu){
            return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/PaymentOptions/Duplicate", MainMenu)
        }
    

    
   
}