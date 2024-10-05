using System.Collections.Generic;
using System.Data;
using System.Security.Policy;
using dotnet_2.Servicios;
using MySql.Data.MySqlClient;
namespace dotnet_2.Models
{
    public class RepositorioUsuario
    {
        private readonly string conexion;
        public RepositorioUsuario()
        {
            this.conexion = Conexion.GetConnectionString();
        }

        public bool GuardarUsuario(Usuario? usuario)
        {
            bool respuesta = false;
            string pass = HashPass.HashearPass(usuario.password);
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = @$"INSERT INTO usuario (`nombre`, `apellido`, `dni`, `email`, `password`, `rol`, `avatarUrl`,`borrado`) 
VALUES (@{nameof(Usuario.nombre)}, @{nameof(Usuario.apellido)}, @{nameof(Usuario.dni)}, @{nameof(Usuario.email)}, @pass ,@{nameof(Usuario.rol)},@{nameof(Usuario.avatarUrl)},0)";
                using var command = new MySqlCommand(sql, connection);
                connection.Open();
                command.Parameters.AddWithValue($"@{nameof(Usuario.nombre)}", usuario.nombre);
                command.Parameters.AddWithValue($"@{nameof(Usuario.apellido)}", usuario.apellido);
                command.Parameters.AddWithValue($"@{nameof(Usuario.dni)}", usuario.dni);
                command.Parameters.AddWithValue($"@{nameof(Usuario.email)}", usuario.email);
                command.Parameters.AddWithValue("@pass", pass);
                command.Parameters.AddWithValue($"@{nameof(Usuario.rol)}", usuario.rol);
                command.Parameters.AddWithValue($"@{nameof(Usuario.avatarUrl)}", usuario.avatarUrl);
                int columnas = command.ExecuteNonQuery(); //se ejecuta la consulta
                //si columnas es mayor a 0 entonces la consulta fue correcta
                if (columnas > 0)
                {
                    respuesta = true;
                }
                connection.Close();
            }
            return respuesta;
        }

        public Usuario? GetUsuario(int id)
        {
            Usuario? usuario = null;
            using (var connection = new MySqlConnection(conexion))
            {

                var sql = @"SELECT id, nombre, apellido, dni, email, rol, password, avatarUrl FROM usuario WHERE id = @Id;";
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            id = reader.IsDBNull("id") ? 0 : reader.GetInt32("id"),
                            nombre = reader.IsDBNull("nombre") ? null : reader.GetString("nombre"),
                            apellido = reader.IsDBNull("apellido") ? null : reader.GetString("apellido"),
                            dni = reader.IsDBNull("dni") ? null : reader.GetString("dni"),
                            email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                            rol = reader.IsDBNull("rol") ? null : reader.GetString("rol"),
                            password = reader.IsDBNull("password") ? null : reader.GetString("password"),
                            avatarUrl = reader.IsDBNull("avatarUrl") ? null : reader.GetString("avatarUrl")
                        };
                    }
                }
                connection.Close();
            }
            return usuario;
        }

        public bool CompararPassword(string password, string email)
        {
            Usuario? usuario = GetUsuarioPorEmail(email);
            if (usuario == null)
            {
                return false;
            }
            bool respuesta = HashPass.VerificarPassword(password, usuario.password);
            return respuesta;
        }

        public Usuario? GetUsuarioPorEmail(string email)
        {
            Usuario? usuario = null;
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = @"SELECT id, nombre, apellido, dni, email, password, rol, avatarUrl FROM usuario WHERE email = @email;";
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            id = reader.IsDBNull("id") ? 0 : reader.GetInt32("id"),
                            nombre = reader.IsDBNull("nombre") ? null : reader.GetString("nombre"),
                            apellido = reader.IsDBNull("apellido") ? null : reader.GetString("apellido"),
                            dni = reader.IsDBNull("dni") ? null : reader.GetString("dni"),
                            email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                            password = reader.IsDBNull("password") ? null : reader.GetString("password"),
                            rol = reader.IsDBNull("rol") ? null : reader.GetString("rol"),
                            avatarUrl = reader.IsDBNull("avatarUrl") ? null : reader.GetString("avatarUrl")
                        };
                    }
                }
                connection.Close();
            }
            return usuario;
        }

        public bool ActualizarUsuario(Usuario usuario)
        {      
            bool respuesta = false;
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = @"UPDATE usuario SET nombre = @nombre, apellido = @apellido, dni = @dni, email = @email, rol = @rol, password = @password, avatarUrl = @avatarUrl WHERE id = @id;";
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", usuario.id);
                command.Parameters.AddWithValue("@nombre", usuario.nombre);
                command.Parameters.AddWithValue("@apellido", usuario.apellido);
                command.Parameters.AddWithValue("@dni", usuario.dni);
                command.Parameters.AddWithValue("@email", usuario.email);
                command.Parameters.AddWithValue("@rol", usuario.rol);
                command.Parameters.AddWithValue("@password", usuario.password);
                command.Parameters.AddWithValue("@avatarUrl", usuario.avatarUrl);
                connection.Open();
                int columnas = command.ExecuteNonQuery();
                if (columnas > 0)
                {
                    respuesta = true;
                }
                connection.Close();
            }
            return respuesta;
        }

        public IList<Usuario> ObtenerUsuarios()
        {
            IList<Usuario> usuarios = new List<Usuario>();
            string sql = "listarUsuarios";
            using (var connection = new MySqlConnection(conexion))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var usuario = new Usuario
                            {
                                id = reader.GetInt32("id"),
                                nombre = reader.GetString("nombre"),
                                apellido = reader.GetString("apellido"),
                                dni = reader.GetString("dni"),
                                email = reader.GetString("email"),
                                rol = reader.GetString("rol"),
                                avatarUrl = reader.IsDBNull("avatarUrl") ? null : reader.GetString("avatarUrl"),
                                borrado = reader.GetBoolean("borrado")
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
                connection.Close();
            }
            return usuarios;

        }

        public bool EliminarUsuario(int id)
        {
            bool respuesta = false;
            using (var connection = new MySqlConnection(conexion))
            {
                connection.Open();
                var sql = @"UPDATE usuario SET borrado = 1 WHERE id = @id;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    int columnas = command.ExecuteNonQuery();
                    respuesta = columnas > 0;
                }
                connection.Close();
            }
            return respuesta;
        }

        public bool CambiarPassword(string email, string password)
        {
            bool respuesta = false;
            using (var connection = new MySqlConnection(conexion))
            {
                connection.Open();
                var sql = @"UPDATE usuario SET password = @pass WHERE email = @email;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@pass", password);
                    command.Parameters.AddWithValue("@email", email);
                    int columnas = command.ExecuteNonQuery();
                    respuesta = columnas > 0;
                }
                connection.Close();
            }
            return respuesta;
        }

        //busca un usuario segun la contraseña
        public Usuario? BuscarUsuarioPorPassword(string password)
        {
            Usuario? usuario = null;
            using (var connection = new MySqlConnection(conexion))
            {
                connection.Open();
                var sql = @"SELECT * FROM usuario WHERE password = @password;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@password", password);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                id = Convert.ToInt32(reader["id"]),
                                nombre = Convert.ToString(reader["nombre"]),
                                apellido = Convert.ToString(reader["apellido"]),
                                dni = Convert.ToString(reader["dni"]),
                                email = Convert.ToString(reader["email"]),
                                password = Convert.ToString(reader["password"]),
                                rol = Convert.ToString(reader["rol"]),
                                avatarUrl = Convert.ToString(reader["avatarUrl"]),
                                borrado = Convert.ToBoolean(reader["borrado"])
                            };
                        }
                    }
                }
                connection.Close();
            }
            return usuario;
        }


    }

}