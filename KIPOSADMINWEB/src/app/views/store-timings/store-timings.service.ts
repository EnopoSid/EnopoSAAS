import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class StoreTimingsService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllConfigurations() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/GetStoreTimings",{});
    }

    getConfigurationById(id: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/GetStoreTimingsById?id="+id,{});
    }

    saveConfiguration(Configuration) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post , 'api/InsertStoreTimings', Configuration,MyAppHttp.REQUEST_TYPES.ADD)   
    }
    updateConfiguration(Configuration,id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/UpdateStoreTimings?id='+id, Configuration,MyAppHttp.REQUEST_TYPES.UPDATE)
    }

    deleteConfiguration(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Admin/Configuration/Delete?id=" + id, {},MyAppHttp.REQUEST_TYPES.DELETE)

    }
    duplicateConfiguration(Configuration){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Admin/Configuration/Duplicate', Configuration)
    }
    ActivateStatus(timings,id){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/UpdateIsActiveStatus?Id='+id, timings,MyAppHttp.REQUEST_TYPES.UPDATE)
    }
    
    
}