import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Settings } from './components/settings/settings';

const routes: Routes = [
  {
    path: '',
    component: Settings,
    data: { pageTitle: 'Настройки' },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SettingsRoutingModule {}
