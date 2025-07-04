﻿using eAgenda.Dominio.ModuloCategoria;
using eAgenda.WebApp.Models;
using static eAgenda.WebApp.Models.FormularioCategoriaViewModel;

namespace eAgenda.WebApp.Extensions
{
    public static class CategoriaExtensions
    {
        public static Categoria ParaEntidade(this FormularioCategoriaViewModel formularioVM)
        {
            return new Categoria(formularioVM.Titulo, formularioVM.idDespesas, formularioVM.despesas);
        }

        public static DetalhesCategoriaViewModel ParaDetalhesVM(this Categoria categoria)
        {
            return new DetalhesCategoriaViewModel(
                categoria.Id,
                categoria.Titulo,
                categoria.despesas,
                categoria.idDespesas
            );
        }
    }
}
