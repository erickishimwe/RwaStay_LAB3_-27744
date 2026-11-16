using lab3Rwastay.Controllers;
using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Views;

public sealed class StayForm : Form
{
    private readonly StayController controller = new(new StayRepository(), new HostRepository());
    private readonly ComboBox host = new(), search = new();
    private readonly TextBox title = new(), description = new(), city = new();
    private readonly NumericUpDown price = new() { DecimalPlaces = 2, Maximum = 100000000, ThousandsSeparator = true }, capacity = new() { Minimum = 1, Maximum = 1000, Value = 1 };
    private readonly CheckBox available = new() { Text = "Available", Checked = true, AutoSize = true };
    private readonly DataGridView grid = new();
    private int selectedId;

    public StayForm()
    {
        Text = "Stays"; StartPosition = FormStartPosition.CenterParent; Size = new Size(1100, 650);
        host.DropDownStyle = ComboBoxStyle.DropDownList; host.DisplayMember = nameof(Host.Name); host.ValueMember = nameof(Host.Id);
        var editor = new TableLayoutPanel { Dock = DockStyle.Top, Height = 170, ColumnCount = 4, RowCount = 4, Padding = new Padding(8) };
        for (var i = 0; i < 4; i++) editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        AddEditor(editor, "Title", title, 0, 0); AddEditor(editor, "Host", host, 1, 0); AddEditor(editor, "City", city, 2, 0); AddEditor(editor, "Capacity", capacity, 3, 0);
        AddEditor(editor, "Price/night", price, 0, 2); AddEditor(editor, "Description", description, 1, 2); editor.SetColumnSpan(description, 2); editor.Controls.Add(available, 3, 2);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill }; var save = UiHelpers.Button("Save"); save.Click += (_, _) => Save(); var clear = UiHelpers.Button("Clear"); clear.Click += (_, _) => Clear(); actions.Controls.Add(save); actions.Controls.Add(clear); editor.Controls.Add(actions, 3, 3);
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) }; toolbar.Controls.Add(UiHelpers.Label("Search:")); search.Width = 250; toolbar.Controls.Add(search); var find = UiHelpers.Button("Search"); find.Click += (_, _) => LoadRows(); var delete = UiHelpers.Button("Delete"); delete.Click += (_, _) => Delete(); toolbar.Controls.Add(find); toolbar.Controls.Add(delete);
        UiHelpers.ConfigureGrid(grid); grid.SelectionChanged += (_, _) => SelectRow(); Controls.Add(grid); Controls.Add(toolbar); Controls.Add(editor); LoadHosts(); LoadRows();
    }
    private static void AddEditor(TableLayoutPanel panel, string label, Control control, int column, int row) { panel.Controls.Add(UiHelpers.Label(label), column, row); control.Dock = DockStyle.Fill; panel.Controls.Add(control, column, row + 1); }
    private void LoadHosts() { try { host.DataSource = controller.GetHosts(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void LoadRows() { try { grid.DataSource = controller.GetAll(search.Text); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void SelectRow() { if (grid.CurrentRow?.DataBoundItem is not Stay stay) return; selectedId = stay.Id; title.Text = stay.Title; description.Text = stay.Description; city.Text = stay.City; price.Value = Math.Min(price.Maximum, stay.PricePerNight); capacity.Value = stay.Capacity; available.Checked = stay.IsAvailable; host.SelectedValue = stay.HostId; }
    private void Save() { try { var selectedHost = host.SelectedItem as Host; controller.Save(new Stay { Id = selectedId, HostId = selectedHost?.Id ?? 0, Title = title.Text.Trim(), Description = description.Text.Trim(), City = city.Text.Trim(), PricePerNight = price.Value, Capacity = (int)capacity.Value, IsAvailable = available.Checked }); MessageBox.Show("Stay saved.", "RwaStay", MessageBoxButtons.OK, MessageBoxIcon.Information); Clear(); LoadRows(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void Delete() { try { if (selectedId > 0 && UiHelpers.ConfirmDelete()) { controller.Delete(selectedId); Clear(); LoadRows(); } } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void Clear() { selectedId = 0; title.Clear(); description.Clear(); city.Clear(); price.Value = 0; capacity.Value = 1; available.Checked = true; grid.ClearSelection(); }
}
