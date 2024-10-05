using System.Diagnostics;
using MySql.Data.MySqlClient;
namespace dotnet_2.Models;

public class RepositorioCiudad
{
    private readonly string conexion;

    public RepositorioCiudad()
    {
        this.conexion = Conexion.GetConnectionString();
    }

    public IList<Ciudad> ObtenerCiudades()
    {
        IList<Ciudad> ciudades = new List<Ciudad>();
        string sql = "listarCiudades";
        using (var connection = new MySqlConnection(conexion))
        {
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ciudad = new Ciudad
                        {
                            id = reader.GetInt32("id"),
                            ciudad = reader.GetString("ciudad")
                        };
                        ciudades.Add(ciudad);
                    }

                }
            }
            return ciudades;
        }
    }
}