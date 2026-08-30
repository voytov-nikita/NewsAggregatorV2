import { TestBed } from '@angular/core/testing';
import { RouterModule, provideRouter } from '@angular/router';
import { App } from './app';
// Imported so the compiler sees the NgModule that declares App and can resolve `router-outlet` in
// its template. Without it App has no compilation scope and the whole test build fails with NG8001.
import { AppModule } from './app.module';

// The original CLI scaffold declared App as standalone and asserted a default "Hello" heading;
// neither has been true since the app moved to NgModules, so this suite never compiled.
describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [App],
      imports: [RouterModule],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('is declared by AppModule', () => {
    expect(AppModule).toBeTruthy();
  });

  it('creates the root component', () => {
    const fixture = TestBed.createComponent(App);

    expect(fixture.componentInstance).toBeTruthy();
  });

  it('renders the router outlet', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();

    expect((fixture.nativeElement as HTMLElement).querySelector('router-outlet')).toBeTruthy();
  });
});
