import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FrasesInesqueciveisDashboardComponent } from './frases-inesqueciveis-dashboard.component';

describe('FrasesInesqueciveisDashboardComponent', () => {
  let component: FrasesInesqueciveisDashboardComponent;
  let fixture: ComponentFixture<FrasesInesqueciveisDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FrasesInesqueciveisDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FrasesInesqueciveisDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
