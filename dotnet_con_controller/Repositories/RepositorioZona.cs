using System.Diagnostics;
using MySql.Data.MySqlClient;
namespace dotnet_2.Models;

public class RepositorioZona
{
    private readonly string conexion;

    public RepositorioZona()
    {
        this.conexion = Conexion.GetConnectionString();
    }

    public IList<Zona> ListarZonas()
    {
        IList<Zona> zonas = new List<Zona>();
        string sql = "listarZonas";
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
                        var zona = new Zona
                        {
                            id = reader.GetInt32("id"),
                            zona = reader.GetString("zona")
                        };
                        zonas.Add(zona);
                    }

                }
            }
            return zonas;
        }
    }
}