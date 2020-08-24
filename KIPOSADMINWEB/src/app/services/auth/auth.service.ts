import {Injectable} from '@angular/core';
import {CanActivate, CanActivateChild, ActivatedRouteSnapshot, RouterStateSnapshot, Router} from '@angular/router';

import MyAppHttp from './../common/myAppHttp.service';
import { SendReceiveService } from '../common/sendReceive.service';

declare var getEncPassword: any;

@Injectable()
export class AuthService implements CanActivate, CanActivateChild {
    public authToken;
    private isAuthenticated = true; // Set this value dynamically
    HMACKey: string;
    n: string;
    e: string;
    datapack: string;
    loggedInUser: string;
    loggedInRole:string;
    loggedInIsPasswordChangedFlag;

    constructor(private router: Router, private sendReceiveService: SendReceiveService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.isAuthenticated) {
            return true;
        }
        this.router.navigate(['/sessions/signin']);
        return false;
    }

    canActivateChild() {
        return true;
    }


    authenticate(username: string, password: string) {
        const inputData = {
            adminEmail: username,
            adminPassword: password
        };

        return this.sendReceiveService.loginService(inputData);

    }

    forgotPassword(userEmail: string) {

        const inputData = {
            modelId: MyAppHttp.REQUESTS.FORGOT_PASSWORD.modelId,
            emailID: userEmail
        };

        return this.sendReceiveService.send(inputData, this.HMACKey);
    }

    public setLoggedInUser(input) {
        this.loggedInUser = input;
    }

    setLoggedInIsPasswordChangedFlag(input){
        this.loggedInIsPasswordChangedFlag=input;
    }

    public setLoggedInRole(input){
        this.loggedInRole=input;
    }

    public getLoggedInRole() {
        return this.loggedInRole;
    } 

    public getLoggedInUser() {
        return this.loggedInUser;
    }

    public getLoggedInIsPasswordChangedFlag() {
        return this.loggedInIsPasswordChangedFlag;
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
