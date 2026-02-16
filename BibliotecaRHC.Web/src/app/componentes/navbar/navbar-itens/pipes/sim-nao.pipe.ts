import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'simNao',
  standalone: true
})
export class SimNaoPipe implements PipeTransform {

  transform(value: boolean | null | undefined): string {
    return value ? 'Sim' : 'Não';
  }

}
