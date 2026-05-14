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
import {
  Avatar,
  CategoryChips,
  EmptyState,
  ErrorBanner,
  ErrorState,
  ImagePlaceholder,
  NotFound,
  Pager,
  SearchBar,
  Sidebar,
  Skeleton,
  SourceBadge,
  ToastHost,
  VoteButtons,
} from './components';

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

const APP_COMPONENTS = [
  Pager,
  Avatar,
  SourceBadge,
  ImagePlaceholder,
  Skeleton,
  VoteButtons,
  SearchBar,
  CategoryChips,
  Sidebar,
  ToastHost,
  EmptyState,
  ErrorState,
  ErrorBanner,
  NotFound,
];

@NgModule({
  declarations: [...APP_COMPONENTS],
  imports: [CommonModule, RouterModule, FormsModule, ...PRIMENG_MODULES],
  exports: [CommonModule, RouterModule, FormsModule, ...PRIMENG_MODULES, ...APP_COMPONENTS],
})
export default class SharedModule {}
