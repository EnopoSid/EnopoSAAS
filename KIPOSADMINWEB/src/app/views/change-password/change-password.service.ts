import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class ChangePasswordService {
    constructor(private sendReceiveService: SendReceiveService) {}

    changePassword(passwordData) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/ForgotPassword/ChangePasword?OldPassword='+passwordData.oldPassword+'&NewPassword='+passwordData.confirmNewPassword+'&UserId='+passwordData.userId+'',{});
    }
}
