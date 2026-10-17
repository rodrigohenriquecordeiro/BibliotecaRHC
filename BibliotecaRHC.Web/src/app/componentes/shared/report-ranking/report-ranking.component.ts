import { Component, Input } from '@angular/core';

export interface RankingRow {
  label: string;
  value: string | number;
}

@Component({
  selector: 'app-report-ranking',
  standalone: true,
  templateUrl: './report-ranking.component.html'
})
export class ReportRankingComponent {
  @Input({ required: true }) title = '';
  @Input({ required: true }) firstColumn = '';
  @Input({ required: true }) secondColumn = '';
  @Input() rows: RankingRow[] = [];
}
