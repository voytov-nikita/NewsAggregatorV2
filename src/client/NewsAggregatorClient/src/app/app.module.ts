import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { App } from './app';
import LayoutModule from './layout/layout.module';

@NgModule({
  declarations: [App],
  imports: [BrowserModule, AppRoutingModule, LayoutModule],
  providers: [provideHttpClient()],
  bootstrap: [App],
})
export class AppModule {}
