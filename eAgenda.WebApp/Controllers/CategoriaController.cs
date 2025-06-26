using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Infraestrutura.ModuloCategoria;
using eAgenda.Infraestrutura.Compartilhado;
using eAgenda.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using static eAgenda.WebApp.Models.FormularioCategoriaViewModel;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Infraestrutura.ModuloDespesa;

namespace eAgenda.WebApp.Controllers
{
    [Route("categorias")]
    public class CategoriaController : Controller
    {
        private readonly ContextoDeDados contextoDados;
        private readonly IRepositorioCategoria repositorioCategoria;
        private readonly IRepositorioDespesa repositorioDespesa;

        public CategoriaController()
        {
            contextoDados = new ContextoDeDados(true);
            repositorioCategoria = new RepositorioCategoriaEmArquivo(contextoDados);
            repositorioDespesa = new RepositorioDespesaEmArquivo(contextoDados);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var registros = repositorioCategoria.SelecionarRegistros();

            var visualizarVM = new VisualizarCategoriaViewModel(registros);

            return View(visualizarVM);
        }

        [HttpGet("cadastrar")]
        public IActionResult Cadastrar()
        {
            var cadastrarVM = new CadastrarCategoriaViewModel();

            return View(cadastrarVM);
        }

        [HttpPost("cadastrar")]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(CadastrarCategoriaViewModel cadastrarVM)
        {
            var registros = repositorioCategoria.SelecionarRegistros() ?? new List<Categoria>();

            foreach (var item in registros)
            {
                if (item.Titulo.Equals(cadastrarVM.Titulo))
                {
                    ModelState.AddModelError("CadastroUnico", "Já existe uma Categoria registrada com este Título.");
                    return View(cadastrarVM);
                }
            }

            var entidade = cadastrarVM.ParaEntidade();

            repositorioCategoria.CadastrarRegistro(entidade);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("editar/{id:guid}")]
        public IActionResult Editar(Guid id)
        {
            var registroSelecionado = repositorioCategoria.SelecionarRegistroPorId(id);

            var editarVM = new EditarCategoriaViewModel(
                id,
                registroSelecionado.Titulo
            );

            return View(editarVM);
        }

        [HttpPost("editar/{id:guid}")]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Guid id, EditarCategoriaViewModel editarVM)
        {
            var registros = repositorioCategoria.SelecionarRegistros();
            Categoria c = repositorioCategoria.SelecionarRegistroPorId(id);
            foreach (var item in registros)
            {
                if (!item.Id.Equals(id) && item.Titulo.Equals(editarVM.Titulo))
                {
                    ModelState.AddModelError("CadastroUnico", "Já existe uma categoria registrada com este título.");
                    return View(editarVM);
                }
            }
            if (c.idDespesas == null) c.idDespesas = new List<Guid>();
            else if(c.idDespesas.Count > 0)
            {
                foreach (var item in c.idDespesas)    
                {
                    Despesa d = repositorioDespesa.SelecionarRegistroPorId(item);
                    d.categoriasTitulo.Remove(repositorioCategoria.SelecionarRegistroPorId(c.Id).Titulo);
                    d.categoriasTitulo.Add(editarVM.Titulo);
                    repositorioDespesa.EditarRegistro(d.Id, d);
                }
            }
            editarVM.despesas = c.despesas;
            editarVM.idDespesas = c.idDespesas;

            var entidadeEditada = editarVM.ParaEntidade();
            entidadeEditada.Id = id;
            repositorioCategoria.EditarRegistro(id, entidadeEditada);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("excluir/{id:guid}")]
        public IActionResult Excluir(Guid id)
        {
            var registroSelecionado = repositorioCategoria.SelecionarRegistroPorId(id);

            var excluirVM = new ExcluirCategoriaViewModel(
                id,
                registroSelecionado.Titulo
                );

            return View(excluirVM);
        }

        [HttpPost("excluir/{id:guid}")]
        public IActionResult Excluir(Guid id, ExcluirCategoriaViewModel excluirVM)
        {
            var registros = repositorioCategoria.SelecionarRegistros();

            foreach (var item in registros)
            {
                if (item.idDespesas == null)
                {
                    item.idDespesas = new List<Guid>();
                    continue;
                }
                else if (item.Id.Equals(id) && item.idDespesas.Count > 0)
                {
                    ModelState.AddModelError("ExclusaoProibida", "Não é possível excluir uma categoria que possui despesas associadas");
                    return View(excluirVM);
                }
                
            }

            repositorioCategoria.ExcluirRegistro(id);

            return RedirectToAction(nameof(Index));
        }
        [HttpGet("despesas/{id:guid}")]
        public IActionResult Despesas(Guid id)
        {
            var registro = repositorioCategoria.SelecionarRegistroPorId(id);
            if (registro.idDespesas == null)
            {
                registro.despesas = new List<Despesa>();
                registro.idDespesas = new List<Guid>();
            }  
            foreach (var item in registro.idDespesas)
            {
                
                registro.despesas!.Add(repositorioDespesa.SelecionarRegistroPorId(item));
            }

            var visualizarVM = new VisualizarCategoriaDespesaViewModel(registro);

            return View(visualizarVM);
        }
        [HttpPost("despesas/{id:guid}")]
        public IActionResult Despesas(Guid id, VisualizarCategoriaDespesaViewModel visualizarVM)
        {
           return View(visualizarVM);
        }

        [HttpGet("despesas/{categoriaId:guid}/excluir/{despesaId:guid}")]
        public IActionResult ExcluirDespesa(Guid categoriaId, Guid despesaId)
        {
            var registroSelecionado = repositorioCategoria.SelecionarRegistroPorId(categoriaId);
            var despesaSelecionada = repositorioDespesa.SelecionarRegistroPorId(despesaId);

            var ExcluirVM = new ExcluirCategoriaDespesaViewModel( registroSelecionado.Id, registroSelecionado.Titulo, despesaId);
            
            return View(ExcluirVM);
        }

        [HttpPost("despesas/{categoriaId:guid}/excluir/{despesaId:guid}")]
        public IActionResult ExcluirDespesa(Guid categoriaId, Guid despesaId, ExcluirCategoriaDespesaViewModel ExcluirVM)
        {
            var despesa = repositorioDespesa.SelecionarRegistroPorId(despesaId);
            var categoria = repositorioCategoria.SelecionarRegistroPorId(categoriaId);
            if (despesa.categorias.Count == 1)
            {
                ModelState.AddModelError("ExclusaoProibida", "Não é possível existir uma despesa sem uma categoria");
            }

            foreach (var item in repositorioDespesa.SelecionarRegistros())
            {
                if(item.Id == despesaId)
                {
                    item.categorias.Remove(categoriaId);
                    item.categoriasTitulo.Remove(categoria.Titulo);
                    repositorioDespesa.EditarRegistro(despesaId, item);
                }
            }

            categoria.idDespesas.Remove(despesa.Id);
            repositorioCategoria.EditarRegistro(categoriaId, categoria);

            return RedirectToAction(nameof(Index));
        }
    }
}
