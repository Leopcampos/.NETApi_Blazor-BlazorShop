using BlazorShop.Models.DTOs;
using BlazorShop.Web.Services;
using global::Microsoft.AspNetCore.Components;

namespace BlazorShop.Web.Pages
{
    public partial class ProdutosPorCategoria
    {
        [Parameter]
        public int CategoriaId { get; set; }

        [Inject]
        public IProdutoService? ProdutoService { get; set; }
        public IEnumerable<ProdutoDto>? ProdutosDto { get; set; }

        [Inject]
        public IGerenciaProdutosLocalStorageService? GerenciaProdutosLocalStorageService { get; set; }
        public string? NomeCategoria { get; set; }
        public string? MensagemErro { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                //ProdutosDto = await ProdutoService.GetItensPorCategoria(CategoriaId);
                ProdutosDto = await GetColecaoProdutosPorCategoriaId(CategoriaId);
                if (ProdutosDto != null && ProdutosDto.Count() > 0)
                {
                    var produtoDto = ProdutosDto.FirstOrDefault(p => p.CategoriaId == CategoriaId);
                    if (produtoDto != null)
                    {
                        NomeCategoria = produtoDto.CategoriaNome;
                    }
                }
            }
            catch (Exception ex)
            {
                MensagemErro = ex.Message;
            }
        }

        private async Task<IEnumerable<ProdutoDto>> GetColecaoProdutosPorCategoriaId(int categoriaId)
        {
            var produtoCollection = await GerenciaProdutosLocalStorageService.GetCollection();
            if (produtoCollection != null)
            {
                return produtoCollection.Where(p => p.CategoriaId == categoriaId);
            }
            else
            {
                return await ProdutoService.GetItensPorCategoria(categoriaId);
            }
        }
    }
}