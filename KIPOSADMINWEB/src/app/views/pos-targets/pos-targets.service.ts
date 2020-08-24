import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from 'src/app/services/common/myAppHttp.service';


@Injectable()
export class  POSTargetsService { 
    constructor(private sendReceiveService: SendReceiveService) { }

    getAllPOSTargets() {
     return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/POSSalesTarget/AllPOSSalesTarget",{});
    }
    getPOSTargetsListById(ID: number) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/POSSalesTarget/GetPOSSalesTargetById?id="+ID,{});
    }
    savePOSTarget(obj:any){       
        
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/POSSalesTarget/POSSalesTargetPOST", obj);
    }
    updatePOSTarget(obj:any, id:number){               
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/POSSalesTarget/POSSalesTargetPUT?id="+id, obj, MyAppHttp.REQUEST_TYPES.UPDATE);
    }
    deletePOSTarget(id:number){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/POSSalesTarget/POSSalesTargetdelete?id="+id, {}, MyAppHttp.REQUEST_TYPES.DELETE)
     }
    getStoreDetails(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Common/GetAllPickUpPoints",{});    
     }
}
