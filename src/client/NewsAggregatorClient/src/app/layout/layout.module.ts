import { NgModule } from '@angular/core';
import SharedModule from '../shared/shared.module';
import { AuthorizedLayout } from './components/authorized-layout/authorized-layout';
import { SettingsMenu } from './components/settings-menu/settings-menu';
import { UserMenu } from './components/user-menu/user-menu';

@NgModule({
  declarations: [AuthorizedLayout, SettingsMenu, UserMenu],
  imports: [SharedModule],
  exports: [AuthorizedLayout],
})
export default class LayoutModule {}
