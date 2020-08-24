import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ComplaintsService {
 
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService,
        private http: HttpClient,) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }
 
    getConsumerComplaintFormDropdownListItems(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Consumer/Complaint/MasterDataForDropdowns',{})
    }
    getComplaintFormDropdownListItems(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/MasterDataForDropdowns',{})
    }

    getConsumerZonesByRegionId(regionId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Consumer/Zone/GetByRegionId?id='+regionId,{})
    }
    getZonesByRegionId(regionId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Zone/GetByRegionId?id='+regionId,{})
    }

    addConsumerComplaints(complaint:any, file: FileList){
        let formdata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
            }
        }
        formdata.append('objArr', JSON.stringify( complaint ))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Consumer/Complaint/Post',formdata)
    }

    addComplaints(complaint:any, file: FileList){
        let formdata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
            }
        }
        formdata.append('objArr', JSON.stringify( complaint ))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Complaint/Post',formdata)
    }

    getComplaintList(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/Get',{})
    }
    getComplaintsByRoleIdAndDeptId(roleId,deptId, searchComplaintFlag){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/GetComplaintsByRoleAndDeptId?RoleId='+roleId+'&DeptId='+deptId+'&searchComplaintFlag='+searchComplaintFlag+'',{})
    }
    getComplaintStatusList(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/ComplaintStatus/GetComplaintStatusForAssignment',{})
    }

    getComplaintById(id){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/Get?id='+id,{})
    }
    
    getusersfromComplaint(id){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/usersbyId/Get?id='+id,{})
    }
    changeComplaintStatus(complaintTracking){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Complaint/ComplaintAudit',complaintTracking);
    }

    downloadFile(FielId)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/DownloadFile?id='+FielId,{})
    }

    getRolesByHierarchy(roleId, searchComplaintFlag){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/GetRolesByHierarchy?RoleId='+roleId+'&searchComplaintFlag='+ searchComplaintFlag,{})
    }

    getDepartments(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/DepartmentForRole/Get',{})
    }

    getComplaintSummary(complaintId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/ComplaintSummary?ComplaintId='+complaintId,{})
    }

    getComplaintNumber(complaintId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/GetComplaintNumber?ComplaintId='+complaintId,{})
    }
    advancedSearch(advancedSearch)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Complaint/GetComplaintDetailsByAdvancedSearch',advancedSearch)
    }
    getCallDetailsList(advancedSearch){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/CallDetails/getCallDetailsByUser',advancedSearch)
    }
    getAssignedToIdByComplaintId(complaintId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, 'api/Complaint/GetAssignedToIdByComplaintId?complaintId='+complaintId+'&loggedInRoleId='+ this.sendReceiveService.globalRoleId, {});
    }
    getComplaintsByUserIdorRoleId(userId, roleId, deptId, searchComplaintFlag){
        if(searchComplaintFlag == 1){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/GetComplaintsByUserId?UserId='+userId+'&searchComplaintFlag='+searchComplaintFlag+'',{})
    }
    else{
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Complaint/GetComplaintsByRoleAndDeptId?RoleId='+roleId+'&DeptId='+deptId+'&searchComplaintFlag='+searchComplaintFlag+'',{})
    
    }
    }
    getInCallDetails(ExtentionNumber){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/CallDetails/GetById?ExtentionNumber='+ExtentionNumber,{});
    }
    getExtentionNumber(UserId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/CallDetails/GetExtention?userId='+UserId +'&webIp='+ this.sendReceiveService.currentuserip,{});
    }
    updateComplaint(complaint:any, file: FileList,id:number){
        let formdata: FormData = new FormData();
        if(file!=null){
            for(let i =0; i < file.length; i++){
                formdata.append( file[i]['name'],file[i]);
            }
        }
        formdata.append('objArr', JSON.stringify( complaint ))
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Complaint/Put?id='+id+'',formdata)
    }

    appendcountrycode(event: any, phoneNumber){
        if(phoneNumber.length < 4){
            phoneNumber = MyAppHttp.PhoneNumberCountryCode +phoneNumber
        } 
    return phoneNumber;
}
encodePhoneNumber(phoneNumber: string)
{
    let phonenumber = phoneNumber.replace(MyAppHttp.PhoneNumberCountryCode, "");
    phonenumber = phonenumber.replace(/-/g, "");     
    return parseInt(phonenumber);
}
decodePhoenNumber(phoenNumber){
    let newStr = '';
    let i =0;
    for( i=0; i < (Math.floor(phoenNumber.length/3) - 1); i++){
        newStr = newStr+ phoenNumber.substr(i*3, 3) + '-';
     }
     let phonnumbers = MyAppHttp.PhoneNumberCountryCode + newStr+ phoenNumber.substr(i*3)
  return phonnumbers;
}
chagecallcategory(mobilenNumber , category){
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/CallDetails/ChangeCallCategory?mobileNumbe='+mobilenNumber +'&calltype='+category,{});
}
}