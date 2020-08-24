import { Injectable } from '@angular/core';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';
const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
const EXCEL_EXTENSION = '.xlsx';
declare var jsPDF: any;
@Injectable()
export class ExcelService {
constructor() { }
public exportAsExcelFile(json: any[], excelFileName: string): void {
  const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);
  const workbook: XLSX.WorkBook = { Sheets: { 'data': worksheet }, SheetNames: ['data'] };
  const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
  this.saveAsExcelFile(excelBuffer, excelFileName);
}
private saveAsExcelFile(buffer: any, fileName: string): void {
   const data: Blob = new Blob([buffer], {type: EXCEL_TYPE});
   FileSaver.saveAs(data, fileName + '_export_' + new  Date().getTime() + EXCEL_EXTENSION);
}
public exportAsPdf(cols, TempDate,reportname){
  var doc = new jsPDF();
  var col = cols;
  var rows = TempDate;
  var i
    doc.autoTable(col, rows,{// theme: 'Default', // 'striped', 'grid' or 'plain'
    styles: { cellPadding:0, // a number, array or object (see margin below)
        columnWidth: 'auto',
        // 'auto', 'wrap' or a number
    },
    headerStyles: { overflow: "linebreak", rowHeight:5 ,halign: "left",
    valign: "middle",lineWidth:0.5, cellPadding: 3},
    bodyStyles: {
        overflow: "linebreak",
        rowHeight:5,
        halign: "left",
        valign: "middle",
        cellPadding: 3,
       lineWidth:3
    },
    alternateRowStyles: {},
    columnStyles: {columnWidth:'auto'},
 
    // Properties
    startY: 2, // false (indicates margin top value) or a number
    margin: { top:5, left:5 , right:5, bottom:5  },
    setFount:5, 
    pageBreak: 'auto', // 'auto', 'avoid' or 'always'
    tableWidth: 'auto', // 'auto', 'wrap' or a number, 
    showHeader: 'everyPage', // 'everyPage', 'firstPage', 'never',
    tableLineWidth:5,
    });

  doc.save(reportname);
}
}