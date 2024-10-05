using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System;

namespace dotnet_2.Models
{
    public class RepositorioPago
    {
        private readonly string conexion = Conexion.GetConnectionString();

        public IList<Pago> ObtenerPagos()
        {
            List<Pago> pagos = new List<Pago>();
            var sql = "SELECT * FROM pago";
            using (var connection = new MySqlConnection(conexion))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pago = new Pago
                            {
                                id = reader.GetInt32("id"),
                                id_contrato = reader.GetInt32("id_contrato"),
                                fecha_pago = new DateOnly(reader.GetDateTime("fecha_pago").Year, reader.GetDateTime("fecha_pago").Month, reader.GetDateTime("fecha_pago").Day),
                                importe = reader.GetDecimal("importe"),
                                estado = reader.GetBoolean("estado"),
                                numero_pago = reader.GetInt32("numero_pago"),
                                detalle = reader.IsDBNull(reader.GetOrdinal("detalle")) ? null : reader.GetString("detalle")
                            };
                            pagos.Add(pago);
                        }
                    }
                }
            }
            return pagos;
        }

        /// <summary>
        /// Guarda un objeto Pago en la base de datos con el ID de usuario dado.
        /// </summary>
        /// <param name="pago">El objeto Pago a guardar.</param>
        /// <param name="userId">El ID del usuario que realiza la operación de guardado.</param>
        /// <returns>El objeto Pago guardado, o null si ocurrió un error.</returns>
        public Pago? GuardarPago(Pago pago, string userId)
        {
            try
            {
                using (var connection = new MySqlConnection(conexion))
                {
                    var sql = @$"INSERT INTO pago (`{nameof(Pago.id_contrato)}`, `{nameof(Pago.importe)}`,`{nameof(Pago.estado)}`, `{nameof(Pago.numero_pago)}`, `{nameof(Pago.detalle)}`, `creado_usuario`, `creado_fecha`)
                    VALUES (@id_contrato, @importe, @estado, @numero_pago, @detalle, @userId, now());";
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id_contrato", pago.id_contrato);
                        command.Parameters.AddWithValue("@importe", pago.importe);
                        command.Parameters.AddWithValue("@estado", pago.estado);
                        command.Parameters.AddWithValue("@numero_pago", pago.numero_pago);
                        command.Parameters.AddWithValue("@detalle", pago.detalle);
                        command.Parameters.AddWithValue("@userId", userId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                return pago;
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console.WriteLine("Error: Ya existe un pago con los mismos valores únicos -->" + ex.Message);
                throw new Exception("Error: Ya existe un pago con los mismos valores únicos  " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el pago: {ex.Message}");
                return null;
            }
        }
        public IList<Pago> listarPagosPorContrato(int id_contrato)
        {
            List<Pago> pagos = new List<Pago>();
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = @$"SELECT  i.nombre, 
            i.apellido,
            i.telefono, 
            i.email, 
            p.id_contrato, 
            p.numero_pago, 
            p.fecha_pago, 
            p.importe, 
            p.detalle,         
            alquiler.direccion,      
            c.fecha_inicio, 
            c.fecha_fin,         
            c.monto,
            pro.id as id_propietario,
            CONCAT(pro.nombre, ', ', pro.apellido ) AS propietario 
            FROM pago p
            INNER JOIN contrato c
            ON p.id_contrato = c.id
            INNER JOIN inquilino i
            ON c.id_inquilino = i.id
            INNER JOIN inmueble AS alquiler
            ON c.id_inmueble = alquiler.id
            INNER JOIN propietario AS pro
            ON  alquiler.id_propietario = pro.id
            WHERE p.id_contrato = @id_contrato";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_contrato", id_contrato);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pago pago = new Pago
                            {
                                id_contrato = reader.GetInt32("id_contrato"),
                                numero_pago = reader.GetInt32("numero_pago"),
                                fecha_pago = new DateOnly(reader.GetDateTime("fecha_pago").Year, reader.GetDateTime("fecha_pago").Month, reader.GetDateTime("fecha_pago").Day),
                                importe = reader.IsDBNull(reader.GetOrdinal("importe")) ? 0 : reader.GetDecimal("importe"),
                                detalle = reader.IsDBNull(reader.GetOrdinal("detalle")) ? string.Empty : reader.GetString("detalle")
                            };
                            pago.Contrato = new Contrato
                            {
                                fecha_inicio = new DateOnly(reader.GetDateTime("fecha_inicio").Year, reader.GetDateTime("fecha_inicio").Month, reader.GetDateTime("fecha_inicio").Day),
                                fecha_fin = new DateOnly(reader.GetDateTime("fecha_fin").Year, reader.GetDateTime("fecha_fin").Month, reader.GetDateTime("fecha_fin").Day),
                                monto = reader.IsDBNull(reader.GetOrdinal("monto")) ? 0 : reader.GetDecimal("monto")
                            };
                            pago.Contrato.inquilino = new Inquilino
                            {
                                nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? string.Empty : reader.GetString("nombre"),
                                apellido = reader.IsDBNull(reader.GetOrdinal("apellido")) ? string.Empty : reader.GetString("apellido"),
                                telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? string.Empty : reader.GetString("telefono"),
                                email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString("email")
                            };

                            pago.Contrato.inmueble = new Inmueble
                            {
                                direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? string.Empty : reader.GetString("direccion")
                            };


                            pago.Contrato.inmueble.propietario = new Propietario
                            {
                                id = reader.GetInt32("id_propietario"),
                                nombre = reader.IsDBNull(reader.GetOrdinal("propietario")) ? string.Empty : reader.GetString("propietario"),
                                apellido = reader.IsDBNull(reader.GetOrdinal("propietario")) ? string.Empty : reader.GetString("propietario"),
                            };

                            pagos.Add(pago);
                        }
                    }
                }
            }
            return pagos;
        }
        public int obtenerUltimoPago(int id_contrato)
        {
            int ultimoPago = 0;
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = @"SELECT MAX(numero_pago) AS ultimo_pago
                    FROM pago
                    WHERE id_contrato = @id_contrato";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_contrato", id_contrato);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("ultimo_pago")))
                            {
                                ultimoPago = reader.GetInt32("ultimo_pago");
                            }
                        }
                    }
                }
            }
            return ultimoPago;
        }
        public IList<Pago> ObtenerHistoricoPagos()
        {
            List<Pago> pagos = new List<Pago>();
            var sql = @"SELECT  
                    i.nombre AS nombre_inquilino,
                    i.apellido AS apellido_inquilino,
                    pro.nombre AS nombre_propietario,
                    pro.apellido AS apellido_propietario,
                    p.id_contrato,
                    p.numero_pago,
                    p.fecha_pago, 
                    p.detalle,         
                    alquiler.direccion AS direccion_inmueble,      
                    c.fecha_inicio AS fecha_inicio_contrato, 
                    c.fecha_fin AS fecha_fin_contrato,         
                    c.monto AS monto_contrato,
                    c.id AS id_contrato
                FROM pago p
                INNER JOIN contrato c ON p.id_contrato = c.id
                INNER JOIN inquilino i ON c.id_inquilino = i.id
                INNER JOIN inmueble AS alquiler ON c.id_inmueble = alquiler.id
                INNER JOIN propietario AS pro ON alquiler.id_propietario = pro.id";
            using (var connection = new MySqlConnection(conexion))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contrato = new Contrato
                            {
                                id = reader.GetInt32("id_contrato"),
                                fecha_inicio = new DateOnly(reader.GetDateTime("fecha_inicio_contrato").Year, reader.GetDateTime("fecha_inicio_contrato").Month, reader.GetDateTime("fecha_inicio_contrato").Day),
                                fecha_fin = new DateOnly(reader.GetDateTime("fecha_fin_contrato").Year, reader.GetDateTime("fecha_fin_contrato").Month, reader.GetDateTime("fecha_fin_contrato").Day),
                                monto = reader.GetDecimal("monto_contrato"),
                                inquilino = new Inquilino
                                {
                                    nombre = reader.GetString("nombre_inquilino"),
                                    apellido = reader.GetString("apellido_inquilino")
                                },
                                inmueble = new Inmueble
                                {
                                    direccion = reader.GetString("direccion_inmueble"),
                                    propietario = new Propietario
                                    {
                                        nombre = reader.GetString("nombre_propietario"),
                                        apellido = reader.GetString("apellido_propietario")
                                    }
                                }
                            };
                            var pago = new Pago
                            {
                                fecha_pago = new DateOnly(reader.GetDateTime("fecha_pago").Year, reader.GetDateTime("fecha_pago").Month, reader.GetDateTime("fecha_pago").Day),
                                detalle = reader.IsDBNull(reader.GetOrdinal("detalle")) ? null : reader.GetString("detalle"),
                                numero_pago = reader.GetInt32("numero_pago"),
                                id_contrato = reader.GetInt32("id_contrato"),
                                Contrato = contrato,
                                // id_contrato = contrato.id
                            };
                            pagos.Add(pago);
                        }
                    }
                }
            }
            return pagos;
        }
        public Pago? PagoPorId(int id)
        {
            Pago? pago = null;
            using (var connection = new MySqlConnection(conexion))
            {
                connection.Open();
                var sql = @$"SELECT
                        i.nombre AS inquilino_nombre,
                        i.apellido AS inquilino_apellido,
                        im.direccion AS inmueble_direccion,
                        pro.nombre AS propietario_nombre,
                        pro.apellido AS propietario_apellido,
                        p.importe,
                        p.detalle,
                        p.numero_pago
                        FROM pago AS p
                        JOIN contrato AS c ON p.id_contrato = c.id
                        JOIN inquilino AS i ON c.id_inquilino = i.id
                        JOIN inmueble AS im ON c.id_inmueble = im.id
                        JOIN propietario AS pro ON im.id_propietario = pro.id
                        WHERE p.id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago
                            {
                                id = id,
                                importe = reader.GetDecimal("importe"),
                                detalle = reader.IsDBNull(reader.GetOrdinal("detalle")) ? null : reader.GetString("detalle"),
                                numero_pago = reader.GetInt32("numero_pago"),
                                Contrato = new Contrato
                                {
                                    inquilino = new Inquilino
                                    {
                                        nombre = reader.GetString("inquilino_nombre"),
                                        apellido = reader.GetString("inquilino_apellido")
                                    },
                                    inmueble = new Inmueble
                                    {
                                        direccion = reader.GetString("inmueble_direccion"),
                                        propietario = new Propietario
                                        {
                                            nombre = reader.GetString("propietario_nombre"),
                                            apellido = reader.GetString("propietario_apellido")
                                        }
                                    }
                                }
                            };
                        }
                    }
                }
            }

            return pago;
        }

        // Actualiza un pago en la base de datos con el nuevo importe y detalles. 
        // Parámetros:
        //   pago: El objeto de pago que contiene la información del pago actualizada.
        //   userId: El ID del usuario que realiza la edición.
        // Retorna:
        //   Un booleano que indica si la operación de actualización fue exitosa.
        public bool EditarPago(Pago pago, string userId)
        {
            using (var connection = new MySqlConnection(conexion))
            {
                connection.Open();
                var sql = "UPDATE pago SET importe = @importe, detalle = @detalle, editado_usuario = @userId, editado_fecha = now() WHERE id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", pago.id);
                    command.Parameters.AddWithValue("@importe", pago.importe);
                    command.Parameters.AddWithValue("@detalle", pago.detalle);
                    command.Parameters.AddWithValue("@userId", userId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public IList<Pago> AuditoriaPago()
        {
            List<Pago> listaPagos = new List<Pago>();
            using (var connection = new MySqlConnection(conexion))
            {
                var sql = @$"SELECT
                            alquiler.direccion,
                            alquiler.uso,
                            t.tipo,
                            i.nombre as nombre_inquilino,
                            i.apellido as apellido_inquilino,
                            p.fecha_pago, 
                            p.importe, 
                            p.numero_pago,
                            p.detalle, 
                            p.creado_fecha, 
                            u.nombre, 
                            u.apellido,
                            u.email
                            FROM pago as p
                            INNER JOIN usuario as u ON p.creado_usuario = u.id
                            INNER JOIN contrato c ON p.id_contrato = c.id
                            INNER JOIN inquilino i ON c.id_inquilino = i.id
                            INNER JOIN inmueble alquiler ON c.id_inmueble = alquiler.id
                            INNER JOIN tipo_inmueble as t ON alquiler.id_tipo = t.id
                            INNER JOIN propietario pro ON alquiler.id_propietario = pro.id
                            WHERE u.id = p.creado_usuario;";
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                // command.CommandType = CommandType.StoredProcedure; //para no usar el procedimiento almacenado
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Pago pago = new Pago();
                        pago.numero_pago = Convert.ToInt32(reader["numero_pago"]);
                        pago.fecha_pago = new DateOnly(reader.GetDateTime("fecha_pago").Year, reader.GetDateTime("fecha_pago").Month, reader.GetDateTime("fecha_pago").Day);
                        pago.importe = Convert.ToDecimal(reader["importe"]);
                        pago.detalle = reader.IsDBNull(reader.GetOrdinal("detalle")) ? null : reader.GetString("detalle");
                        pago.Contrato = new Contrato
                        {
                            inquilino = new Inquilino
                            {
                                nombre = reader["nombre_inquilino"].ToString(),
                                apellido = reader["apellido_inquilino"].ToString()
                            },
                            inmueble = new Inmueble
                            {
                                direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? string.Empty : reader.GetString("direccion"),
                                uso = Enum.TryParse<UsoDeInmueble>(reader.GetString("uso"), out UsoDeInmueble usoEnum) ? usoEnum : UsoDeInmueble.Residencial,
                                tipoInmueble = new TipoInmueble
                                {
                                    tipo = reader.GetString("tipo")
                                },
                            }
                        };

                        pago.creado_usuario = new Usuario
                        {
                            nombre = reader["nombre"].ToString(),
                            apellido = reader["apellido"].ToString(),
                            email = reader["email"].ToString()
                        };

                        listaPagos.Add(pago);
                    }
                }
            }
            return listaPagos;
        }



    }
}
// SELECT
//     alquiler.direccion,
//     alquiler.uso,
//     t.tipo,
//     i.nombre as nombre_inquilino,
//     i.apellido as apellido_inquilino,
//     p.fecha_pago, 
//     p.importe, 
//     p.numero_pago,
//     p.detalle, 
//     p.creado_fecha, 
//     u.nombre, 
//     u.apellido,
//     u.email
// FROM pago as p
// INNER JOIN usuario as u ON p.creado_usuario = u.id
// INNER JOIN contrato c ON p.id_contrato = c.id
// INNER JOIN inquilino i ON c.id_inquilino = i.id
// INNER JOIN inmueble alquiler ON c.id_inmueble = alquiler.id
// INNER JOIN tipo_inmueble as t ON alquiler.id_tipo = t.id
// INNER JOIN propietario pro ON alquiler.id_propietario = pro.id
// WHERE u.id = p.creado_usuario;
