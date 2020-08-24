import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ConsumerService {

    ComplaintNum: string
    constructor(private sendReceiveService: SendReceiveService,
        private http: HttpClient,) { }

    getComplaintFormDropdownListItems(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/MasterDataForDropdowns',{})
    }

    getZonesByRegionId(regionId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Zone/GetByRegionId?id='+regionId,{})
    }
    addComplaints(complaint:any, file: FileList){
        let formdata: FormData = new FormData();
        for(let i =0; i < file.length; i++){
            formdata.append( file[i]['name'],file[i]);
        }
        formdata.append('objArr', JSON.stringify( complaint ))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Complaint/Post',formdata)
    }

    getComplaintList(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/Get',{})
    }
    getComplaintStatusList(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/ComplaintStatus/Get',{})
    }

    getComplaintById(id){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/Get?id='+id,{})
    }

    changeComplaintStatus(checkedComplaintIds: number[], complaintStatusId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Complaint/ChangeComplaintStatus?complaintStatusId='+complaintStatusId+'', checkedComplaintIds)
    }

    downloadFile(FielId)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/DownloadFile?id='+FielId,{})
    }

    getComplaintByComplaintNumber(complaintNum){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Consumer/GetComplaintStatus?complaintNum='+complaintNum,{})
    }

    validateComplaintNumber(complaintNum){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Consumer/ValidateComplaintStatus?complaintNum='+complaintNum,{})
    }

}