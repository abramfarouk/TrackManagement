import { TestBed } from '@angular/core/testing';
import { RouterModule } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AppComponent } from './app.component';
import { AuthService } from './services/auth.service';

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterModule.forRoot([]),
        HttpClientTestingModule
      ],
      declarations: [
        AppComponent
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render the nav brand', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.brand')?.textContent).toContain('Track Management');
  });

  it('should clear stored auth session when logging out', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    const authService = TestBed.inject(AuthService);

    localStorage.setItem('trackmanagement.jwt', 'token');
    localStorage.setItem('trackmanagement.user', 'admin');

    spyOn(authService, 'logout').and.callThrough();
    app.logout();

    expect(authService.logout).toHaveBeenCalled();
    expect(localStorage.getItem('trackmanagement.jwt')).toBeNull();
    expect(localStorage.getItem('trackmanagement.user')).toBeNull();
  });
});
