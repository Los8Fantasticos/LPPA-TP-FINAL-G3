using System.ComponentModel.DataAnnotations;

namespace Api.Request
{
    public class EditUserRequest
    {
        [Required(ErrorMessage = "Campo requerido")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [RegularExpression(@"\A[\w|.|-]+@[\w|.|-]+\Z", ErrorMessage = "El formato no corresponde al de una dirección de correo")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string PhoneNumber { get; set; }
    }
}
