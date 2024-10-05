namespace dotnet_2.Models
{
    public class Pago
    {
        public int id { get; set; }
        public int id_contrato { get; set; }
        public DateOnly fecha_pago { get; set; }
        public decimal importe { get; set; }
        public bool estado { get; set; }
        public int numero_pago { get; set; }
        public string? detalle { get; set; }
        public Contrato? Contrato { get; set; }
        public DateOnly fecha_creado { get; set; }
        public DateOnly fecha_editado { get; set; }
        public Usuario? creado_usuario { get; set; }
        public Usuario? editado_usuario { get; set; }

    }
}

