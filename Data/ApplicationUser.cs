using Microsoft.AspNetCore.Identity;

namespace sharpness_sharp.Data;

public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;
}
