import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class ConfigurationService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllConfigurations() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Admin/Configuration/Get",{});
    }

    getConfigurationById(id: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Admin/Configuration/GetbyId?id="+id,{});
    }

    saveConfiguration(Configuration) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post , 'api/Admin/Configuration/Post', Configuration,MyAppHttp.REQUEST_TYPES.ADD)   
    }
    updateConfiguration(Configuration,id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Admin/Configuration/Put?id='+id, Configuration,MyAppHttp.REQUEST_TYPES.UPDATE)
    }

    deleteConfiguration(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Admin/Configuration/Delete?id=" + id, {},MyAppHttp.REQUEST_TYPES.DELETE)

    }
    duplicateConfiguration(Configuration){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Admin/Configuration/Duplicate', Configuration)
    }

}