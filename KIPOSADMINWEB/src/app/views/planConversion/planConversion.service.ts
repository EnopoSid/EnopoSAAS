import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class PlanConversionService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllPlanConversion() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/FreeMemberShip/GetMembers",{});
    }
    getAllMembers() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Convert/GetAllMembers",{});
       }
       getAllNonMembers() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Convert/GetAllNonMembers",{});
       }
       getAllfreeMembershipMembers() {
        let inputData = {
        };
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Convert/GetAllFreeMembers",{});
    }

    covertMemberToNonMember(MemberId) {
       
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Convert/MemToNonMem', MemberId,MyAppHttp.Conerted_REQUEST_TYPES.MemToNonMem)
       }
       convertMemberToFreeMember(MemberData) {
      
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Convert/MemToFreeMem', MemberData,MyAppHttp.Conerted_REQUEST_TYPES.MemToFreeMem)
       }
       covertNonMemberToMember(MemberId) {
    
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Convert/NonMemToMem', MemberId,MyAppHttp.Conerted_REQUEST_TYPES.NonMemToMem)
       }
       covertNonMemberToFreeMember(MemberData) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Convert/NonMemToFreeMem', MemberData,MyAppHttp.Conerted_REQUEST_TYPES.NonMemToFreeMem)
       }
       covertFreeMemberToMember(MemberId) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Convert/FreeMemToMem', MemberId,MyAppHttp.Conerted_REQUEST_TYPES.FreeMemToMem)
       }
       covertFreeMemberToNonMember(MemberId) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Convert/FreeMemToNonMem', MemberId,MyAppHttp.Conerted_REQUEST_TYPES.FreeMemToNonMem)
       }
    savePlanConversion(MemberData) {
        let formdata: FormData = new FormData();
        
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