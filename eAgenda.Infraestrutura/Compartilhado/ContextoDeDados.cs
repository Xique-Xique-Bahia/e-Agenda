using eAgenda.Dominio.ModuloCategoria;
using eAgenda.Dominio.ModuloCompromisso;
using eAgenda.Dominio.ModuloContato;
using eAgenda.Dominio.ModuloDespesa;
using eAgenda.Dominio.ModuloTarefa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace eAgenda.Infraestrutura.Compartilhado
{
    public class ContextoDeDados
    {
        private string pastaArmazenamento = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "eAgenda"
        );
        private string arquivoArmazenamento = "dados.json";

        public List<Contato> Contatos { get; set; }
        public List<Compromisso> Compromissos { get; set; }
        public List<Categoria> Categorias { get; set; }
        public List<Despesa> Despesas { get; set; }
        public List<Tarefa> Tarefas { get; set; }

        public ContextoDeDados()
        {
            Contatos = new List<Contato>();
            Compromissos = new List<Compromisso>();
            Categorias = new List<Categoria>();
            Despesas = new List<Despesa>();
            Tarefas = new List<Tarefa>();
        }
        public ContextoDeDados(bool carregarDados) : this()
        {
            if (carregarDados)
                Carregar();
        }

        public void Salvar()
        {
            string caminhoCompleto = Path.Combine(pastaArmazenamento, arquivoArmazenamento);

            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.WriteIndented = true;
            jsonOptions.ReferenceHandler = ReferenceHandler.Preserve;

            string json = JsonSerializer.Serialize(this, jsonOptions);

            if (!Directory.Exists(pastaArmazenamento))
                Directory.CreateDirectory(pastaArmazenamento);

            File.WriteAllText(caminhoCompleto, json);
        }

        public void Carregar()
        {
            string caminhoCompleto = Path.Combine(pastaArmazenamento, arquivoArmazenamento);

            if (!File.Exists(caminhoCompleto)) return;

            string json = File.ReadAllText(caminhoCompleto);

            if (string.IsNullOrWhiteSpace(json)) return;

            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.ReferenceHandler = ReferenceHandler.Preserve;

            ContextoDeDados contextoArmazenado = JsonSerializer.Deserialize<ContextoDeDados>(
                json,
                jsonOptions
            )!;

            if (contextoArmazenado == null) return;

            Contatos = contextoArmazenado.Contatos;
            Compromissos = contextoArmazenado.Compromissos;
            Categorias = contextoArmazenado.Categorias;
            Despesas = contextoArmazenado.Despesas;
            Tarefas = contextoArmazenado.Tarefas;
        }
    }
}
