import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class EnquiryService { 
    constructor(private sendReceiveService: SendReceiveService) { }

    getAllEnquiry() {
     return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Enquiry/Get",{});
    }
    getEnquiryListById(ID: number) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Enquiry/Get?id="+ID,{});
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
}
