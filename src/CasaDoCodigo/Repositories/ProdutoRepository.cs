using CasaDoCodigo.Models;
using CasaDoCodigo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public IList<Produto> GetProdutos()
        {
            return dbSet.ToList();
        }

        public void SaveProdutos(List<Livro> livros)
        {
            foreach (var livro in livros)
            {
                if (!dbSet.Any(x => x.Codigo == livro.Codigo))
                    dbSet.Add(new Produto(livro.Codigo, livro.Nome, livro.Preco));
            }

            _context.SaveChanges();
        }
    }
}
