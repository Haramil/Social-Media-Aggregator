using Microsoft.AspNet.Identity.EntityFramework;

public class ApplicationUser : IdentityUser
{
    public string Login { get; set; }
    public ApplicationUser()
    {
    }
}