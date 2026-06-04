import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginCadastroComponent } from './login-cadastro.component';

describe('LoginCadastroComponent', () => {
  let component: LoginCadastroComponent;
  let fixture: ComponentFixture<LoginCadastroComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginCadastroComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoginCadastroComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
