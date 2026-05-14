import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminStats } from './components/admin-stats/admin-stats';
import { SourcesList } from './components/sources-list/sources-list';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'sources' },
  { path: 'sources', component: SourcesList },
  { path: 'stats', component: AdminStats },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
