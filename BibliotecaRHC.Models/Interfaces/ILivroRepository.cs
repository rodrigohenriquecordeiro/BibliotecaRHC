using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BibliotecaRHC.Models.Entities;

namespace BibliotecaRHC.Models.Interfaces;

public interface ILivroRepository
{
    IEnumerable<Livro> ObterTodos();

    Livro ObterPorId(int id);

    void Adicionar(Livro produto);

    void Atualizar(Livro produto);

    void Excluir(int id);
}
