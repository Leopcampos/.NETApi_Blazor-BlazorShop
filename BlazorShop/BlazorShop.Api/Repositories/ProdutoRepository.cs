﻿using BlazorShop.Api.Context;
using BlazorShop.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorShop.Api.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Produto> GetItem(int id)
        {
            var produto = await _context.Produtos
                .Include(c => c.Categoria)
                .SingleOrDefaultAsync(c => c.Id.Equals(id));

            return produto;
        }

        public async Task<IEnumerable<Produto>> GetItens()
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .ToListAsync();

            return produto;
        }

        public async Task<IEnumerable<Produto>> GetItensPorCategoria(int id)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.CategoriaId.Equals(id)).ToListAsync();

            return produtos;
        }
    }
}