import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthShell } from './components/auth-shell/auth-shell';
import { Login } from './components/login/login';
import { Register } from './components/register/register';

const routes: Routes = [
  {
    path: '',
    component: AuthShell,
    children: [
      { path: 'login', component: Login, data: { pageTitle: 'Sign in' } },
      { path: 'register', component: Register, data: { pageTitle: 'Create account' } },
      { path: '', pathMatch: 'full', redirectTo: 'login' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
