namespace dotnet_2.Models;

public class Propietario
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
    return $"id: {id}, nombre: {nombre}, apellido: {apellido}, dni: {dni}, email: {email}, telefono: {telefono}, borrado: {borrado}";
}
public string ToStringWeb(){
    return $" {apellido} , {nombre} dni: {dni} ";
}

}