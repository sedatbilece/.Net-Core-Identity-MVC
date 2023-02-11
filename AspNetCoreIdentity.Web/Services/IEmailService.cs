namespace AspNetCoreIdentity.Web.Services
{
    public interface IEmailService
    {

        Task SendResetEmail(string resetLink, string toEmail);
    }
}
