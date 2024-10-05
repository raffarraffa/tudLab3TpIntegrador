namespace dotnet_2.Models;
public class Inquilino
{
    public int id { get; set; }
    public string? nombre { get; set; }
    public string? apellido { get; set; }
    public string? dni { get; set; }
    public string? email { get; set; }
    public string? telefono { get; set; }
    public bool? borrado { get; set; }
    public override string ToString()
    {
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
        return $" {apellido.ToUpper()}, {nombre}, DNI: {dni}, Email: {email}, Teléfono: {telefono}";
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.

    }
}
