using CasaDoCodigo.Models;
using CasaDoCodigo.Repositories;
using CasaDoCodigo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace CasaDoCodigo
{
    public partial class Startup
    {
        public class DataService : IDataService
        {
            private readonly ApplicationContext _applicationContext;
            private readonly IProdutoRepository _produtoRepository;

            public DataService(ApplicationContext applicationContext, IProdutoRepository produtoRepository)
            {
                _applicationContext = applicationContext;
                _produtoRepository = produtoRepository;
            }

            public void InicializaDB()
            {
                _applicationContext.Database.Migrate();
                
                List<Livro> livros = GetLivros();

                _produtoRepository.SaveProdutos(livros);
            }

            private static List<Livro> GetLivros()
            {
                var json = File.ReadAllText("livros.json");
                var livros = JsonConvert.DeserializeObject<List<Livro>>(json);
                return livros;
            }
        }
    }
    public class Livro
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }
}
