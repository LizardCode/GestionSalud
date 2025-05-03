using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LizardCode.SalmaSalud.Application.Models.Usuarios
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        public int IdTipoUsuario { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9]{4,49}$", ErrorMessage = "El Nombre de Usuario debe empezar con una letra y solo puede contener letras y números. Además debe tener al menos 5 caracteres.")]
        [Remote("ValidarLogin", "Usuarios", HttpMethod = "Post", ErrorMessage = "Login en uso")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        [EmailAddress(ErrorMessage = "Formato incorrecto")]
        public string Email { get; set; }

        public bool Admin { get; set; }

        public List<int> Empresas { get; set; }

        public SelectList MaestroTipoUsuarios { get; set; }
        public SelectList MaestroEmpresas { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        public int IdProfesional { get; set; }
        public SelectList MaestroProfesionales { get; set; }

        [Required(ErrorMessage = "Obligatorio")]
        public int IdPaciente { get; set; }
        public SelectList MaestroPacientes { get; set; }

    }
}