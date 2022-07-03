
using System.ComponentModel.DataAnnotations;

namespace Api.Request.Privileges
{
    public class PrivilegesRequest
    {
        [Required(ErrorMessage = "Campo requerido")]
        public string PrivilegeNewName { get; set; }
        public string PrivilegeActualName { get; set; }
    }
}
