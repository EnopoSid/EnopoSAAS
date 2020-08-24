import { Injectable } from '@angular/core';
import * as FileSaver from 'file-saver';
import * as XLS from 'xlsx';
import * as $ from 'jquery';
declare var jsPDF: any;

const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8';
const EXCEL_EXTENSION = '.xlsx';



@Injectable()
export class ExportService {

  constructor() { }

  public exportAsExcelFile(json: any, excelFileName: string): void {
  var blob = new Blob([json], {
    type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
  });
 FileSaver.saveAs(blob, excelFileName);
  }


  public exportAsPdf(cols, TempDate,reportname){
    var doc = new jsPDF();
    var col = cols;
    var rows = TempDate;
    var i
      doc.autoTable(col, rows,{// theme: 'Default', // 'striped', 'grid' or 'plain'
      styles: { cellPadding:0, // a number, array or object (see margin below)
          columnWidth: 'auto',
      },
      headerStyles: { overflow: "linebreak", rowHeight: 5 ,halign: "left",
      valign: "top",lineWidth:0.5, cellPadding: 2},
      bodyStyles: {
          overflow: "linebreak",
         // rowHeight: 5,
          rowHeight:5,
          halign: "left",
          valign: "top",
          cellPadding: 2,
         lineWidth:3
      },
      alternateRowStyles: {},
      columnStyles: {columnWidth:'auto'},
   
      // Properties
      startY: 2, // false (indicates margin top value) or a number
      margin: { top:10, left:5 , right:5, bottom:3  },
      setFount:5, 
      pageBreak: 'auto', // 'auto', 'avoid' or 'always'
      tableWidth: 'auto', // 'auto', 'wrap' or a number, 
      showHeader: 'everyPage', // 'everyPage', 'firstPage', 'never',
      tableLineWidth:5,
      });

    doc.save(reportname);
  }
  

}