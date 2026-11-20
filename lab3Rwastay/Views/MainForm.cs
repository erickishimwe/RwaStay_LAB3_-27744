using lab3Rwastay.Controllers;
using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Views;

public sealed class MainForm : Form
{
    private readonly User currentUser;

    public MainForm(User currentUser)
    {
        this.currentUser = currentUser;
        Text = $"RwaStay - {currentUser.Role} dashboard";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(700, 450);
        Size = new Size(900, 600);
        BackColor = Color.White;

        var title = new Label { Text = "RwaStay", Font = new Font("Segoe UI", 26, FontStyle.Bold), ForeColor = Color.DarkGreen, Dock = DockStyle.Top, Height = 65, TextAlign = ContentAlignment.MiddleCenter };
        var subtitle = new Label { Text = $"Welcome, {currentUser.DisplayName} ({currentUser.Role})", Dock = DockStyle.Top, Height = 35, TextAlign = ContentAlignment.TopCenter };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(30), AutoScroll = true };
        if (currentUser.Role == UserRole.Admin)
        {
            buttons.Controls.Add(CreateButton("Manage Hosts", (_, _) => Open(() => new HostForm())));
            buttons.Controls.Add(CreateButton("Manage Stays", (_, _) => Open(() => new StayForm())));
            buttons.Controls.Add(CreateButton("Manage Guests", (_, _) => Open(() => new GuestForm())));
        }
        if (currentUser.Role is UserRole.Admin or UserRole.Manager or UserRole.Client)
            buttons.Controls.Add(CreateButton("Bookings", (_, _) => Open(() => new BookingForm(currentUser))));
        buttons.Controls.Add(CreateButton("Test MySQL Connection", (_, _) => TestConnection()));
        buttons.Controls.Add(CreateButton("Logout", (_, _) => { DialogResult = DialogResult.Retry; Close(); }));
        Controls.Add(buttons); Controls.Add(subtitle); Controls.Add(title);
    }

    private static Button CreateButton(string text, EventHandler handler)
    {
        var button = UiHelpers.Button(text); button.Width = 280; button.Height = 45; button.Margin = new Padding(5, 5, 5, 10); button.Click += handler; return button;
    }

    private void Open(Func<Form> formFactory)
    {
        try
        {
            using var form = formFactory();
            form.ShowDialog(this);
        }
        catch (Exception exception)
        {
            UiHelpers.ShowError(exception);
        }
    }

    private void TestConnection()
    {
        try { DbConnection.TestAsync().GetAwaiter().GetResult(); MessageBox.Show("MySQL connection succeeded.", "RwaStay", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        catch (Exception exception) { UiHelpers.ShowError(exception); }
    }
}
