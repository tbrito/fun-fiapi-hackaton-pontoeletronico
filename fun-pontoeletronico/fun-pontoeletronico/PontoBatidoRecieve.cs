using fun_pontoeletronico.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Newtonsoft.Json;
using Npgsql;
using System.Collections.Generic;
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
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            var registro = JsonConvert.DeserializeObject<RegitroPontos>(mySbMsg);

            using (var conn = new NpgsqlConnection("Server=lanchonetedaruadb.c16om6u44j69.us-east-1.rds.amazonaws.com;Port=5432;Database=fiapihackatonrelatorios;User Id=postgres;Password=QE1muGg0fwsepsH;"))
            {
                conn.Open();
                var id = Guid.NewGuid();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        "insert into registro_ponto values (@Id, @Registro, @Tipo, @UserName)";

                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.AddWithValue("UserName", registro?.Email);
                    cmd.Parameters.AddWithValue("Registro", registro.Registro);
                    cmd.Parameters.AddWithValue("Tipo", "ENTRADA");

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
