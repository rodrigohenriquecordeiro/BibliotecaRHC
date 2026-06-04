export interface UsuarioCadastro {
  nome: string;
  email: string;
  senha?: string;             
  confirmarSenha?: string;    
  dataCriacao: string;        
}