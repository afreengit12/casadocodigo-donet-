using CasaDoCodigo.Models;
using CasaDoCodigo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public class CadastroRepository : BaseRepository<Cadastro>, ICadastroRepository
    {
        public CadastroRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public Cadastro Update(int cadastroId, Cadastro novoCadastro)
        {
            var cadastroDB = dbSet.SingleOrDefault(c => c.Id == cadastroId);

            if (cadastroDB == null)
                throw new ArgumentNullException("cadastro");

            cadastroDB.Update(novoCadastro);
            _context.SaveChanges();

            return novoCadastro;
        }
    }
}
