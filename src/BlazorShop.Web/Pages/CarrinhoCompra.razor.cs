using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using BlazorShop.Web;
using BlazorShop.Web.Shared;
using BlazorShop.Web.Services;
using BlazorShop.Models.DTOs;
using BlazorShop.Web.Pages;

namespace BlazorShop.Web.Pages
{
    public partial class CarrinhoCompra
    {
        [Inject]
        public ICarrinhoCompraService? CarrinhoCompraService { get; set; }
        public List<CarrinhoItemDto>? CarrinhoCompraItens { get; set; }
        public string? MensagemErro { get; set; }
        protected string? PrecoTotal { get; set; }
        protected int QuantidadeTotal { get; set; }

        [Inject]
        public IGerenciaCarrinhoItensLocalStorageService? GerenciaCarrinhoItensLocalStorageService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                CarrinhoCompraItens = await GerenciaCarrinhoItensLocalStorageService.GetCollection();
                //CalculaResumoCarrinhoTotal();
                CarrinhoChanged();
            }
            catch (Exception ex)
            {
                MensagemErro = ex.Message;
            }
        }

        protected async Task DeletaCarrinhoItem_Click(int id)
        {
            //Excluir o item do banco de dados
            var carrinhoItemDto = await CarrinhoCompraService.DeletaItem(id);
            //Remoção do item da coleção de objetos da memoria no cliente
            await RemoveCarrinhoItem(id);
            //CalculaResumoCarrinhoTotal();
            CarrinhoChanged();
        }

        private CarrinhoItemDto GetCarrinhoItem(int id)
        {
            return CarrinhoCompraItens.FirstOrDefault(i => i.Id == id);
        }

        private async Task RemoveCarrinhoItem(int id)
        {
            var carrinhoItemDto = GetCarrinhoItem(id);
            CarrinhoCompraItens.Remove(carrinhoItemDto);
            await GerenciaCarrinhoItensLocalStorageService.SaveCollection(CarrinhoCompraItens);
        }

        protected async Task AtualizaQuantidadeCarrinhoItem_Click(int id, int quantidade)
        {
            try
            {
                if (quantidade > 0)
                {
                    var atualizaItemDto = new CarrinhoItemAtualizaQuantidadeDto
                    {
                        CarrinhoItemId = id,
                        Quantidade = quantidade
                    };
                    var retornaItemAtualizadoDto = await CarrinhoCompraService.AtualizaQuantidade(atualizaItemDto);
                    AtualizaPrecoTotalItem(retornaItemAtualizadoDto);
                    //CalculaResumoCarrinhoTotal();
                    CarrinhoChanged();
                    await Js.InvokeVoidAsync("TornaBotaoAtualizarQuantidadeVisivel", id, false);
                }
                else
                {
                    var item = CarrinhoCompraItens.FirstOrDefault(i => i.Id == id);
                    if (item is not null)
                    {
                        item.Quantidade = 1;
                        item.PrecoTotal = item.Preco;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetPrecoTotal()
        {
            PrecoTotal = CarrinhoCompraItens.Sum(p => p.PrecoTotal).ToString("C");
        }

        private void SetQuantidadeTotal()
        {
            QuantidadeTotal = CarrinhoCompraItens.Sum(p => p.Quantidade);
        }

        private void CalculaResumoCarrinhoTotal()
        {
            SetPrecoTotal();
            SetQuantidadeTotal();
        }

        private async Task AtualizaPrecoTotalItem(CarrinhoItemDto carrinhoItemDto)
        {
            var item = GetCarrinhoItem(carrinhoItemDto.Id);
            if (item != null)
            {
                item.PrecoTotal = carrinhoItemDto.Preco * carrinhoItemDto.Quantidade;
            }

            await GerenciaCarrinhoItensLocalStorageService.SaveCollection(CarrinhoCompraItens);
        }

        protected async Task AtualizaQuantidade_Input(int id)
        {
            await Js.InvokeVoidAsync("TornaBotaoAtualizarQuantidadeVisivel", id, true);
        }

        private void CarrinhoChanged()
        {
            CalculaResumoCarrinhoTotal();
            CarrinhoCompraService.RaiseEventOnCarrinhoCompraChanged(QuantidadeTotal);
        }
    }
}