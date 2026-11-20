using lab3Rwastay.Controllers;
using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Views;

public sealed class LoginForm : Form
{
    private readonly AuthController controller = new(new UserRepository());
    private readonly TextBox username = new(), password = new() { UseSystemPasswordChar = true };
    public User? AuthenticatedUser { get; private set; }

    public LoginForm()
    {
        Text = "RwaStay Login"; StartPosition = FormStartPosition.CenterScreen; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false; Size = new Size(420, 300);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(25), ColumnCount = 2, RowCount = 5 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        layout.Controls.Add(new Label { Text = "RwaStay", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.DarkGreen, AutoSize = true }, 0, 0); layout.SetColumnSpan(layout.GetControlFromPosition(0, 0)!, 2);
        Add(layout, "Username", username, 1); Add(layout, "Password", password, 2);
        var login = UiHelpers.Button("Login"); login.Click += (_, _) => Login(); var register = UiHelpers.Button("Client registration"); register.Click += (_, _) => Register(); var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill }; buttons.Controls.Add(login); buttons.Controls.Add(register); layout.Controls.Add(buttons, 0, 4); layout.SetColumnSpan(buttons, 2);
        AcceptButton = login; Controls.Add(layout);
    }
    private static void Add(TableLayoutPanel layout, string label, Control control, int row) { layout.Controls.Add(UiHelpers.Label(label), 0, row); control.Dock = DockStyle.Fill; layout.Controls.Add(control, 1, row); }
    private void Login() { try { AuthenticatedUser = controller.Login(username.Text, password.Text); DialogResult = DialogResult.OK; Close(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void Register() { using var form = new RegisterForm(controller); if (form.ShowDialog(this) == DialogResult.OK) MessageBox.Show("Registration successful. You can now login.", "RwaStay", MessageBoxButtons.OK, MessageBoxIcon.Information); }
}
