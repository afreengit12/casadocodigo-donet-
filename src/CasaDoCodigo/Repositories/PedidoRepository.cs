using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Repositories
{
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IItemPedidoRepository _itemPedidoRepository;
        private readonly ICadastroRepository _cadastroRepository;
        public PedidoRepository(ApplicationContext applicationContext,
            IHttpContextAccessor contextAccessor,
            IItemPedidoRepository itemPedidoRepository,
            ICadastroRepository cadastroRepository)
            : base(applicationContext)
        {
            _contextAccessor = contextAccessor;
            _itemPedidoRepository = itemPedidoRepository;
            _cadastroRepository = cadastroRepository;
        }

        public void AddItem(string codigo)
        {
            var produto = _context.Set<Produto>().SingleOrDefault(x => x.Codigo == codigo);

            if (produto == null)
            {
                throw new ArgumentException("Produto não encontrado.");
            }

            var pedido = GetPedido();

            var itemPedido = _context.Set<ItemPedido>().SingleOrDefault(x =>
                                      x.Produto.Codigo == codigo
                                      && x.Pedido.Id == pedido.Id);

            var teste = _context.Set<Pedido>().Where(x => x.Id == 1);

            if (itemPedido == null)
            {
                itemPedido = new ItemPedido(pedido, produto, 1, produto.Preco);
                _context.Set<ItemPedido>().Add(itemPedido);
                _context.SaveChanges();
            }
        }

        public Pedido GetPedido()
        {
            var pedidoId = GetPedidoId();
            var pedido = dbSet
                .Include(x => x.Itens)
                    .ThenInclude(x => x.Produto)
                .Include(x => x.Cadastro)
                .FirstOrDefault(x => x.Id == pedidoId);

            if (pedido == null)
            {
                pedido = new Pedido();
                dbSet.Add(pedido);
                _context.SaveChanges();
                SetPedidoId(pedido.Id);
            }

            return pedido;
        }

        private int? GetPedidoId()
        {
            return _contextAccessor.HttpContext.Session.GetInt32("pedidoId");
        }

        private void SetPedidoId(int pedidoId)
        {
            _contextAccessor.HttpContext.Session.SetInt32("pedidoId", pedidoId);
        }

        public UpdateQuantidadeResponse UpdateQuantidade(ItemPedido itemPedido)
        {
            var itemPedidoDB = _itemPedidoRepository.GetItemPedido(itemPedido.Id);

            if (itemPedidoDB != null)
            {
                itemPedidoDB.AtualizaQuantidade(itemPedido.Quantidade);

                if (itemPedido.Quantidade == 0)
                    _itemPedidoRepository.RemoveItemPedido(itemPedido.Id);

                _context.SaveChanges();

                return new UpdateQuantidadeResponse(itemPedidoDB, new CarrinhoViewModel(GetPedido().Itens));
            }

            throw new ArgumentException("ItemPedido não encontrado.");
        }

        public Pedido UpdateCadastro(Cadastro cadastro)
        {
            var pedido = GetPedido();
            _cadastroRepository.Update(pedido.Cadastro.Id, cadastro);

            return pedido;
        }
    }
}
