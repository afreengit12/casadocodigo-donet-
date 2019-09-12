using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.Models;
using CasaDoCodigo.Models.ViewModels;
using CasaDoCodigo.Repositories;
using CasaDoCodigo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CasaDoCodigo.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoController(IProdutoRepository produtoRepository, IPedidoRepository pedidoRepository)
        {
            _produtoRepository = produtoRepository;
            _pedidoRepository = pedidoRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Carrossel()
        {
            return View(_produtoRepository.GetProdutos());
        }

        public IActionResult Carrinho(string codigo)
        {
            if (!string.IsNullOrEmpty(codigo))
                _pedidoRepository.AddItem(codigo);

            Pedido pedido = _pedidoRepository.GetPedido();
            var viewModel = new CarrinhoViewModel(pedido.Itens);
            return View(viewModel);
        }

        public IActionResult Cadastro()
        {
            Pedido pedido = _pedidoRepository.GetPedido();

            if (pedido == null)
                RedirectToAction("Carrossel");

            return View(pedido.Cadastro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Resumo(Cadastro cadastro)
        {
            if (ModelState.IsValid)
                return View(_pedidoRepository.UpdateCadastro(cadastro));

            return RedirectToAction("Cadastro");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public UpdateQuantidadeResponse UpdateQuantidade([FromBody]ItemPedido itemPedido)
        {
            return _pedidoRepository.UpdateQuantidade(itemPedido);
        }
    }
}