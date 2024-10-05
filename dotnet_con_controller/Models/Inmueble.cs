using System.ComponentModel.DataAnnotations;
namespace dotnet_2.Models;
public enum EstadoInmueble
{
    Disponible,
    Retirado
}
public enum UsoDeInmueble
{
    Comercial,
    Residencial
}

public class Inmueble
{
    [Key]
    public int id { get; set; }
    [Required]
    [StringLength(100)]
    [Display(Name = "Dirección")]
    public string? direccion { get; set; }
    [Required]
    [Display(Name = "Uso")]
    public UsoDeInmueble uso { get; set; }
    [Required]
    public int id_tipo { get; set; }
    [Display(Name = "Ambientes")]
    public int ambientes { get; set; }
    [Required]
    [Display(Name = "Coordenadas")]
    public string? coordenadas { get; set; }
    [Required]
    [Display(Name = "Precio")]
    public decimal precio { get; set; }
    [Required]
    public int id_propietario { get; set; }
    [Required]
    [Display(Name = "Estado")]
    public EstadoInmueble estado { get; set; }
    [Required]
    public int id_ciudad { get; set; }
    [Required]
    public int id_zona { get; set; }
    [Required]
    public bool borrado { get; set; }
    [Required]
    [Display(Name = "Descripción")]
    public string? descripcion { get; set; }
    [Display(Name = "Propietario")]
    public Propietario? propietario { set; get; }
    [Display(Name = "Tipo Inmueble")]
    public TipoInmueble? tipoInmueble { set; get; }
    [Display(Name = "Ciudad")]
    public Ciudad? ciudad { set; get; }
    [Display(Name = "Zona")]
    public Zona? zona { set; get; }
    [Display(Name = "Mapa")]
    public string mapa => $"https://www.google.com/maps?q={coordenadas}";
    public override string ToString()
    {
        return $"{direccion}    {uso}   Ambientes: {ambientes}  $ {precio}   Descripción: {descripcion} ";
    }
    public string datosPropietario()
    {
        return $"{propietario?.nombre}, {propietario?.apellido}";
    }

}
