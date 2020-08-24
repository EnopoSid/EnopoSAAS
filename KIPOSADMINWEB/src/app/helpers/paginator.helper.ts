import { MatPaginatorIntl } from '@angular/material';

export function getCMSPaginatorIntl() {
    const paginatorIntl = new MatPaginatorIntl();
    
    paginatorIntl.itemsPerPageLabel = 'Records per page';
    return paginatorIntl;
  }