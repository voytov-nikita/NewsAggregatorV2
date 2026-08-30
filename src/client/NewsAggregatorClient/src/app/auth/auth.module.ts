import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import SharedModule from '../shared/shared.module';
import { AuthRoutingModule } from './auth-routing.module';
import { AuthShell } from './components/auth-shell/auth-shell';
import { Login } from './components/login/login';
import { Register } from './components/register/register';

/**
 * ReactiveFormsModule is imported here and not in SharedModule: SharedModule is pulled into every
 * lazy chunk, and these are the only reactive forms in the app.
 *
 * The services, guards and interceptor under `src/app/auth/` are intentionally NOT provided by this
 * module - they are `providedIn: 'root'` because they have to work before this lazy chunk loads.
 */
@NgModule({
  declarations: [AuthShell, Login, Register],
  imports: [SharedModule, ReactiveFormsModule, AuthRoutingModule],
})
export default class AuthModule {}
