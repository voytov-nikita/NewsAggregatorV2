import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Pager } from './components';

@NgModule({
  declarations: [Pager],
  imports: [CommonModule, RouterModule],
  exports: [CommonModule, RouterModule, Pager],
})
export default class SharedModule {}
