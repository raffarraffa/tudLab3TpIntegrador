using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace dotnet_2.Models
{
    public class RepositorioTipoInmueble
    {
        private readonly string conexion;
        public RepositorioTipoInmueble()
        { //esto llama a la misma conexion 
            this.conexion = Conexion.GetConnectionString();
        }
        public IList<TipoInmueble> GetTipoInmuebles()
        {
            var tipoInmuebles = new List<TipoInmueble>();
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = @$"SELECT {nameof(TipoInmueble.id)},{nameof(TipoInmueble.tipo)},{nameof(TipoInmueble.borrado)} FROM tipo_inmueble ;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tipoInmuebles.Add(new TipoInmueble
                            {
                                id = reader.GetInt32(nameof(TipoInmueble.id)),
                                tipo = reader.GetString(nameof(TipoInmueble.tipo)),
                                borrado = reader.GetInt32(nameof(TipoInmueble.borrado))
                            });
                        }
                    }
                    connection.Close();
                }
            }

            return tipoInmuebles;

        }
        public bool AltaTipoInmueble(TipoInmueble tipoInmueble)
        {
            try
            {
                using (var connection = new MySqlConnection(conexion))
                {
                    var sql = @$"INSERT INTO tipo_inmueble({nameof(TipoInmueble.tipo)}) VALUES(@{nameof(TipoInmueble.tipo)});";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue($"@{nameof(TipoInmueble.tipo)}", tipoInmueble.tipo);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar tipo de inmueble: {ex.Message}");
                return false;
            }
        }
        public bool UpdateTipoInmueble(TipoInmueble tipoInmueble)
        {
            try
            {
                using (var connection = new MySqlConnection(conexion))
                {
                    var sql = @$"UPDATE tipo_inmueble SET {nameof(TipoInmueble.tipo)} = @{nameof(TipoInmueble.tipo)},  {nameof(TipoInmueble.borrado)} = @{nameof(TipoInmueble.borrado)} WHERE tipo_inmueble.id = @{nameof(TipoInmueble.id)};";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue($"@{nameof(TipoInmueble.tipo)}", tipoInmueble.tipo);
                        command.Parameters.AddWithValue($"@{nameof(TipoInmueble.borrado)}", tipoInmueble.borrado);
                        command.Parameters.AddWithValue($"@{nameof(TipoInmueble.id)}", tipoInmueble.id);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar tipo de inmueble: {ex.Message}");
                return false;
            }
        }
public TipoInmueble GetTipoInmueble(int id)
{
    TipoInmueble tipoInmueble = null;
    using (var connection = new MySqlConnection(conexion))
    {
        var sql = @$"SELECT {nameof(TipoInmueble.id)}, {nameof(TipoInmueble.tipo)}, {nameof(TipoInmueble.borrado)} FROM tipo_inmueble WHERE {nameof(TipoInmueble.id)} = @Id;";
        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    tipoInmueble = new TipoInmueble
                    {
                        id = reader.GetInt32(nameof(TipoInmueble.id)),                                
                        tipo = reader.GetString(nameof(TipoInmueble.tipo)),
                        borrado = reader.GetInt32(nameof(TipoInmueble.borrado))
                    };
                }
            }
            connection.Close();
        }
    }
    return tipoInmueble;
}

 

    }
}