using System.ComponentModel.DataAnnotations;

namespace Api.Request
{
    public class PrivilegesRequest
    {
        [Required(ErrorMessage = "Campo requerido")]
        public string PrivilegeName { get; set; }
    }
}
