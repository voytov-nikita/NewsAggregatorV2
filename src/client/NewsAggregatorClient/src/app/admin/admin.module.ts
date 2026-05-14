import { NgModule } from '@angular/core';
import SharedModule from '../shared/shared.module';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminStats } from './components/admin-stats/admin-stats';
import { FieldMapper } from './components/field-mapper/field-mapper';
import { SourceCard } from './components/source-card/source-card';
import { SourceWizard } from './components/source-wizard/source-wizard';
import { SourcesList } from './components/sources-list/sources-list';

@NgModule({
  declarations: [SourceCard, FieldMapper, SourceWizard, SourcesList, AdminStats],
  imports: [SharedModule, AdminRoutingModule],
})
export default class AdminModule {}
