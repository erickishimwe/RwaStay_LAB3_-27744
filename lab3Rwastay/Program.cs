using lab3Rwastay.Data;
using lab3Rwastay.Views;

namespace lab3Rwastay;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            AuthSchemaInitializer.Ensure();
            while (true)
            {
                using var login = new LoginForm();
                if (login.ShowDialog() != DialogResult.OK || login.AuthenticatedUser is null) break;
                using var main = new MainForm(login.AuthenticatedUser);
                if (main.ShowDialog() != DialogResult.Retry) break;
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("RwaStay could not start. Make sure MySQL is running in XAMPP and the database exists.\n\n" + exception.Message, "RwaStay", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
