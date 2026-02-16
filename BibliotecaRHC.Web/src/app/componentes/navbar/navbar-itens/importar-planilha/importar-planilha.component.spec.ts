import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportarPlanilhaComponent } from './importar-planilha.component';

describe('ImportarPlanilhaComponent', () => {
  let component: ImportarPlanilhaComponent;
  let fixture: ComponentFixture<ImportarPlanilhaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImportarPlanilhaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImportarPlanilhaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
