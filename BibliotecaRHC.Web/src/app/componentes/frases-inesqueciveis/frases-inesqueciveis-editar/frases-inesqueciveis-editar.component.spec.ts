import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FrasesInesqueciveisEditarComponent } from './frases-inesqueciveis-editar.component';

describe('FrasesInesqueciveisEditarComponent', () => {
  let component: FrasesInesqueciveisEditarComponent;
  let fixture: ComponentFixture<FrasesInesqueciveisEditarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FrasesInesqueciveisEditarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FrasesInesqueciveisEditarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
