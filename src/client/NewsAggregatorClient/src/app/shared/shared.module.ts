import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PaginatorModule } from 'primeng/paginator';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { PopoverModule } from 'primeng/popover';
import { TabsModule } from 'primeng/tabs';
import { TagModule } from 'primeng/tag';
import { AvatarModule } from 'primeng/avatar';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { ChipModule } from 'primeng/chip';
import { MenuModule } from 'primeng/menu';
import { Pager } from './components';

const PRIMENG_MODULES = [
  PaginatorModule,
  ButtonModule,
  InputTextModule,
  TextareaModule,
  ToggleSwitchModule,
  PopoverModule,
  TabsModule,
  TagModule,
  AvatarModule,
  BreadcrumbModule,
  ChipModule,
  MenuModule,
];

@NgModule({
  declarations: [Pager],
  imports: [CommonModule, RouterModule, FormsModule, ...PRIMENG_MODULES],
  exports: [CommonModule, RouterModule, FormsModule, Pager, ...PRIMENG_MODULES],
})
export default class SharedModule {}
