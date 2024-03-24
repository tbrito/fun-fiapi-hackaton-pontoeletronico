using Microsoft.Win32;
using Npgsql;

namespace fun_pontoeletronico
{
    public class FuncionarioRepository
    {
        public bool ExisteFuncionario(string email)
        {
            var sqlConnection = Environment.GetEnvironmentVariable("SQLConnectionString");

            using (var conn = new NpgsqlConnection(sqlConnection))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand($"select nome_usuario from usuario where nome_usuario = @Email"))
                {
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("Email", email);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        return string.IsNullOrEmpty(reader.GetString(0)) == false;
                    }
                }
            }

            return true;
        }

        public void Adicionar(string email)
        {
            var sqlConnection = Environment.GetEnvironmentVariable("SQLConnectionString");

            using (var conn = new NpgsqlConnection(sqlConnection))
            {
                conn.Open();
                var id = Guid.NewGuid();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText ="insert into usuario values (@Email)";

                    cmd.Parameters.AddWithValue("Email", email);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
