using lab3Rwastay.Controllers;

namespace lab3Rwastay.Views;

public sealed class RegisterForm : Form
{
    private readonly AuthController controller;
    private readonly TextBox username = new(), password = new() { UseSystemPasswordChar = true }, confirm = new() { UseSystemPasswordChar = true }, fullName = new(), email = new(), phone = new();
    public RegisterForm(AuthController controller)
    {
        this.controller = controller; Text = "Client Registration"; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; Size = new Size(520, 390);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2, RowCount = 8 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        Add(layout, "Username", username, 0); Add(layout, "Password", password, 1); Add(layout, "Confirm password", confirm, 2); Add(layout, "Full name", fullName, 3); Add(layout, "Email", email, 4); Add(layout, "Phone", phone, 5);
        var save = UiHelpers.Button("Create client account"); save.Click += (_, _) => Save(); layout.Controls.Add(save, 1, 7); AcceptButton = save; Controls.Add(layout);
    }
    private static void Add(TableLayoutPanel layout, string label, Control control, int row) { layout.Controls.Add(UiHelpers.Label(label), 0, row); control.Dock = DockStyle.Fill; layout.Controls.Add(control, 1, row); }
    private void Save() { try { if (password.Text != confirm.Text) throw new ArgumentException("Passwords do not match."); controller.RegisterClient(username.Text, password.Text, fullName.Text, email.Text, phone.Text); DialogResult = DialogResult.OK; Close(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
}
