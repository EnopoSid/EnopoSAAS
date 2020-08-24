import {Injectable} from '@angular/core';
import {SendReceiveService} from '../../../services/common/sendReceive.service';
import MyAppHttp from '../../../services/common/myAppHttp.service';
declare var jsPDF: any;
@Injectable()
export class SalesReportService {
    HMACKey: string;
    constructor(private sendReceiveService: SendReceiveService) { }

    public setHMACKey(input) {
        this.HMACKey = input;
    }

    SalesSearch(advancedSearch)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'',advancedSearch,{})
    }
    getFreshOrderSales(searchParamAdvance)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Sales/GetFreshSalesReport',searchParamAdvance,{})
    }

    getGourmentOrderSales(objparam)
    {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Sales/GetGourmetSalesReport',objparam,{})
    }

    getUsersByRoleIdOrDeptId(complaintSearchBy) {
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Post,'api/Users/GetUserByRoleIdorUserId?roleId='+complaintSearchBy.roleId+'&userId='+complaintSearchBy.userId+'&reportName='+complaintSearchBy.reportName,{});
    }

    getConsumerComplaintFormDropdownListItems(){
    }
    getZonesByRegionId(regionId){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Zone/GetByRegionId?id='+regionId,{})
    }
    getopratorAndStaff(roletype ){
        return this.sendReceiveService.send(this.sendReceiveService.httpVerb.Get,'api/Role/opratorAndStaff?roleType='+ roletype,{})
    }
    
convert(cols, TempDate,reportname){
    var doc = new jsPDF();
    var col = cols;
    var rows = TempDate;
    doc.autoTable(col, rows,{// theme: 'Default', // 'striped', 'grid' or 'plain'
    styles: { cellPadding:5, // a number, array or object (see margin below)
        columnWidth: 'auto',
    },
    headerStyles: { overflow: "linebreak", rowHeight: 5},
    bodyStyles: {
        overflow: "linebreak",
        rowHeight: 5,
        halign: "left",
        valign: "top",
        cellPadding: 5,
        lineWidth: 0.1
    },
    alternateRowStyles: {},
    columnStyles: {columnWidth:'auto'},
 
    // Properties
    startY: 2, // false (indicates margin top value) or a number
    margin: { top: 35, left: 5, right: 5, bottom: 35 },
    setFount:15, 
    pageBreak: 'auto', // 'auto', 'avoid' or 'always'
    tableWidth: 'auto', // 'auto', 'wrap' or a number, 
    showHeader: 'everyPage', // 'everyPage', 'firstPage', 'never',
    tableLineWidth:2,
 
    });
    doc.save(reportname);
  }

}