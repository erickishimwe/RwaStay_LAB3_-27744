using lab3Rwastay.Controllers;
using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Views;

public sealed class GuestForm : Form
{
    private readonly GuestController controller = new(new GuestRepository());
    private readonly TextBox fullName = new(), email = new(), phone = new(), search = new();
    private readonly DataGridView grid = new();
    private int selectedId;

    public GuestForm()
    {
        Text = "Guests"; StartPosition = FormStartPosition.CenterParent; Size = new Size(900, 550);
        var editor = new TableLayoutPanel { Dock = DockStyle.Top, Height = 100, ColumnCount = 4, RowCount = 2, Padding = new Padding(8) };
        for (var i = 0; i < 4; i++) editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        AddEditor(editor, "Full name", fullName, 0); AddEditor(editor, "Email", email, 1); AddEditor(editor, "Phone", phone, 2);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill }; var save = UiHelpers.Button("Save"); save.Click += (_, _) => Save(); var clear = UiHelpers.Button("Clear"); clear.Click += (_, _) => Clear(); actions.Controls.Add(save); actions.Controls.Add(clear); editor.Controls.Add(actions, 3, 1);
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) }; toolbar.Controls.Add(UiHelpers.Label("Search:")); search.Width = 250; toolbar.Controls.Add(search); var find = UiHelpers.Button("Search"); find.Click += (_, _) => LoadRows(); var delete = UiHelpers.Button("Delete"); delete.Click += (_, _) => Delete(); toolbar.Controls.Add(find); toolbar.Controls.Add(delete);
        UiHelpers.ConfigureGrid(grid); grid.SelectionChanged += (_, _) => SelectRow(); Controls.Add(grid); Controls.Add(toolbar); Controls.Add(editor); LoadRows();
    }
    private static void AddEditor(TableLayoutPanel panel, string label, Control control, int column) { panel.Controls.Add(UiHelpers.Label(label), column, 0); control.Dock = DockStyle.Fill; panel.Controls.Add(control, column, 1); }
    private void LoadRows() { try { grid.DataSource = controller.GetAll(search.Text); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void SelectRow() { if (grid.CurrentRow?.DataBoundItem is not Guest guest) return; selectedId = guest.Id; fullName.Text = guest.FullName; email.Text = guest.Email; phone.Text = guest.Phone; }
    private void Save() { try { controller.Save(new Guest { Id = selectedId, FullName = fullName.Text.Trim(), Email = email.Text.Trim(), Phone = phone.Text.Trim() }); MessageBox.Show("Guest saved.", "RwaStay", MessageBoxButtons.OK, MessageBoxIcon.Information); Clear(); LoadRows(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void Delete() { try { if (selectedId > 0 && UiHelpers.ConfirmDelete()) { controller.Delete(selectedId); Clear(); LoadRows(); } } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void Clear() { selectedId = 0; fullName.Clear(); email.Clear(); phone.Clear(); grid.ClearSelection(); }
}
