import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FrasesInesqueciveisApagarComponent } from './frases-inesqueciveis-apagar.component';

describe('FrasesInesqueciveisApagarComponent', () => {
  let component: FrasesInesqueciveisApagarComponent;
  let fixture: ComponentFixture<FrasesInesqueciveisApagarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FrasesInesqueciveisApagarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FrasesInesqueciveisApagarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
