using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloTarefa;
using eAgenda.Infraestrutura.Compartilhado;
using eAgenda.Infraestrutura.ModuloCompromisso;
using eAgenda.Infraestrutura.ModuloTarefa;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContextoDeDados contextodados;
        private readonly IRepositorioCompromisso repositorioCompromisso;
        private readonly IRepositorioTarefa repositorioTarefa;

        public HomeController()
        {
            contextodados = new ContextoDeDados(true);
            repositorioCompromisso = new RepositorioCompromissoEmArquivo(contextodados);
            repositorioTarefa = new RepositorioTarefaEmArquivo(contextodados);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var compromissos = repositorioCompromisso.SelecionarRegistros().Select
                (c => new { Id = c.Id, Tipo = "Compromisso", Titulo = c.Assunto, Data = c.DataOcorrencia });

            var tarefas = repositorioTarefa.SelecionarTarefas().Select
                (t => new { Id = t.Id, Tipo = "Tarefa", Titulo = t.Titulo, Data = t.DataConclusao != default ? t.DataConclusao : t.DataCriacao });

            var itensDaAgenda = compromissos.Concat(tarefas).OrderBy(x => x.Data).ToList();

            ViewBag.ItensDaAgenda = itensDaAgenda;

            return View();
        }

        [HttpGet("erro")]
        public IActionResult Erro()
        {
            return View();
        }
    }
}
