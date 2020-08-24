import {Injectable} from '@angular/core';
import {CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router} from '@angular/router';
import {SendReceiveService} from './../common/sendReceive.service';
import MyAppHttp from './../common/myAppHttp.service';

@Injectable()
export class LogoutService {

    HMACKey: string;
    n: string;
    e: string;
    datapack: string;

    constructor(private router: Router, private sendReceiveService: SendReceiveService) {}

    logout() {
        let inputData = {
            modelId: MyAppHttp.REQUESTS.LOGOUT.modelId,
        };
        return this.sendReceiveService.send(inputData, this.HMACKey);
    }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    public setN(input) {
        this.n = input;
    }

    public setE(input) {
        this.e = input;
    }

    public setDataPack(input) {
        this.datapack = input;
    }
}
