import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../../services/common/sendReceive.service';
import MyAppHttp from '../../../services/common/myAppHttp.service';
declare var jsPDF: any;
@Injectable()
export class AllReportsService {
    constructor(private sendReceiveService: SendReceiveService) { }
   
    Salesreport(reportSearchBy){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Sales/SalesReportBasedonDates?fDate='+reportSearchBy.fDate+ '&tdate='+ reportSearchBy.tDate+'&storeId='+reportSearchBy.storeId,{})
    }

    getStoreDetails(){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"/api/Common/GetAllStorePickupPoint",{});    
     }
   
}





