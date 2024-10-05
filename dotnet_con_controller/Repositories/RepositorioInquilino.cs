using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using MySql.Data.MySqlClient;
namespace dotnet_2.Models;
public class RepositorioInquilino
{
    private readonly Conexion conexion;
    public RepositorioInquilino()
    {
        conexion = new Conexion();
    }
    /**
metodo para obtener todos los inquilinos
*/
    public IList<Inquilino> GetInquilinos()
    {
        var inquilinos = new List<Inquilino>();
        using (var connection = new MySqlConnection(Conexion.GetConnectionString()))
        {
            //     var sql = @$"SELECT {nameof(Inquilino.id)},{nameof(Inquilino.nombre)},{nameof(Inquilino.apellido)},{nameof(Inquilino.dni)},{nameof(Inquilino.telefono)},{nameof(Inquilino.email)} FROM inquilino WHERE {nameof(Inquilino.estado)} = 1;";
            string sql = @$"SELECT {nameof(Inquilino.id)},{nameof(Inquilino.nombre)},{nameof(Inquilino.apellido)},{nameof(Inquilino.dni)},{nameof(Inquilino.telefono)},{nameof(Inquilino.email)}";
            sql += " FROM inquilino ";
            sql += @$"WHERE {nameof(Inquilino.borrado)} = 0";
            sql += @$" ORDER BY {nameof(Inquilino.nombre)} ASC;";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        inquilinos.Add(new Inquilino
                        {
                            id = reader.GetInt32(nameof(Inquilino.id)),
                            nombre = reader.GetString(nameof(Inquilino.nombre)),
                            apellido = reader.GetString(nameof(Inquilino.apellido)),
                            dni = reader.GetString(nameof(Inquilino.dni)),
                            telefono = reader.GetString(nameof(Inquilino.telefono)),
                            email = reader.GetString(nameof(Inquilino.email)),
                            //email = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.email))) ? null : reader.GetString(nameof(Inquilino.email)),
                            // estado = reader.GetString(nameof(Inquilino.estado))
                        });
                    }
                }
                connection.Close();
            }
        }
        return inquilinos;
    }
    /**
metodo para traer un inquilino
*/
    public Inquilino? GetInquilino(int getId)
    {
        Inquilino? inquilino = null;
        string connectionString = Conexion.GetConnectionString();

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = "SELECT * FROM inquilino WHERE id = @Id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", getId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            id = reader.GetInt32("id"),
                            nombre = reader.GetString("nombre"),
                            apellido = reader.GetString("apellido"),
                            dni = reader.GetString("dni"),
                            email = reader.GetString("email"),
                            telefono = reader.GetString("telefono"),
                            //email = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.email))) ? null : reader.GetString(nameof(Inquilino.email)),
                            //estado = reader.GetString(nameof(Inquilino.estado))
                        };
                    }
                }
                connection.Close();
            }
        }
        return inquilino;
    }
    /**
metodo para guardar un nuevo inquilino en la base de datos
*/
    public bool GuardarInquilino(Inquilino inquilino, int? id)
    {
        bool respuesta = false;
        using (var connection = new MySqlConnection(Conexion.GetConnectionString()))
        {
            String sql = "";
            if (!id.HasValue)
            {
                id = 0;
                sql = @$"INSERT INTO inquilino (`nombre`, `apellido`, `dni`, `email`,`telefono`,`borrado`)  VALUES (@{nameof(Inquilino.nombre)}, @{nameof(Inquilino.apellido)}, @{nameof(Inquilino.dni)}, @{nameof(Inquilino.email)}, @{nameof(Inquilino.telefono)}, '0');";
            }
            else
            {
                sql = @$"UPDATE inquilino SET `nombre` = @{nameof(Inquilino.nombre)}, `apellido` = @{nameof(Inquilino.apellido)}, `dni` = @{nameof(Inquilino.dni)}, `email` = @{nameof(Inquilino.email)}, `telefono` = @{nameof(Inquilino.telefono)} WHERE id = @Id";
            }
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            if (id.HasValue) { command.Parameters.AddWithValue("@Id", id); }
            command.Parameters.AddWithValue($"@{nameof(inquilino.nombre)}", inquilino.nombre);
            command.Parameters.AddWithValue($"@{nameof(inquilino.apellido)}", inquilino.apellido);
            command.Parameters.AddWithValue($"@{nameof(inquilino.dni)}", inquilino.dni);
            command.Parameters.AddWithValue($"@{nameof(inquilino.email)}", inquilino.email);
            command.Parameters.AddWithValue($"@{nameof(inquilino.telefono)}", inquilino.telefono);
            // command.Parameters.AddWithValue($"@{nameof(inquilino.estado)}", inquilino.estado);
            int columnas = command.ExecuteNonQuery();

            if (columnas > 0)
            {
                respuesta = true;
            }
            connection.Close();
        }
        return respuesta;
    }
    public bool EliminarInquilino(int id)
    {
        bool respuesta = false;
        using (var connection = new MySqlConnection(Conexion.GetConnectionString()))
        {
            string sql = "UPDATE inquilino SET borrado = 1, email = UUID(), telefono = UUID()   WHERE id = @Id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                int columnas = command.ExecuteNonQuery();
                if (columnas > 0)
                {
                    respuesta = true;
                }
                connection.Close();
            }
        }
        return respuesta;
    }
    public List<Inquilino> FindInquilinos(string busqueda)
    {
        //        string inquilinos = "";
        var inquilinos = new List<Inquilino>();
        using (var connection = new MySqlConnection(Conexion.GetConnectionString()))
        {
            // string sql ="SELECT obtener_inquilinos_json('"+value+"') AS inquilinos_json;";
            /*
            string sql = "";
            sql += " SELECT ";
            sql += " CONCAT('[', ";
            sql += " GROUP_CONCAT( ";
            sql += " JSON_OBJECT( ";
            sql += " 'id', id, 'nombre', nombre, 'apellido', apellido, 'dni', dni, 'telefono', telefono, 'email', email ) ";
            sql += " ORDER BY nombre ASC ";
            sql += " SEPARATOR ','  ), ']') ";
            sql += " AS result ";
            sql +="  FROM inquilino ";
            sql += " WHERE borrado = 0 ";
            sql += " AND (nombre LIKE CONCAT(' @busqueda, '%') ";
            sql += " OR apellido LIKE CONCAT(' @busqueda, '%') ";
            sql += " OR email LIKE CONCAT(' @busqueda, '%') ";
            sql += " OR telefono LIKE CONCAT(' @busqueda, '%') ";
            sql += " OR dni LIKE CONCAT(' @busqueda, '%'));";
            */
            string sql = @" SELECT 
                            `id`,                                                       
                            `nombre`,
                            `apellido`,
                            `dni`,
                            `telefono`,
                            `email`
                            FROM inquilino 
                            WHERE borrado = 0 
                            AND (nombre LIKE  @busqueda 
                            OR email LIKE  @busqueda 
                            OR apellido LIKE  @busqueda 
                            OR telefono LIKE  @busqueda 
                            OR dni LIKE  @busqueda );";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@busqueda", busqueda + "%");
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inquilinos.Add(new Inquilino
                        {
                            id = reader.GetInt32(nameof(Inquilino.id)),
                            nombre = reader.GetString(nameof(Inquilino.nombre)),
                            apellido = reader.GetString(nameof(Inquilino.apellido)),
                            dni = reader.GetString(nameof(Inquilino.dni)),
                            telefono = reader.GetString(nameof(Inquilino.telefono)),
                            email = reader.GetString(nameof(Inquilino.email)),
                            //email = reader.IsDBNull(reader.GetOrdinal(nameof(Inquilino.email))) ? null : reader.GetString(nameof(Inquilino.email)),
                            // estado = reader.GetString(nameof(Inquilino.estado))
                        });
                    }


                }
                connection.Close();
            }
        }
        return inquilinos;
    }

    public int GuardarInquilinoPost(Inquilino inquilino)
    {
        int id = 0;
        using (var connection = new MySqlConnection(Conexion.GetConnectionString()))
        {
            string sql = @"INSERT INTO 
                            inquilino (`nombre`, `apellido`, `dni`, `email`, `telefono`, `borrado`)
                            VALUES (@Nombre, @Apellido, @DNI, @Email, @Telefono, '0');
                            SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Nombre", inquilino.nombre);
            command.Parameters.AddWithValue("@Apellido", inquilino.apellido);
            command.Parameters.AddWithValue("@DNI", inquilino.dni);
            command.Parameters.AddWithValue("@Email", inquilino.email);
            command.Parameters.AddWithValue("@Telefono", inquilino.telefono);        
            try
            {
                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
            Console.WriteLine("Error al guardar inquilino: " + ex.Message);
            return -1;
            }
        }
        return id;
    }

}