using lab3Rwastay.Controllers;
using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Views;

public sealed class HostForm : Form
{
    private readonly HostController controller = new(new HostRepository());
    private readonly TextBox name = new(), phone = new(), email = new(), city = new(), search = new();
    private readonly DataGridView grid = new();
    private int selectedId;

    public HostForm()
    {
        Text = "Hosts"; StartPosition = FormStartPosition.CenterParent; Size = new Size(950, 600);
        var editor = new TableLayoutPanel { Dock = DockStyle.Top, Height = 100, ColumnCount = 5, RowCount = 2, Padding = new Padding(8) };
        editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25)); editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20)); editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        AddEditor(editor, "Name", name, 0); AddEditor(editor, "Phone", phone, 1); AddEditor(editor, "Email", email, 2); AddEditor(editor, "City", city, 3);
        var save = UiHelpers.Button("Save"); save.Click += (_, _) => Save(); var clear = UiHelpers.Button("Clear"); clear.Click += (_, _) => Clear();
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill }; actions.Controls.Add(save); actions.Controls.Add(clear); editor.Controls.Add(actions, 4, 1);
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) }; toolbar.Controls.Add(UiHelpers.Label("Search:")); search.Width = 250; toolbar.Controls.Add(search); var find = UiHelpers.Button("Search"); find.Click += (_, _) => LoadRows(); var delete = UiHelpers.Button("Delete"); delete.Click += (_, _) => Delete(); toolbar.Controls.Add(find); toolbar.Controls.Add(delete);
        UiHelpers.ConfigureGrid(grid); grid.SelectionChanged += (_, _) => SelectRow(); Controls.Add(grid); Controls.Add(toolbar); Controls.Add(editor); LoadRows();
    }
    private static void AddEditor(TableLayoutPanel panel, string label, Control control, int column) { panel.Controls.Add(UiHelpers.Label(label), column, 0); control.Dock = DockStyle.Fill; panel.Controls.Add(control, column, 1); }
    private void LoadRows() { try { grid.DataSource = controller.GetAll(search.Text); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void SelectRow() { if (grid.CurrentRow?.DataBoundItem is not Host host) return; selectedId = host.Id; name.Text = host.Name; phone.Text = host.Phone; email.Text = host.Email; city.Text = host.City; }
    private void Save() { try { controller.Save(new Host { Id = selectedId, Name = name.Text.Trim(), Phone = phone.Text.Trim(), Email = email.Text.Trim(), City = city.Text.Trim() }); MessageBox.Show("Host saved.", "RwaStay", MessageBoxButtons.OK, MessageBoxIcon.Information); Clear(); LoadRows(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void Delete() { try { if (selectedId > 0 && UiHelpers.ConfirmDelete()) { controller.Delete(selectedId); Clear(); LoadRows(); } } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void Clear() { selectedId = 0; name.Clear(); phone.Clear(); email.Clear(); city.Clear(); grid.ClearSelection(); }
}
