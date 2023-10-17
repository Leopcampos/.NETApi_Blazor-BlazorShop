using BlazorShop.Models.DTOs;
using BlazorShop.Web.Services;
using global::Microsoft.AspNetCore.Components;

namespace BlazorShop.Web.Pages
{
    public partial class ProdutoDetalhes
    {
        [Inject]
        public IProdutoService? ProdutoService { get; set; }

        [Inject]
        public ICarrinhoCompraService? CarrinhoCompraService { get; set; }

        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        [Inject]
        public IGerenciaProdutosLocalStorageService? GerenciaProdutosLocalStorageService { get; set; }

        [Inject]
        public IGerenciaCarrinhoItensLocalStorageService? GerenciaCarrinhoItensLocalStorageService { get; set; }
        private List<CarrinhoItemDto>? CarrinhoCompraItens { get; set; }

        [Parameter]
        public int Id { get; set; }
        public ProdutoDto? Produto { get; set; }
        public string? MensagemErro { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                CarrinhoCompraItens = await GerenciaCarrinhoItensLocalStorageService.GetCollection();
                //Produto = await ProdutoService.GetItem(Id);
                Produto = await GetProdutoPorId(Id);
            }
            catch (Exception ex)
            {
                MensagemErro = ex.Message;
            }
        }

        protected async Task AdicionarAoCarrinho_Click(CarrinhoItemAdicionaDto carrinhoItemAdicionaDto)
        {
            try
            {
                var carrinhoItemDto = await CarrinhoCompraService.AdicionaItem(carrinhoItemAdicionaDto);
                if (carrinhoItemDto != null)
                {
                    CarrinhoCompraItens.Add(carrinhoItemDto);
                    await GerenciaCarrinhoItensLocalStorageService.SaveCollection(CarrinhoCompraItens);
                }

                NavigationManager.NavigateTo("/CarrinhoCompra");
            }
            catch (Exception)
            {
                //Log Exception
                throw;
            }
        }

        private async Task<ProdutoDto> GetProdutoPorId(int id)
        {
            var produtosDto = await GerenciaProdutosLocalStorageService.GetCollection();
            if (produtosDto != null)
            {
                return produtosDto.SingleOrDefault(p => p.Id == id);
            }

            return null;
        }
    }
}