import { NgModule } from '@angular/core';
import SharedModule from '../shared/shared.module';
import { ForYouRoutingModule } from './foryou-routing.module';
import { ForYou } from './components/foryou/foryou';

@NgModule({
  declarations: [ForYou],
  imports: [SharedModule, ForYouRoutingModule],
})
export default class ForYouModule {}
