import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjetosDeLeituraComponent } from './projetos-de-leitura.component';

describe('ProjetosDeLeituraComponent', () => {
  let component: ProjetosDeLeituraComponent;
  let fixture: ComponentFixture<ProjetosDeLeituraComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjetosDeLeituraComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjetosDeLeituraComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
