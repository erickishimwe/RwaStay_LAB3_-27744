using lab3Rwastay.Controllers;
using lab3Rwastay.Data;

namespace lab3Rwastay.Views;

public sealed class MainForm : Form
{
    public MainForm()
    {
        Text = "RwaStay - Rwanda Stay Booking System";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(700, 450);
        Size = new Size(900, 600);
        BackColor = Color.White;

        var title = new Label { Text = "RwaStay", Font = new Font("Segoe UI", 26, FontStyle.Bold), ForeColor = Color.DarkGreen, Dock = DockStyle.Top, Height = 65, TextAlign = ContentAlignment.MiddleCenter };
        var subtitle = new Label { Text = "Manage hosts, stays, guests, and bookings", Dock = DockStyle.Top, Height = 35, TextAlign = ContentAlignment.TopCenter };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(30), AutoScroll = true };
        buttons.Controls.Add(CreateButton("Manage Hosts", (_, _) => Open(() => new HostForm())));
        buttons.Controls.Add(CreateButton("Manage Stays", (_, _) => Open(() => new StayForm())));
        buttons.Controls.Add(CreateButton("Manage Guests", (_, _) => Open(() => new GuestForm())));
        buttons.Controls.Add(CreateButton("Manage Bookings", (_, _) => Open(() => new BookingForm())));
        buttons.Controls.Add(CreateButton("Test MySQL Connection", (_, _) => TestConnection()));
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
