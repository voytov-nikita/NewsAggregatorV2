import { NgModule } from '@angular/core';
import SharedModule from '../shared/shared.module';
import { SavedRoutingModule } from './saved-routing.module';
import { Saved } from './components/saved/saved';

@NgModule({
  declarations: [Saved],
  imports: [SharedModule, SavedRoutingModule],
})
export default class SavedModule {}
