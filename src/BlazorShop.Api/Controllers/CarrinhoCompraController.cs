using BlazorShop.Api.Mappings;
using BlazorShop.Api.Repositories;
using BlazorShop.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoCompraController : ControllerBase
    {
        private readonly ICarrinhoCompraRepository _carrinhoCompraRepository;
        private readonly IProdutoRepository _produtoRepository;

        private ILogger<CarrinhoCompraController> logger;

        public CarrinhoCompraController(ICarrinhoCompraRepository carrinhoCompraRepository, IProdutoRepository produtoRepository, ILogger<CarrinhoCompraController> logger)
        {
            _carrinhoCompraRepository = carrinhoCompraRepository;
            _produtoRepository = produtoRepository;
            this.logger = logger;
        }

        [HttpGet]
        [Route("{usuarioId}/GetItens")]
        public async Task<ActionResult<IEnumerable<CarrinhoItemDto>>> GetItens(string usuarioId)
        {
            try
            {
                var carrinhoItens = await _carrinhoCompraRepository.GetItens(usuarioId);
                if (carrinhoItens == null)
                {
                    return NoContent(); // 204 Status Code
                }

                var produtos = await this._produtoRepository.GetItens();
                if (produtos == null)
                {
                    throw new Exception("Não existem produtos...");
                }

                var carrinhoItensDto = carrinhoItens.ConverterCarrinhoItensParaDto(produtos);
                return Ok(carrinhoItensDto);
            }
            catch (Exception ex)
            {
                logger.LogError("## Erro ao obter itens do carrinho");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CarrinhoItemDto>> GetItem(int id)
        {
            try
            {
                var carrinhoItem = await _carrinhoCompraRepository.GetItem(id);
                if (carrinhoItem == null)
                {
                    return NoContent(); // 204 Status Code
                }

                var produtos = await this._produtoRepository.GetItem(carrinhoItem.ProdutoId);
                if (produtos == null)
                {
                    throw new Exception("Não existem produtos...");
                }

                var carrinhoItensDto = carrinhoItem.ConverterCarrinhoItemParaDto(produtos);
                return Ok(carrinhoItensDto);
            }
            catch (Exception ex)
            {
                logger.LogError("## Erro ao obter itens do carrinho");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CarrinhoItemDto>> PostItem([FromBody] CarrinhoItemAdicionaDto carrinhoItemAdicionaDto)
        {
            try
            {
                var novoCarrinhoItem = await _carrinhoCompraRepository.AdicionaItem(carrinhoItemAdicionaDto);

                if (novoCarrinhoItem == null)
                {
                    return NoContent(); //Status 204
                }

                var produto = await _produtoRepository.GetItem(novoCarrinhoItem.ProdutoId);

                if (produto == null)
                {
                    throw new Exception($"Produto não localizado (Id:({carrinhoItemAdicionaDto.ProdutoId})");
                }

                var novoCarrinhoItemDto = novoCarrinhoItem.ConverterCarrinhoItemParaDto(produto);

                return CreatedAtAction(nameof(GetItem), new { id = novoCarrinhoItemDto.Id },
                    novoCarrinhoItemDto);

            }
            catch (Exception ex)
            {
                logger.LogError("## Erro criar um novo item no carrinho");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CarrinhoItemDto>> DeleteItem(int id)
        {
            try
            {
                var carrinhoItem = await _carrinhoCompraRepository.DeletaItem(id);

                if (carrinhoItem == null)
                {
                    return NotFound();
                }

                var produto = await _produtoRepository.GetItem(carrinhoItem.ProdutoId);

                if (produto is null)
                    return NotFound();

                var carrinhoItemDto = carrinhoItem.ConverterCarrinhoItemParaDto(produto);
                return Ok(carrinhoItemDto);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<CarrinhoItemDto>> AtualizaQuantidade(int id,
        CarrinhoItemAtualizaQuantidadeDto carrinhoItemAtualizaQuantidadeDto)
        {
            try
            {

                var carrinhoItem = await _carrinhoCompraRepository.AtualizaQuantidade(id,
                                       carrinhoItemAtualizaQuantidadeDto);

                if (carrinhoItem == null)
                {
                    return NotFound();
                }
                var produto = await _produtoRepository.GetItem(carrinhoItem.ProdutoId);
                var carrinhoItemDto = carrinhoItem.ConverterCarrinhoItemParaDto(produto);
                return Ok(carrinhoItemDto);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("{categoriaId}/GetItensPorCategoria")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetItensPorCategoria(int categoriaId)
        {
            try
            {
                var produtos = await _produtoRepository.GetItensPorCategoria(categoriaId);
                var produtosDto = produtos.ConverterProdutosParaDto();
                return Ok(produtosDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao acessar o banco de dados");
            }
        }

        [HttpGet]
        [Route("GetCategorias")]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            try
            {
                var categorias = await _produtoRepository.GetCategorias();
                var categoriaDto = categorias.ConverterCategoriasParaDto();
                return Ok(categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao acessar o banco de dados");
            }
        }
    }
}