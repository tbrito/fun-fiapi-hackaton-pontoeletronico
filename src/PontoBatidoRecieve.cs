using fun_pontoeletronico.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Newtonsoft.Json;
using Npgsql;
using System.Collections.Generic;
using System.Configuration;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace fun_pontoeletronico
{
    public class PontoBatidoRecieve
    {
        private readonly ILogger _logger;

        public PontoBatidoRecieve(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PontoBatidoRecieve>();
        }

        [Function("PontoBatidoRecieve")]
        public void Run([ServiceBusTrigger("grupo23-pontobatido", "pontoeletronico-sub")] string mySbMsg)
        {
            _logger.LogInformation("Mensagem Recebida: " + mySbMsg);

            var registro = JsonConvert.DeserializeObject<RegitroPontos>(mySbMsg);

            if (new FuncionarioRepository().ExisteFuncionario(registro.Email) == false)
            {
                _logger.LogInformation("Funcionario ainda não existe e será inserido: " + mySbMsg);
                new FuncionarioRepository().Adicionar(registro.Email);
            }

            var totalpontos = new RegistroPontoRepository().TotalPontosDoDia(registro.Email);

            registro.Tipo = (totalpontos % 2 == 0 || totalpontos == 0) ? "ENTRADA" : "SAIDA";
            new RegistroPontoRepository().Registrar(registro);

            _logger.LogInformation("Mensagem Inserida para consulta espelho");
        }
    }
}
