import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjetoLeituraComponent } from './projeto-leitura.component';

describe('ProjetoLeituraComponent', () => {
  let component: ProjetoLeituraComponent;
  let fixture: ComponentFixture<ProjetoLeituraComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjetoLeituraComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjetoLeituraComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
