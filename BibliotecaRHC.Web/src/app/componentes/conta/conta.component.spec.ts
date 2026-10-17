import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AuthService } from '../../services/auth/auth.service';
import { ContaComponent } from './conta.component';

describe('ContaComponent', () => {
  let fixture: ComponentFixture<ContaComponent>;
  let component: ContaComponent;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', [
      'obterConta',
      'atualizarConta',
      'saveToken'
    ]);
    authService.obterConta.and.returnValue(of({ nome: 'leitor', email: 'leitor@example.com' }));
    authService.atualizarConta.and.returnValue(of({
      accessToken: 'token-atualizado',
      usuario: { nome: 'novo-leitor', email: 'novo@example.com' }
    }));

    await TestBed.configureTestingModule({
      imports: [ContaComponent],
      providers: [{ provide: AuthService, useValue: authService }]
    }).compileComponents();

    fixture = TestBed.createComponent(ContaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('carrega o nome e o email da conta autenticada', () => {
    expect(component.form.get('nome')?.value).toBe('leitor');
    expect(component.form.get('email')?.value).toBe('leitor@example.com');
  });

  it('exige senha atual e confirmação para alterar a senha', () => {
    component.form.patchValue({ novaSenha: 'nova-senha', confirmarSenha: 'nova-senha' });

    expect(component.form.hasError('senhaAtualObrigatoria')).toBeTrue();

    component.form.patchValue({ senhaAtual: 'senha-atual', confirmarSenha: 'outra-senha' });

    expect(component.form.hasError('senhasDiferentes')).toBeTrue();
  });

  it('salva as alterações e atualiza o token da sessão', () => {
    component.form.patchValue({ nome: ' novo-leitor ', email: ' novo@example.com ' });

    component.salvar();

    expect(authService.atualizarConta).toHaveBeenCalledWith({
      nome: 'novo-leitor',
      email: 'novo@example.com',
      senhaAtual: undefined,
      novaSenha: undefined
    });
    expect(authService.saveToken).toHaveBeenCalledWith('token-atualizado');
    expect(component.mensagemSucesso).toBe('Dados da conta atualizados com sucesso.');
  });
});
