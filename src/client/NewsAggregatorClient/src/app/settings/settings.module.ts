import { NgModule } from '@angular/core';
import { SettingsRoutingModule } from './settings-routing.module';
import { Settings } from './components/settings/settings';
import SharedModule from '../shared/shared.module';

@NgModule({
  declarations: [Settings],
  imports: [SharedModule, SettingsRoutingModule],
})
export default class SettingsModule {}
