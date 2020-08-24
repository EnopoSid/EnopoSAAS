import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class ForgotPasswordUserListService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllChangePasswordRequestUsers() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/ForgotPassword/GetRequestList",{});
    }

    sendMail(userList){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "api/ForgotPassword/Post",userList);
    }
}