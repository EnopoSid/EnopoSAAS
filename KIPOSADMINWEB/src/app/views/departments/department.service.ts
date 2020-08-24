import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../services/common/sendReceive.service';
import MyAppHttp from '../../services/common/myAppHttp.service';

@Injectable()
export class DepartmentService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    getAllDepartments() {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get, "api/Department/Get",{});
    }

    getDepartmentById(id: number) {
    return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/Department/Get?id="+id,{});
    }

    saveDepartment(Department) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post ,'api/Department/Post', Department,MyAppHttp.REQUEST_TYPES.ADD)   
    }
    updateDepartment(Department,id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Department/Put?id='+id, Department,MyAppHttp.REQUEST_TYPES.UPDATE)
    }

    deleteDepartment(id) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,"api/Department/Delete?id=" + id, {},MyAppHttp.REQUEST_TYPES.DELETE)

    }
    duplicateDepartment(Department){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, 'api/Department/Duplicate', Department)
    }

}