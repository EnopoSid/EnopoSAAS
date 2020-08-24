import { Directive, HostListener, ElementRef, Input, Renderer2, ViewChild } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort } from '@angular/material';

@Directive({
    selector: '[filter]',
    inputs:['filter'],
    host:{
      '(keyup)': 'onInput()'
    }
    })
    export class filterDirective {
        @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;
        @ViewChild(MatSort, {static: false}) sort: MatSort;
        filter;
    constructor(public el: ElementRef, public renderer: Renderer2) { };
    onInput() {
     
    var tempGridData = [];
    tempGridData = this.filter.gridData;
    
    for(var i=0;i<this.filter.filterColumnNames.length; i++){
      tempGridData =  tempGridData.filter(
        x=>(!!this.filter.filterColumnNames[i].Value ? 
          (!!x[this.filter.filterColumnNames[i].Key] ? x[this.filter.filterColumnNames[i].Key] : "" ).toString().toLowerCase().indexOf(this.filter.filterColumnNames[i].Value.toString().trim().toLowerCase()) > -1 : true))
    }
    this.filter.dataSource =new MatTableDataSource( tempGridData );
    this.filter.dataSource.paginator = this.filter.paginator;
    this.filter.dataSource.sort = this.filter.sort;
    if (this.filter.paginator) {
      this.filter.paginator.firstPage();
    }
    }
    }