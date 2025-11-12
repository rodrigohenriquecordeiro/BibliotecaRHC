import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FrasesInesqueciveisAdicionarComponent } from './frases-inesqueciveis-adicionar.component';

describe('FrasesInesqueciveisAdicionarComponent', () => {
  let component: FrasesInesqueciveisAdicionarComponent;
  let fixture: ComponentFixture<FrasesInesqueciveisAdicionarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FrasesInesqueciveisAdicionarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FrasesInesqueciveisAdicionarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
