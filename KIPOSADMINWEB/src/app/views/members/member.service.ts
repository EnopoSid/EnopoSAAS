import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class MemberService { 
    constructor(private sendReceiveService: SendReceiveService) { }

    getAllCustomers(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/AllCustomers",{});
    }
    getAllMembers() {
     return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/membersDetailsAdmin",{});
    }
    getAllNonMembers() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/nonmembersDetailsAdmin",{});
       }

    getMembersByMemberId(ID: string) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/MemberDetails?id="+ID,{});
    }

    saveConsumerEnquiry(EnquiryList:any,  file: FileList){
        let formdata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
            }
        }
        formdata.append('objArr', JSON.stringify( EnquiryList ))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Consumer/Enquiry/Post", formdata);
    }
    saveEnquiryList(EnquiryList:any,  file: FileList){
        let formdata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
            }
        }
        formdata.append('objArr', JSON.stringify( EnquiryList ))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Enquiry/Post", formdata);
    }
     duplicateEmail(EnquiryList){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/User/Duplicate",EnquiryList)
     }

     downloadFile(FielId)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Enquiry/DownloadFile?id='+FielId,{})
    }

    getRewardPoints()
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/memberRewardPoints',{})
    }


    MemberDetails(MemberId)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/membersDetailsOrders?memberId='+MemberId,{})
    }



}
