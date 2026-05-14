import { NgModule } from '@angular/core';
import SharedModule from '../shared/shared.module';
import { AuthorizedLayout } from './components/authorized-layout/authorized-layout';
import { SettingsMenu } from './components/settings-menu/settings-menu';

@NgModule({
  declarations: [AuthorizedLayout, SettingsMenu],
  imports: [SharedModule],
  exports: [AuthorizedLayout],
})
export default class LayoutModule {}
