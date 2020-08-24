import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../../services/common/sendReceive.service';
import MyAppHttp from '../../../services/common/myAppHttp.service';
declare var jsPDF: any;
@Injectable()
export class IngradientCountReportService {
    constructor(private sendReceiveService: SendReceiveService) { }

   

    getIngradientReport(storeId,transformfdate,transformtdate,categoryId,mainCategoryFlag){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"api/BowlsCount/GetPOSOrderProductAttributeValuesForAdmin?StoreId="+storeId+"&fDate="+transformfdate+"&tDate="+transformtdate+"&categoryid="+categoryId+"&AllMainCate="+mainCategoryFlag,{});
    }
    getTopMenu() {
        var requestdata={
            "CustomerGUID": "00000000-0000-0000-0000-000000000000"
          }
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post, "Api/Client/POSTopMenu", requestdata,{},true);
    };
    

     getAllIsActiveStorePickupPoints()
     {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,"GetAllIsActiveStorePickUpPoints",{});    
     }
   
}





