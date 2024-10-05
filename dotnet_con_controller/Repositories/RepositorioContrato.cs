using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.CRUD;
namespace dotnet_2.Models;
public class RepositorioContrato
{
    private readonly string conexion;
    public RepositorioContrato()
    {
        this.conexion = Conexion.GetConnectionString();
    }
    /**
metodo para obtener todos los Contratos
*/
    public IList<Contrato> GetContratos(string? fechaInicio, string? fechaFin)
    {

        List<Contrato> contratos = new List<Contrato>();
        try
        {
            using (var connection = new MySqlConnection(conexion))
            {
                // particionaod consulta
                string dataAccion = "SELECT ";
                string dataContrato = @$"c.{nameof(Contrato.id_inquilino)}  AS idInquilino,  c.{nameof(Contrato.id)} AS idContrato,c.{nameof(Contrato.monto)} AS montoContrato, c.{nameof(Contrato.fecha_inicio)} AS fechaInicio, c.{nameof(Contrato.fecha_fin)} AS fechaFin, c.{nameof(Contrato.fecha_efectiva)} AS fechaEfectiva, ";
                string dataInquilino = @$"  i.{nameof(Inquilino.nombre)} AS inquilinoNombre, i.{nameof(Inquilino.apellido)} AS inquilinoApellido, ";
                string dataInmueble = @$" p.{nameof(Inmueble.id)} AS inmuebleId, p.{nameof(Inmueble.direccion)} AS inmuebleDireccion, ";
                string dataPropietario = @$" pro.{nameof(Propietario.nombre)} AS propietarioNombre , pro.{nameof(Propietario.apellido)} AS propietarioApellido ";
                string dataFrom = " FROM `contrato` AS c ";
                string dataJoinInquilino = " JOIN inquilino AS i ";
                string dataOnInquilino = " ON c.id_inquilino = i.id AND c.fecha_efectiva > now() ";
                string dataJoinInmueble = " JOIN inmueble AS p ";
                string dataOnInmueble = " ON p.id = c.id_inmueble ";
                string dataJoinPropietario = " JOIN propietario AS pro ";
                string dataOnPropietario = " ON pro.id = p.id_propietario ";
                string dataWhere = "";
                if (!(string.IsNullOrEmpty(fechaInicio) && string.IsNullOrEmpty(fechaFin)))
                {
                    dataWhere = $"WHERE c.{nameof(Contrato.fecha_inicio)} >= {fechaInicio} AND c.{nameof(Contrato.fecha_fin)} <= '{fechaFin}' ";
                }
                string sql = dataAccion + dataContrato + dataInquilino + dataInmueble + dataPropietario + dataFrom;
                sql += dataJoinInquilino + dataOnInquilino + dataJoinInmueble + dataOnInmueble + dataJoinPropietario + dataOnPropietario + dataWhere;
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Contrato contrato = new Contrato
                            contratos.Add(new Contrato
                            {
                                id = reader.GetInt32("idContrato"),
                                id_inquilino = reader.GetInt32("idInquilino"),
                                monto = reader.GetDecimal("montoContrato"),
                                fecha_inicio = new DateOnly(reader.GetDateTime("fechaInicio").Year, reader.GetDateTime("fechaInicio").Month, reader.GetDateTime("fechaInicio").Day),
                                fecha_fin = !reader.IsDBNull(reader.GetOrdinal("fechaFin")) ? new DateOnly(reader.GetDateTime("fechaFin").Year, reader.GetDateTime("fechaFin").Month, reader.GetDateTime("fechaFin").Day) : new DateOnly(0001, 01, 01), // O cualquier otro valor por defecto que desees
                                fecha_efectiva = DateOnly.FromDateTime(reader.GetDateTime("fechaEfectiva")),
                                dias_to_fin = Utils.CompararFecha(reader.GetDateTime("fechaFin").ToString("yyyy-MM-dd"), null, false),
                                meses_to_fin = (int)Math.Abs((decimal)(Utils.CompararFecha(reader.GetDateTime("fechaFin").ToString("yyyy-MM-dd"), null, false) / 30.473)),
                                meses_contrato = (int)Math.Abs((decimal)(Utils.CompararFecha(reader.GetDateTime("fechaFin").ToString("yyyy-MM-dd"), reader.GetDateTime("fechaInicio").ToString("yyyy-MM-dd"), false) / 30.473)),
                                inquilino = new Inquilino
                                {
                                    nombre = reader.GetString("inquilinoNombre"),
                                    apellido = reader.GetString("inquilinoApellido").ToUpper()
                                },
                                inmueble = new Inmueble
                                {
                                    id = reader.GetInt32("inmuebleId"),
                                    direccion = reader.GetString("inmuebleDireccion"),
                                    propietario = new Propietario
                                    {
                                        nombre = reader.GetString("propietarioNombre"),
                                        apellido = reader.GetString("propietarioApellido").ToUpper()
                                    },
                                }
                            });
                        }
                    }
                    connection.Close();
                    return contratos;
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("Error al obtener contratos: " + e.Message);
        }
        return contratos;
    }
    /** meotod paralistar contratos de un inmueble*/
    public IList<Contrato> GetContratosInmueble(int id)
    {
        var contratos = new List<Contrato>();
        try
        {
            using (var connection = new MySqlConnection(conexion))
            {
                string dataAccion = "SELECT ";
                string dataContrato = @$"c.{nameof(Contrato.id_inquilino)}  AS idInquilino,  c.{nameof(Contrato.id)} AS idContrato,c.{nameof(Contrato.monto)} AS montoContrato, c.{nameof(Contrato.fecha_inicio)} AS fechaInicio, c.{nameof(Contrato.fecha_fin)} AS fechaFin,";
                string dataInquilino = @$"  i.{nameof(Inquilino.nombre)} AS inquilinoNombre, i.{nameof(Inquilino.apellido)} AS inquilinoApellido, ";
                string dataInmueble = @$" p.{nameof(Inmueble.direccion)} AS inmuebleDireccion, ";
                string dataPropietario = @$" pro.{nameof(Propietario.nombre)} AS propietarioNombre , pro.{nameof(Propietario.apellido)} AS propietarioApellido ";
                string dataFrom = " FROM `contrato` AS c ";
                string dataJoinInquilino = " JOIN inquilino AS i ";
                string dataOnInquilino = " ON c.id_inquilino = i.id AND c.fecha_efectiva IS NULL ";
                string dataJoinInmueble = " JOIN inmueble AS p ";
                string dataOnInmueble = " ON p.id = c.id_inmueble ";
                string dataJoinPropietario = " JOIN propietario AS pro ";
                string dataOnPropietario = " ON pro.id = p.id_propietario ";
                string dataWhere = " WHERE c.id_inmueble =@Id";
                string sql = dataAccion + dataContrato + dataInquilino + dataInmueble + dataPropietario + dataFrom;
                sql += dataJoinInquilino + dataOnInquilino + dataJoinInmueble + dataOnInmueble + dataJoinPropietario + dataOnPropietario + dataWhere;
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contratos.Add(new Contrato
                            {
                                id = reader.GetInt32("idContrato"),
                                id_inquilino = reader.GetInt32("idInquilino"),
                                monto = reader.GetDecimal("montoContrato"),
                                fecha_inicio = new DateOnly(reader.GetDateTime("fechaInicio").Year, reader.GetDateTime("fechaInicio").Month, reader.GetDateTime("fechaInicio").Day),
                                //       fecha_fin = new DateOnly(reader.GetDateTime("fechaFin").Year, reader.GetDateTime("fechaFin").Month, reader.GetDateTime("fechaFin").Day),
                                fecha_fin = !reader.IsDBNull(reader.GetOrdinal("fechaFin")) ?
                                    new DateOnly(reader.GetDateTime("fechaFin").Year, reader.GetDateTime("fechaFin").Month, reader.GetDateTime("fechaFin").Day) :
                                    new DateOnly(0001, 01, 01),
                                dias_to_fin = Utils.CompararFecha(reader.GetDateTime("fechaFin").ToString("yyyy-MM-dd"), null, false),

                                inquilino = new Inquilino
                                {
                                    nombre = reader.GetString("inquilinoNombre"),
                                    apellido = reader.GetString("inquilinoApellido").ToUpper()
                                },
                                inmueble = new Inmueble
                                {
                                    direccion = reader.GetString("inmuebleDireccion"),
                                    propietario = new Propietario
                                    {
                                        nombre = reader.GetString("propietarioNombre"),
                                        apellido = reader.GetString("propietarioApellido").ToUpper()
                                    },
                                }
                            });
                        }
                    }
                    connection.Close();
                    return contratos;
                }
            }

        }
        catch (Exception e)
        {
            Debug.WriteLine("Error al obtener contratos: " + e.Message);
        }
        return contratos;
    }
    public Contrato GetContrato(int id)
    {
        Contrato contrato = null;
        try
        {
            using (var connection = new MySqlConnection(conexion))
            {
                // particionaod consulta
                string dataAccion = "SELECT ";
                string dataContrato = @$"c.{nameof(Contrato.id_inquilino)}  AS idInquilino,  c.{nameof(Contrato.id)} AS idContrato,c.{nameof(Contrato.monto)} AS montoContrato, c.{nameof(Contrato.fecha_inicio)} AS fechaInicio, c.{nameof(Contrato.fecha_fin)} AS fechaFin, c.{nameof(Contrato.id_inmueble)} AS idInmueble,";
                string dataInquilino = @$"  i.{nameof(Inquilino.nombre)} AS inquilinoNombre, i.{nameof(Inquilino.apellido)} AS inquilinoApellido, ";
                string dataInmueble = @$"p.{nameof(Inmueble.id)} AS inmuebleId, p.{nameof(Inmueble.direccion)} AS inmuebleDireccion, p.{nameof(Inmueble.uso)} AS UsoInmueble, p.{nameof(Inmueble.precio)},p.{nameof(Inmueble.descripcion)},";
                string dataPropietario = @$" pro.{nameof(Propietario.nombre)} AS propietarioNombre , pro.{nameof(Propietario.apellido)} AS propietarioApellido ";
                string dataFrom = " FROM `contrato` AS c ";
                string dataJoinInquilino = " JOIN inquilino AS i ";
                string dataOnInquilino = " ON c.id_inquilino = i.id AND c.fecha_efectiva > now() ";
                string dataJoinInmueble = " JOIN inmueble AS p ";
                string dataOnInmueble = " ON p.id = c.id_inmueble ";
                string dataJoinPropietario = " JOIN propietario AS pro ";
                string dataOnPropietario = " ON pro.id = p.id_propietario ";
                string dataWhere = " WHERE c.id = " + id;
                string sql = dataAccion + dataContrato + dataInquilino + dataInmueble + dataPropietario + dataFrom;
                sql += dataJoinInquilino + dataOnInquilino + dataJoinInmueble + dataOnInmueble + dataJoinPropietario + dataOnPropietario + dataWhere;

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //                           int? diasToFin = Utils.CompararFecha(reader.GetDateTime("fechaFin").ToString("yyyy-MM-dd"), null, false);
                            contrato = new Contrato
                            {
                                id = reader.GetInt32("idContrato"),
                                id_inquilino = reader.GetInt32("idInquilino"),
                                id_inmueble = reader.GetInt32("idInmueble"),
                                monto = reader.GetDecimal("montoContrato"),
                                fecha_inicio = new DateOnly(reader.GetDateTime("fechaInicio").Year, reader.GetDateTime("fechaInicio").Month, reader.GetDateTime("fechaInicio").Day),
                                //       fecha_fin = new DateOnly(reader.GetDateTime("fechaFin").Year, reader.GetDateTime("fechaFin").Month, reader.GetDateTime("fechaFin").Day),
                                fecha_fin = !reader.IsDBNull(reader.GetOrdinal("fechaFin")) ?
                                    new DateOnly(reader.GetDateTime("fechaFin").Year, reader.GetDateTime("fechaFin").Month, reader.GetDateTime("fechaFin").Day) :
                                    new DateOnly(0001, 01, 01), // O cualquier otro valor por defecto que desees
                                dias_to_fin = Utils.CompararFecha(reader.GetDateTime("fechaFin").ToString("yyyy-MM-dd"), null, false),
                                meses_to_fin = (int)Math.Abs((decimal)(Utils.CompararFecha(reader.GetDateTime("fechaFin").ToString("yyyy-MM-dd"), null, false) / 30.473)),
                                meses_contrato = (int)Math.Abs((decimal)(Utils.CompararFecha(reader.GetDateTime("fechaFin").ToString("yyyy-MM-dd"), reader.GetDateTime("fechaInicio").ToString("yyyy-MM-dd"), false) / 30.473)),
                                inquilino = new Inquilino
                                {
                                    id = reader.GetInt32("idInquilino"),
                                    nombre = reader.GetString("inquilinoNombre"),
                                    apellido = reader.GetString("inquilinoApellido").ToUpper()
                                },
                                inmueble = new Inmueble
                                {
                                    id = reader.GetInt32("inmuebleId"),
                                    precio = reader.GetDecimal("precio"),
                                    uso = Enum.TryParse<UsoDeInmueble>(reader.GetString("usoInmueble"), out UsoDeInmueble usoEnum) ? usoEnum : UsoDeInmueble.Residencial,
                                    direccion = reader.GetString("inmuebleDireccion"),
                                    descripcion = reader.GetString("descripcion"),
                                    propietario = new Propietario
                                    {
                                        nombre = reader.GetString("propietarioNombre"),
                                        apellido = reader.GetString("propietarioApellido").ToUpper()
                                    },
                                }
                            };
                        }
                    }
                    connection.Close();
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("Error al obtener contratos: " + e.Message);
        }
        return contrato;
    }
    public IList<Contrato> GetContratosPropietario(int id)
    {
        IList<Contrato> contratos = new List<Contrato>();

        try
        {
            using (var connection = new MySqlConnection(conexion))
            {
                string dataAccion = "SELECT ";
                string dataContrato = @$"c.{nameof(Contrato.id_inquilino)}  AS idInquilino,  c.{nameof(Contrato.id)} AS idContrato,c.{nameof(Contrato.monto)} AS montoContrato, c.{nameof(Contrato.fecha_inicio)} AS fechaInicio, c.{nameof(Contrato.fecha_fin)} AS fechaFin,";
                string dataInquilino = @$"  i.{nameof(Inquilino.nombre)} AS inquilinoNombre, i.{nameof(Inquilino.apellido)} AS inquilinoApellido, ";
                string dataInmueble = @$" p.{nameof(Inmueble.direccion)} AS inmuebleDireccion, ";
                string dataPropietario = @$" pro.{nameof(Propietario.nombre)} AS propietarioNombre , pro.{nameof(Propietario.apellido)} AS propietarioApellido ";
                string dataFrom = " FROM `contrato` AS c ";
                string dataJoinInquilino = " JOIN inquilino AS i ";
                string dataOnInquilino = " ON c.id_inquilino = i.id AND c.fecha_efectiva IS NULL ";
                string dataJoinInmueble = " JOIN inmueble AS p ";
                string dataOnInmueble = " ON p.id = c.id_inmueble ";
                string dataJoinPropietario = " JOIN propietario AS pro ";
                string dataOnPropietario = " ON pro.id = p.id_propietario ";
                string dataWhere = " WHERE p.id_propietario =@Id";
                string sql = dataAccion + dataContrato + dataInquilino + dataInmueble + dataPropietario + dataFrom;
                sql += dataJoinInquilino + dataOnInquilino + dataJoinInmueble + dataOnInmueble + dataJoinPropietario + dataOnPropietario + dataWhere;
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? diasToFin = Utils.CompararFecha(reader.GetDateTime("fechaFin").ToString("yyyy-MM-dd"), null, false);
                            contratos.Add(new Contrato
                            {
                                id = reader.GetInt32("idContrato"),
                                id_inquilino = reader.GetInt32("idInquilino"),
                                monto = reader.GetDecimal("montoContrato"),
                                fecha_inicio = new DateOnly(reader.GetDateTime("fechaInicio").Year, reader.GetDateTime("fechaInicio").Month, reader.GetDateTime("fechaInicio").Day),
                                //       fecha_fin = new DateOnly(reader.GetDateTime("fechaFin").Year, reader.GetDateTime("fechaFin").Month, reader.GetDateTime("fechaFin").Day),
                                fecha_fin = !reader.IsDBNull(reader.GetOrdinal("fechaFin")) ?
                                    new DateOnly(reader.GetDateTime("fechaFin").Year, reader.GetDateTime("fechaFin").Month, reader.GetDateTime("fechaFin").Day) :
                                    new DateOnly(0001, 01, 01),
                                dias_to_fin = diasToFin,

                                inquilino = new Inquilino
                                {
                                    nombre = reader.GetString("inquilinoNombre"),
                                    apellido = reader.GetString("inquilinoApellido").ToUpper()
                                },
                                inmueble = new Inmueble
                                {
                                    direccion = reader.GetString("inmuebleDireccion"),
                                    propietario = new Propietario
                                    {
                                        nombre = reader.GetString("propietarioNombre"),
                                        apellido = reader.GetString("propietarioApellido").ToUpper()
                                    },
                                }
                            });
                        }
                    }
                    connection.Close();
                    return contratos;
                }
            }

        }
        catch (Exception e)
        {
            Debug.WriteLine("Error al obtener contratos: " + e.Message);
        }
        return contratos;
    }
    public Contrato Create(Contrato contrato, string userId)
    {
        /*validar datos*/
        using (var connection = new MySqlConnection(conexion))
        {            
            string sql = @$" INSERT INTO `contrato` (`id_inquilino`, `id_inmueble`, `fecha_inicio`, `fecha_fin`,`fecha_efectiva` , `monto`, `creado_fecha`, `creado_usuario`) VALUES (   @{nameof(Contrato.id_inquilino)},@{nameof(Contrato.id_inmueble)}, @{nameof(Contrato.fecha_inicio)}, @{nameof(Contrato.fecha_fin)},  @{nameof(Contrato.fecha_efectiva)}, @{nameof(Contrato.monto)},now(),@userId);";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue($"@{nameof(Contrato.id_inquilino)}", contrato.id_inquilino);
            command.Parameters.AddWithValue($"@{nameof(Contrato.id_inmueble)}", contrato.id_inmueble);
            command.Parameters.AddWithValue($"@{nameof(Contrato.fecha_inicio)}", contrato.fecha_inicio.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue($"@{nameof(Contrato.fecha_fin)}", contrato.fecha_fin.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue($"@{nameof(Contrato.fecha_efectiva)}", contrato.fecha_efectiva.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue($"@{nameof(Contrato.monto)}", contrato.monto);
            command.Parameters.AddWithValue($"@userId", userId);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        return contrato;
    }
    public Contrato SetFinContrato(int id, string fechaEfectiva)
    {
        using (var connection = new MySqlConnection(conexion))
        {
            string sql = @$"UPDATE `contrato` SET                
                `fecha_efectiva` = @fechaEfectiva
                WHERE `id` = @Id;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue($"@fechaEfectiva", fechaEfectiva);
            command.Parameters.AddWithValue($"@Id", id);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        return null;
    }

    public bool ActualizarContrato(int id, string fecha_fin, decimal monto,string userId)
    { //solo se le perimite modificar la fecha de fin - la fecha de efectiva y el monto
        bool respuesta = false;
        using (var connection = new MySqlConnection(conexion))
        {

            var sql = @$"UPDATE contrato SET fecha_fin = @fecha_fin, fecha_efectiva = @fecha_efectiva, monto = @monto, editado_fecha = now(), editado_usuario = @userId  WHERE id = @id";
            using var command = new MySqlCommand(sql, connection);
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@fecha_fin", fecha_fin);
                command.Parameters.AddWithValue("@fecha_efectiva", fecha_fin);
                command.Parameters.AddWithValue("@monto", monto);
                command.Parameters.AddWithValue("@userID", userId);
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
    }

    public bool VerifyInmuebleContrato(string F1, string F2, int id)
    {
        using (var connection = new MySqlConnection(conexion))
        {
            try
            {
                connection.Open();
                string sql = "verify_contrato";
                using var command = new MySqlCommand(sql, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@f_inicio", F1);
                command.Parameters.AddWithValue("@f_fin", F2);
                command.Parameters.AddWithValue("@id_prop", id);
                using MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error en VerifyInmuebleContrato: " + ex.Message);
                throw;

            }
            finally
            {
                connection.Close();
            }

            return true;
        }
    }

}