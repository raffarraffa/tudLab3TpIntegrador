using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace dotnet_2.Models
{
    public class RepositorioInmueble
    {
        private readonly string conexion;
        public RepositorioInmueble()
        {
            //esto llama a la misma conexion 
            this.conexion = Conexion.GetConnectionString();
            // this.conexion = Conexion.GetConnectionStringRemota();
        }

        public IList<Inmueble> GetInmuebles(int page, int pageSize, string usoInmueble = "", string precioInmueble = "", string tipoInmueble = "", string ciudadInmueble = "", string zonaInmueble = "", string fechaInicioPedida = "", string fechaFinPedida = "") //paginado + filtros
        {
            var inmuebles = new List<Inmueble>();
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = @$"                              
                    SELECT 
                        i.id,
                        i.direccion,
                        i.uso,
                        i.id_tipo,
                        i.ambientes,
                        i.coordenadas,                        
                        i.precio,
                        i.id_propietario,
                        i.estado,
                        i.id_ciudad,
                        i.id_zona,
                        i.borrado,
                        i.descripcion,
                        t.id AS t_id_tipo,
                        t.tipo AS tipo_inmueble,
                        p.id AS p_id,
                        p.nombre AS nombre_propietario,
                        p.apellido AS apellido_propietario,
                        c.ciudad,
                        z.zona
                    FROM
                        inmueble AS i
                        INNER JOIN tipo_inmueble AS t 
                            ON i.id_tipo = t.id
                        INNER JOIN propietario AS p 
                            ON i.id_propietario = p.id
                        JOIN ciudad AS c
                            ON c.id = i.id_ciudad
                        JOIN zona AS z 
                            ON z.id = i.id_zona                        
                    WHERE i.id NOT in (
									SELECT con.id_inmueble from contrato as con where con.fecha_inicio < @fechaFinPedida
                                    AND 
                                    con.fecha_efectiva > @fechaInicioPedida) 
                                    AND i.borrado = 0

                    ";

                if (zonaInmueble != "") { sql += " AND i.id_zona = @zonaInmueble "; }
                if (ciudadInmueble != "") { sql += " AND i.id_ciudad = @ciudadInmueble "; }
                if (tipoInmueble != "") { sql += " AND i.id_tipo = @tipoInmueble "; }
                if (precioInmueble != "") { sql += " AND i.precio <= @precioInmueble "; }
                if (usoInmueble != "") { sql += " AND i.uso = @usoInmueble "; }
                sql += " ORDER BY i.id LIMIT @PageSize OFFSET @Offset ";
                sql += " ;";          
                using (var command = new MySqlCommand(sql, connection))
                {
                    int offset = (page - 1) * pageSize;
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@fechaInicioPedida", fechaInicioPedida);
                    command.Parameters.AddWithValue("@fechaFinPedida", fechaFinPedida);
                    if (usoInmueble != null) { command.Parameters.AddWithValue("@usoInmueble", usoInmueble); }
                    if (precioInmueble != null) { command.Parameters.AddWithValue("@precioInmueble", precioInmueble); }
                    if (tipoInmueble != null) { command.Parameters.AddWithValue("@tipoInmueble", tipoInmueble); }
                    if (ciudadInmueble != null) { command.Parameters.AddWithValue("@ciudadInmueble", ciudadInmueble); }
                    if (zonaInmueble != null) { command.Parameters.AddWithValue("@zonaInmueble", zonaInmueble); }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {

                                id = reader.GetInt32("id"),
                                direccion = reader.GetString("direccion"),
                                uso = Enum.TryParse<UsoDeInmueble>(reader.GetString("uso"), out UsoDeInmueble usoEnum) ? usoEnum : UsoDeInmueble.Residencial,
                                estado = Enum.TryParse<EstadoInmueble>(reader.GetString("estado"), out EstadoInmueble estadoEnum) ? estadoEnum : EstadoInmueble.Disponible,
                                //estado = reader.GetString("estado"),
                                id_tipo = reader.GetInt32("id_tipo"),
                                ambientes = reader.GetInt32("ambientes"),
                                coordenadas = reader.GetString("coordenadas"),
                                precio = reader.GetDecimal("precio"),
                                id_propietario = reader.GetInt32("id_propietario"),
                                id_ciudad = reader.GetInt32("id_ciudad"),
                                id_zona = reader.GetInt32("id_zona"),
                                borrado = reader.GetBoolean("borrado"),
                                descripcion = reader.GetString("descripcion"),
                                tipoInmueble = new TipoInmueble
                                {
                                    id = reader.GetInt32("t_id_tipo"),
                                    tipo = reader.GetString("tipo_inmueble")
                                },
                                propietario = new Propietario
                                {
                                    id = reader.GetInt32("p_id"),
                                    nombre = reader.GetString("nombre_propietario"),
                                    apellido = reader.GetString("apellido_propietario")
                                },
                                ciudad = new Ciudad
                                {
                                    id = reader.GetInt32("id_ciudad"),
                                    ciudad = reader.GetString("ciudad")
                                },
                                zona = new Zona
                                {
                                    id = reader.GetInt32("id_zona"),
                                    zona = reader.GetString("zona")
                                }
                            });
                        }
                        connection.Close();
                    }

                }
            }

            return inmuebles;
        }

        public IList<Inmueble> GetInmuebles(int page, int pageSize)// solo paginado
        {
            var inmuebles = new List<Inmueble>();
            using (var connection = new MySqlConnection(conexion))
            {
                string sql = "SELECT i.id, i.direccion, i.uso, i.id_tipo, i.ambientes, i.coordenadas, i.precio, i.id_propietario, i.estado, i.id_ciudad, i.id_zona, i.borrado, i.descripcion, ";
                sql += " t.id AS t_id_tipo , t.tipo AS tipo_inmueble, ";
                sql += " p.id AS p_id, p.nombre AS nombre_propietario, p.apellido AS apellido_propietario, ";
                sql += " c.ciudad , z.zona ";
                sql += "FROM inmueble AS i ";
                sql += "INNER JOIN tipo_inmueble AS t ";
                sql += "ON i.id_tipo = t.id ";
                sql += "INNER JOIN propietario AS p ";
                sql += "ON i.id_propietario = p.id ";
                sql += "JOIN ciudad AS c ";
                sql += "ON c.id = i.id_ciudad ";
                sql += "JOIN zona AS z ";
                sql += "ON z.id = i.id_zona ";
                sql += "LIMIT @PageSize OFFSET @Offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    int offset = (page - 1) * pageSize;
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@Offset", offset);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {

                                id = reader.GetInt32("id"),
                                direccion = reader.GetString("direccion"),
                                uso = Enum.TryParse<UsoDeInmueble>(reader.GetString("uso"), out UsoDeInmueble usoEnum) ? usoEnum : UsoDeInmueble.Residencial,
                                estado = Enum.TryParse<EstadoInmueble>(reader.GetString("estado"), out EstadoInmueble estadoEnum) ? estadoEnum : EstadoInmueble.Disponible,
                                id_tipo = reader.GetInt32("id_tipo"),
                                ambientes = reader.GetInt32("ambientes"),
                                coordenadas = reader.GetString("coordenadas"),
                                precio = reader.GetDecimal("precio"),
                                id_propietario = reader.GetInt32("id_propietario"),
                                id_ciudad = reader.GetInt32("id_ciudad"),
                                id_zona = reader.GetInt32("id_zona"),
                                borrado = reader.GetBoolean("borrado"),
                                descripcion = reader.GetString("descripcion"),
                                tipoInmueble = new TipoInmueble
                                {
                                    id = reader.GetInt32("t_id_tipo"),
                                    tipo = reader.GetString("tipo_inmueble")
                                },
                                propietario = new Propietario
                                {
                                    id = reader.GetInt32("p_id"),
                                    nombre = reader.GetString("nombre_propietario"),
                                    apellido = reader.GetString("apellido_propietario")
                                },
                                ciudad = new Ciudad
                                {
                                    id = reader.GetInt32("id_ciudad"),
                                    ciudad = reader.GetString("ciudad")
                                },
                                zona = new Zona
                                {
                                    id = reader.GetInt32("id_zona"),
                                    zona = reader.GetString("zona")
                                }


                            });
                        }
                        connection.Close();
                    }
                }
            }

            return inmuebles;
        }
        public IList<Inmueble> GetInmuebles(int? id = null)
        {
            var inmuebles = new List<Inmueble>();
            using (var connection = new MySqlConnection(conexion))
            {
                string sql = "SELECT i.id, i.direccion, i.uso, i.id_tipo, i.ambientes, i.coordenadas, i.precio, i.id_propietario, i.estado, i.id_ciudad, i.id_zona, i.borrado, i.descripcion, ";
                sql += " t.id AS t_id_tipo , t.tipo AS tipo_inmueble, ";
                sql += " p.id AS p_id, p.nombre AS nombre_propietario, p.apellido AS apellido_propietario, ";
                sql += " c.ciudad , z.zona ";
                sql += "FROM inmueble AS i ";
                sql += "INNER JOIN tipo_inmueble AS t ";
                sql += "ON i.id_tipo = t.id ";
                sql += "INNER JOIN propietario AS p ";
                sql += "ON i.id_propietario = p.id ";
                sql += "JOIN ciudad AS c ";
                sql += "ON c.id = i.id_ciudad ";
                sql += "JOIN zona AS z ";
                sql += "ON z.id = i.id_zona ";
                sql += "WHERE i.id_propietario = " + id;
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {

                                id = reader.GetInt32("id"),
                                direccion = reader.GetString("direccion"),
                                uso = Enum.TryParse<UsoDeInmueble>(reader.GetString("uso"), out UsoDeInmueble usoEnum) ? usoEnum : UsoDeInmueble.Residencial,
                                estado = Enum.TryParse<EstadoInmueble>(reader.GetString("estado"), out EstadoInmueble estadoEnum) ? estadoEnum : EstadoInmueble.Disponible,
                                id_tipo = reader.GetInt32("id_tipo"),
                                ambientes = reader.GetInt32("ambientes"),
                                coordenadas = reader.GetString("coordenadas"),
                                precio = reader.GetDecimal("precio"),
                                id_propietario = reader.GetInt32("id_propietario"),
                                id_ciudad = reader.GetInt32("id_ciudad"),
                                id_zona = reader.GetInt32("id_zona"),
                                borrado = reader.GetBoolean("borrado"),
                                descripcion = reader.GetString("descripcion"),
                                tipoInmueble = new TipoInmueble
                                {
                                    id = reader.GetInt32("t_id_tipo"),
                                    tipo = reader.GetString("tipo_inmueble")
                                },
                                propietario = new Propietario
                                {
                                    id = reader.GetInt32("p_id"),
                                    nombre = reader.GetString("nombre_propietario"),
                                    apellido = reader.GetString("apellido_propietario")
                                },
                                ciudad = new Ciudad
                                {
                                    id = reader.GetInt32("id_ciudad"),
                                    ciudad = reader.GetString("ciudad")
                                },
                                zona = new Zona
                                {
                                    id = reader.GetInt32("id_zona"),
                                    zona = reader.GetString("zona")
                                }


                            });
                        }
                        connection.Close();
                    }
                }
            }

            return inmuebles;
        }
        public int GetTotalInmuebles(string usoInmueble = "", string precioInmueble = "", string tipoInmueble = "", string ciudadInmueble = "", string zonaInmueble = "")
        {
            int total = 0;
            using (var connection = new MySqlConnection(conexion))
            {
                string sql = "SELECT COUNT(*) FROM inmueble ";
                sql += " WHERE borrado = 0  AND estado =\"Disponible\" ";
                if (zonaInmueble != "") { sql += " AND id_zona = @zonaInmueble "; }
                if (ciudadInmueble != "") { sql += " AND id_ciudad = @ciudadInmueble "; }
                if (tipoInmueble != "") { sql += " AND id_tipo = @tipoInmueble "; }
                if (precioInmueble != "") { sql += " AND precio <= @precioInmueble "; }
                if (usoInmueble != "") { sql += " AND uso = @usoInmueble "; }



                using (var command = new MySqlCommand(sql, connection))
                {
                    if (usoInmueble != null) { command.Parameters.AddWithValue("@usoInmueble", usoInmueble); }
                    if (precioInmueble != null) { command.Parameters.AddWithValue("@precioInmueble", precioInmueble); }
                    if (tipoInmueble != null) { command.Parameters.AddWithValue("@tipoInmueble", tipoInmueble); }
                    if (ciudadInmueble != null) { command.Parameters.AddWithValue("@ciudadInmueble", ciudadInmueble); }
                    if (zonaInmueble != null) { command.Parameters.AddWithValue("@zonaInmueble", zonaInmueble); }

                    connection.Open();
                    total = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }

            return total;
        }
        public Inmueble? GetInmueble(int id)
        {
            Inmueble? inmueble = null;
            using (var connection = new MySqlConnection(conexion))
            {
                string sql = "SELECT i.id, i.direccion, i.uso, i.id_tipo, i.ambientes, i.coordenadas, i.precio, i.id_propietario, i.estado, i.id_ciudad, i.id_zona, i.borrado, i.descripcion, ";
                sql += " t.id AS t_id_tipo, t.tipo AS tipo_inmueble, ";
                sql += " p.id AS p_id, p.nombre AS nombre_propietario, p.apellido AS apellido_propietario ";
                sql += "FROM inmueble AS i ";
                sql += "INNER JOIN tipo_inmueble AS t ";
                sql += "ON i.id_tipo = t.id ";
                sql += "INNER JOIN propietario AS p ";
                sql += "ON i.id_propietario = p.id ";
                sql += "WHERE i.id = @id";


                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                id = reader.GetInt32("id"),
                                direccion = reader.GetString("direccion"),
                                uso = Enum.TryParse<UsoDeInmueble>(reader.GetString("uso"), out UsoDeInmueble usoEnum) ? usoEnum : UsoDeInmueble.Residencial,
                                estado = Enum.TryParse<EstadoInmueble>(reader.GetString("estado"), out EstadoInmueble estadoEnum) ? estadoEnum : EstadoInmueble.Disponible,
                                id_tipo = reader.GetInt32("id_tipo"),
                                ambientes = reader.GetInt32("ambientes"),
                                coordenadas = reader.GetString("coordenadas"),
                                precio = reader.GetDecimal("precio"),
                                id_propietario = reader.GetInt32("id_propietario"),
                                id_ciudad = reader.GetInt32("id_ciudad"),
                                id_zona = reader.GetInt32("id_zona"),
                                borrado = reader.GetBoolean("borrado"),
                                descripcion = reader.GetString("descripcion"),
                                tipoInmueble = new TipoInmueble
                                {
                                    id = reader.GetInt32("t_id_tipo"),
                                    tipo = reader.GetString("tipo_inmueble")
                                },
                                propietario = new Propietario
                                {
                                    id = reader.GetInt32("p_id"),
                                    nombre = reader.GetString("nombre_propietario"),
                                    apellido = reader.GetString("apellido_propietario")
                                }
                            };
                        }
                        connection.Close();
                    }
                }
            }

            return inmueble;
        }

        public bool GuardarInmueble(Inmueble inmueble,string userId)
        {
            bool respuesta = false;
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = $"INSERT INTO inmueble (`direccion`, `uso`, `id_tipo`, `ambientes`, `coordenadas`, `precio`, `id_propietario`, `estado`, `id_ciudad`, `id_zona`, `descripcion`) VALUES (@{nameof(Inmueble.direccion)}, @{nameof(Inmueble.uso)},@{nameof(Inmueble.id_tipo)}, @{nameof(Inmueble.ambientes)}, @{nameof(Inmueble.coordenadas)}, @{nameof(Inmueble.precio)}, @{nameof(Inmueble.id_propietario)}, @{nameof(Inmueble.estado)}, @{nameof(Inmueble.id_ciudad)}, @{nameof(Inmueble.id_zona)}, @{nameof(Inmueble.descripcion)})";
                using var command = new MySqlCommand(sql, connection);
                connection.Open();
                command.Parameters.AddWithValue($"@{nameof(Inmueble.direccion)}", inmueble.direccion);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.uso)}", inmueble.uso);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.id_tipo)}", inmueble.id_tipo);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.ambientes)}", inmueble.ambientes);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.coordenadas)}", inmueble.coordenadas);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.precio)}", inmueble.precio.ToString());
                command.Parameters.AddWithValue($"@{nameof(Inmueble.id_propietario)}", inmueble.id_propietario);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.estado)}", inmueble.estado.ToString());
                command.Parameters.AddWithValue($"@{nameof(Inmueble.id_ciudad)}", inmueble.id_ciudad);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.id_zona)}", inmueble.id_zona);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.descripcion)}", inmueble.descripcion);
                int columnas = command.ExecuteNonQuery();
                if (columnas > 0)
                {
                    respuesta = true;
                }
                connection.Close();
            }
            return respuesta;
        }

        public bool ActualizarInmueble(Inmueble inmueble, string userId)
        {
            bool respuesta = true;
            Inmueble? casa = GetInmueble(inmueble.id);
            if (casa != null)
            {
                using (var connection = new MySqlConnection(conexion))
                {
                    var sql = @$"UPDATE `inmueble` SET `direccion` = @{nameof(Inmueble.direccion)},`uso`= @{nameof(Inmueble.uso)},`id_tipo`= @{nameof(Inmueble.id_tipo)},`ambientes`=@{nameof(Inmueble.ambientes)},
                    `coordenadas`=@{nameof(Inmueble.coordenadas)},`precio`=@{nameof(Inmueble.precio)},
                    `id_propietario`=@{nameof(Inmueble.id_propietario)},`estado`=@{nameof(Inmueble.estado)},`id_ciudad`=@{nameof(Inmueble.id_ciudad)},`id_zona`=@{nameof(Inmueble.id_zona)},`descripcion`=@{nameof(Inmueble.descripcion)} WHERE `id` = @id;";
                    using var command = new MySqlCommand(sql, connection);
                    connection.Open();
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.direccion)}", inmueble.direccion);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.uso)}", inmueble.uso);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.id_tipo)}", inmueble.id_tipo);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.ambientes)}", inmueble.ambientes);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.coordenadas)}", inmueble.coordenadas);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.precio)}", inmueble.precio);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.id_propietario)}", inmueble.id_propietario);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.estado)}", inmueble.estado);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.id_ciudad)}", inmueble.id_ciudad);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.id_zona)}", inmueble.id_zona);
                    command.Parameters.AddWithValue($"@{nameof(Inmueble.descripcion)}", inmueble.descripcion);
                    command.Parameters.AddWithValue($"@id", inmueble.id);
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
        public bool EliminarInmueble(int id,string userId)
        {
            bool respuesta = false;
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = "UPDATE `inmueble` SET `borrado` = 1 WHERE `id` = @id;";
                using var command = new MySqlCommand(sql, connection);
                connection.Open();
                command.Parameters.AddWithValue($"@id", id);
                int columnas = command.ExecuteNonQuery();
                if (columnas > 0)
                {
                    respuesta = true;
                }
                connection.Close();
            }

            return respuesta;
        }
        public bool setEstado(int id, string estado = "Disponible")
        {
            bool respuesta = false;
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = "UPDATE `inmueble` SET `estado` = @estado WHERE `id` = @id;";
                using var command = new MySqlCommand(sql, connection);
                connection.Open();
                command.Parameters.AddWithValue($"@id", id);
                command.Parameters.AddWithValue($"@estado", estado);
                int columnas = command.ExecuteNonQuery();
                if (columnas > 0)
                {
                    respuesta = true;
                }
                connection.Close();
            }
            return respuesta;
        }

        public IList<Inmueble> GetInmuebleByPropietario(int id) //paginado + filtros
        {
            var inmuebles = new List<Inmueble>();
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = @$"                              
                    SELECT DISTINCT
                        i.id,
                        i.direccion,
                        i.uso,
                        i.id_tipo,
                        i.ambientes,
                        i.coordenadas,                        
                        i.precio,
                        i.id_propietario,
                        i.estado,
                        i.id_ciudad,
                        i.id_zona,
                        i.borrado,
                        i.descripcion,
                        t.id AS t_id_tipo,
                        t.tipo AS tipo_inmueble,
                        p.id AS p_id,
                        p.nombre AS nombre_propietario,
                        p.apellido AS apellido_propietario,
                        c.ciudad,
                        z.zona
                    FROM
                        inmueble AS i
                        INNER JOIN tipo_inmueble AS t 
                            ON i.id_tipo = t.id
                        INNER JOIN propietario AS p 
                            ON i.id_propietario = p.id
                        JOIN ciudad AS c
                            ON c.id = i.id_ciudad
                        JOIN zona AS z 
                            ON z.id = i.id_zona                        
                    WHERE i.id_propietario = @Id   AND i.borrado = 0 ;";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {

                                id = reader.GetInt32("id"),
                                direccion = reader.GetString("direccion"),
                                uso = Enum.TryParse<UsoDeInmueble>(reader.GetString("uso"), out UsoDeInmueble usoEnum) ? usoEnum : UsoDeInmueble.Residencial,
                                estado = Enum.TryParse<EstadoInmueble>(reader.GetString("estado"), out EstadoInmueble estadoEnum) ? estadoEnum : EstadoInmueble.Disponible,
                                //estado = reader.GetString("estado"),
                                id_tipo = reader.GetInt32("id_tipo"),
                                ambientes = reader.GetInt32("ambientes"),
                                coordenadas = reader.GetString("coordenadas"),
                                precio = reader.GetDecimal("precio"),
                                id_propietario = reader.GetInt32("id_propietario"),
                                id_ciudad = reader.GetInt32("id_ciudad"),
                                id_zona = reader.GetInt32("id_zona"),
                                borrado = reader.GetBoolean("borrado"),
                                descripcion = reader.GetString("descripcion"),
                                tipoInmueble = new TipoInmueble
                                {
                                    id = reader.GetInt32("t_id_tipo"),
                                    tipo = reader.GetString("tipo_inmueble")
                                },
                                propietario = new Propietario
                                {
                                    id = reader.GetInt32("p_id"),
                                    nombre = reader.GetString("nombre_propietario"),
                                    apellido = reader.GetString("apellido_propietario")
                                },
                                ciudad = new Ciudad
                                {
                                    id = reader.GetInt32("id_ciudad"),
                                    ciudad = reader.GetString("ciudad")
                                },
                                zona = new Zona
                                {
                                    id = reader.GetInt32("id_zona"),
                                    zona = reader.GetString("zona")
                                }
                            });
                        }
                        connection.Close();
                    }

                }
            }
            return inmuebles;
        }
    }
}





