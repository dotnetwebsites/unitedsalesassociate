using System.Net;
using System.Net.Mail;

namespace UnitedSales.Areas.Identity.Data
{
    public static class MailData
    {
        // public static MailAddress FromAddress = new MailAddress("samyraglobal@gmail.com", "Do-not-reply");
        // public static string Password = "Google@143#$%";
        // public static string Host = "smtp.gmail.com";
        // public static int Port = 587;
        // public static bool EnableSsl = true;
        // public static bool UseDefaultCredentials = false;
        public static MailAddress FromAddress = new MailAddress("info@focushrms.com", "Do-not-reply");
        public static string Password = "5dSGnxjKD";
        public static string Host = "mail.focushrms.com";
        public static int Port = 587;
        public static bool EnableSsl = false;
        public static bool UseDefaultCredentials = false;
    }

}