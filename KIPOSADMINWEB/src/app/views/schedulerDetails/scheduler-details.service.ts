import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { Observable } from 'rxjs';

@Injectable()
export class SchedulerDetailsService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getschedulerDetails() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/SchedulerDetails/GetDetails',{});  
    }
    errorHandler(error: Response) {
        return Observable.throw(error);
    }
}