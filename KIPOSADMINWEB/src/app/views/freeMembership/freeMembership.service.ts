import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class FreeMembershipService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllfreeMembershipMembers() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/FreeMemberShip/GetMembers",{});
    }
    saveFreeMemberShipMember(MemberData,file: FileList) {
        let formdata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
            }
        }
        formdata.append('objArr', JSON.stringify( MemberData ))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post , 'api/FreeMemberShip/UploadList', formdata)   
    }
    duplicateConfiguration(Configuration){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Admin/Configuration/Duplicate', Configuration)
    }

    registerInNop(registrationdata){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'Api/Client/Register', registrationdata,null,true)
    }

    registerInMemberShip(memberRegistrationData){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/MemberDetails/PostDetails', memberRegistrationData)
    }

    getLatestRecords(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, 'api/FreeMemberShip/GetLatestRecords',{})
    }




}