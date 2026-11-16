using MySqlConnector;

namespace lab3Rwastay.Views;

internal static class UiHelpers
{
    public static Label Label(string text) => new() { Text = text, AutoSize = true, Anchor = AnchorStyles.Left };

    public static Button Button(string text) => new() { Text = text, AutoSize = true, Padding = new Padding(8, 3, 8, 3) };

    public static void ConfigureGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoGenerateColumns = true;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AllowUserToAddRows = false;
    }

    public static void ShowError(Exception exception)
    {
        var message = exception switch
        {
            MySqlException => "Could not access MySQL. Make sure MySQL is running in XAMPP and the rwastaydb database exists.",
            ArgumentException => exception.Message,
            _ => $"Something went wrong: {exception.Message}"
        };
        MessageBox.Show(message, "RwaStay", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static bool ConfirmDelete() => MessageBox.Show("Delete the selected record?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
}
