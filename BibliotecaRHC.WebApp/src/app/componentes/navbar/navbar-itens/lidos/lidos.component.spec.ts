import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LidosComponent } from './lidos.component';

describe('LidosComponent', () => {
  let component: LidosComponent;
  let fixture: ComponentFixture<LidosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LidosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LidosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
