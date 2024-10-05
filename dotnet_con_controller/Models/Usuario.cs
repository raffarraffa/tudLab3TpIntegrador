using System.ComponentModel.DataAnnotations;

namespace dotnet_2.Models
{
    public class Usuario
    {
        public int id { get; set; }

        [Required]
        public string? nombre { get; set; }

        [Required]
        public string? apellido { get; set; }

        public string? dni { get; set; }

        public string? email { get; set; }
        public string? password { get; set; }
        public string? rol { get; set; }
        public string? avatarUrl { get; set; }

        public IFormFile avatarFile { get; set; } = null!;


        public bool borrado { get; set; }
        public override string ToString()
        {
            return  $"{id}  | {nombre}  |  {apellido}  |   {dni}  |   {email} | {password}  | {rol}  | {avatarUrl} | {borrado}";
        }
    }
}
