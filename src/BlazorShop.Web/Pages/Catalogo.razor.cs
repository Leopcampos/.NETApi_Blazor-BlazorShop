using BlazorShop.Models.DTOs;
using BlazorShop.Web.Services;
using global::Microsoft.AspNetCore.Components;

namespace BlazorShop.Web.Pages
{
    public partial class Catalogo
    {
        public IEnumerable<ProdutoDto>? Produtos { get; set; }

        [Inject]
        public IGerenciaProdutosLocalStorageService? GerenciaProdutosLocalStorageService { get; set; }

        [Inject]
        public IGerenciaCarrinhoItensLocalStorageService? GerenciaCarrinhoItensLocalStorageService { get; set; }

        [Inject]
        public ICarrinhoCompraService? CarrinhoCompraService { get; set; }
        public string? MensagemErro { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LimpaLocalStorage();
                Produtos = await GerenciaProdutosLocalStorageService.GetCollection();
                var carrinhoCompraItens = await GerenciaCarrinhoItensLocalStorageService.GetCollection();
                var totalQuantidade = carrinhoCompraItens.Sum(i => i.Quantidade);
                CarrinhoCompraService.RaiseEventOnCarrinhoCompraChanged(totalQuantidade);
            }
            catch (Exception ex)
            {
                MensagemErro = ex.Message;
            }
        }

        private async Task LimpaLocalStorage()
        {
            await GerenciaProdutosLocalStorageService.RemoveCollection();
            await GerenciaCarrinhoItensLocalStorageService.RemoveCollection();
        }
    }
}