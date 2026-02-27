import { NgModule } from '@angular/core';
import SharedModule from '../shared/shared.module';
import { AuthorizedLayout } from './components/authorized-layout/authorized-layout';

@NgModule({
  declarations: [AuthorizedLayout],
  imports: [SharedModule],
  exports: [AuthorizedLayout],
})
export default class LayoutModule {}
