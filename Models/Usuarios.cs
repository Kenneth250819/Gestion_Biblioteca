﻿namespace Gestion_Biblioteca.Models
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string TipoUsuario { get; set; }
        public string Clave { get; set; }
    }
}
