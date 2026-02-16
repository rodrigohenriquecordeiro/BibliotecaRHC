import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { BibliotecaService } from '../../../../services/biblioteca/biblioteca.service';

@Component({
  selector: 'app-importar-planilha',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './importar-planilha.component.html',
  styleUrl: './importar-planilha.component.css'
})
export class ImportarPlanilhaComponent {
  
  private service = inject(BibliotecaService);

  arquivoSelecionado: File | null = null;
  isDragging = false;
  uploading = false;
  
  mensagensErro: string[] = [];
  mensagemSucesso: string = '';

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    if (event.dataTransfer?.files.length) {
      this.validarEAtribuirArquivo(event.dataTransfer.files[0]);
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.validarEAtribuirArquivo(file);
    }
  }

  validarEAtribuirArquivo(file: File) {
    this.limparFeedback();

    if (!file.name.endsWith('.xlsx')) {
      this.mensagensErro = ['Formato inválido. Por favor, selecione apenas arquivos Excel (.xlsx).'];
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.mensagensErro = ['O arquivo é muito grande. O limite é 5MB.'];
      return;
    }

    this.arquivoSelecionado = file;
  }

  removerArquivo() {
    this.arquivoSelecionado = null;
    this.limparFeedback();
  }

  limparFeedback() {
    this.mensagensErro = [];
    this.mensagemSucesso = '';
  }

  enviarArquivo() {
    if (!this.arquivoSelecionado) return;

    this.uploading = true;
    this.limparFeedback();

    this.service.importarPlanilha(this.arquivoSelecionado).subscribe({
      next: (response) => {
        this.uploading = false;
        this.mensagemSucesso = 'Importação realizada com sucesso! Todos os livros foram salvos.';
        this.arquivoSelecionado = null;
      },
      error: (err: HttpErrorResponse) => {
        this.uploading = false;
        console.error('Erro na importação:', err);

        if (err.status === 400 && err.error) {
          if (err.error.erros && Array.isArray(err.error.erros)) {
             this.mensagensErro = err.error.erros;
          }
          else if (typeof err.error === 'string') {
             this.mensagensErro = [err.error];
          }
          else {
             this.mensagensErro = ['A planilha contém dados inválidos. Verifique o formato.'];
          }
        } 
        else if (err.status === 500) {
           this.mensagensErro = ['Ocorreu um erro interno no servidor. Tente novamente mais tarde.'];
        }
        else {
           this.mensagensErro = ['Erro de conexão. Verifique se a API está rodando.'];
        }
      }
    });
  }

  baixarModelo() {
    const link = document.createElement('a');
    link.href = 'assets/modelo-importacao.xlsx';
    link.download = 'modelo-importacao.xlsx';
    link.click();
  }

  formatarBytes(bytes: number, decimals = 2) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const dm = decimals < 0 ? 0 : decimals;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }
}
