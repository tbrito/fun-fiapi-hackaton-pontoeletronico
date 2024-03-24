using fun_pontoeletronico.Model;
using Microsoft.Win32;
using Npgsql;

namespace fun_pontoeletronico
{
    public  class RegistroPontoRepository
    {
        public void Registrar(RegitroPontos registro)
        {
            var sqlConnection = Environment.GetEnvironmentVariable("SQLConnectionString");

            using (var conn = new NpgsqlConnection(sqlConnection))
            {
                conn.Open();
                var id = Guid.NewGuid();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "insert into registro_ponto values (@Id, @Registro, @Tipo, @UserName)";

                    cmd.Parameters.AddWithValue("Id", id);
                    cmd.Parameters.AddWithValue("UserName", registro?.Email);
                    cmd.Parameters.AddWithValue("Registro", registro.Registro);
                    cmd.Parameters.AddWithValue("Tipo", registro.Tipo);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int TotalPontosDoDia(string email)
        {
            var sqlConnection = Environment.GetEnvironmentVariable("SQLConnectionString");

            using (var conn = new NpgsqlConnection(sqlConnection))
            {
                var sql = $@"
select 
    extract('MONTH' from horario), extract('DAY' from horario), count(0)
from 
    registro_ponto 
where 
    usuario = '{email}'
    and extract('DAY' from horario) = '{DateTime.Now.Day}'
    and extract('MONTH' from horario) = '{DateTime.Now.Month}'
group by 
    extract('MONTH' from horario), extract('DAY' from horario)";

                conn.Open();
                using (var cmd = new NpgsqlCommand(sql))
                {
                    cmd.Connection = conn;
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        return reader.GetInt32(2);
                    }
                }
            }

            return 1;
        }
    }
}
