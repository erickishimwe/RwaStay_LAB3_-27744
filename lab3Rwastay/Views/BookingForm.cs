using lab3Rwastay.Controllers;
using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Views;

public sealed class BookingForm : Form
{
    private readonly BookingController controller;
    private readonly User currentUser;
    private readonly ComboBox guest = new(), stay = new(), status = new();
    private readonly DateTimePicker checkIn = new() { Format = DateTimePickerFormat.Short, MinDate = DateTime.Today };
    private readonly DateTimePicker checkOut = new() { Format = DateTimePickerFormat.Short, MinDate = DateTime.Today.AddDays(1) };
    private readonly Label total = new() { Text = "Total: 0 RWF", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
    private readonly DataGridView grid = new();
    private int selectedId;

    public BookingForm(User currentUser)
    {
        this.currentUser = currentUser;
        controller = new BookingController(new BookingRepository(), new GuestRepository(), new StayRepository(), currentUser);
        Text = "Bookings"; StartPosition = FormStartPosition.CenterParent; Size = new Size(1100, 650);
        guest.DropDownStyle = ComboBoxStyle.DropDownList; guest.DisplayMember = nameof(Guest.FullName); guest.ValueMember = nameof(Guest.Id);
        stay.DropDownStyle = ComboBoxStyle.DropDownList; stay.DisplayMember = nameof(Stay.Title); stay.ValueMember = nameof(Stay.Id);
        status.DropDownStyle = ComboBoxStyle.DropDownList; status.DataSource = Enum.GetValues<BookingStatus>();
        var editor = new TableLayoutPanel { Dock = DockStyle.Top, Height = 135, ColumnCount = 6, RowCount = 2, Padding = new Padding(8) };
        for (var i = 0; i < 6; i++) editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66f));
        AddEditor(editor, "Guest", guest, 0); AddEditor(editor, "Stay", stay, 1); AddEditor(editor, "Check-in", checkIn, 2); AddEditor(editor, "Check-out", checkOut, 3); AddEditor(editor, "Status", status, 4); editor.Controls.Add(total, 5, 1);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) }; var create = UiHelpers.Button("Create booking"); create.Click += (_, _) => Create(); var change = UiHelpers.Button("Change status"); change.Click += (_, _) => ChangeStatus(); var cancel = UiHelpers.Button("Cancel booking"); cancel.Click += (_, _) => Cancel(); var delete = UiHelpers.Button("Delete"); delete.Click += (_, _) => Delete(); if (currentUser.Role != UserRole.Manager) actions.Controls.Add(create); if (currentUser.Role == UserRole.Admin) actions.Controls.Add(change); if (currentUser.Role is UserRole.Admin or UserRole.Client) actions.Controls.Add(cancel); if (currentUser.Role == UserRole.Admin) actions.Controls.Add(delete);
        checkIn.ValueChanged += (_, _) => UpdateTotal(); checkOut.ValueChanged += (_, _) => UpdateTotal(); stay.SelectedIndexChanged += (_, _) => UpdateTotal();
        UiHelpers.ConfigureGrid(grid); grid.SelectionChanged += (_, _) => SelectRow(); Controls.Add(grid); Controls.Add(actions); Controls.Add(editor); LoadLookups(); LoadRows();
    }
    private static void AddEditor(TableLayoutPanel panel, string label, Control control, int column) { panel.Controls.Add(UiHelpers.Label(label), column, 0); control.Dock = DockStyle.Fill; panel.Controls.Add(control, column, 1); }
    private void LoadLookups() { try { guest.DataSource = controller.GetGuests(); stay.DataSource = controller.GetAvailableStays(); UpdateTotal(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void LoadRows() { try { grid.DataSource = controller.GetAll(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void UpdateTotal() { if (stay.SelectedItem is Stay selectedStay && checkOut.Value.Date > checkIn.Value.Date) total.Text = $"Total: {controller.CalculateTotal(selectedStay, checkIn.Value, checkOut.Value):N0} RWF"; else total.Text = "Total: 0 RWF"; }
    private void SelectRow() { if (grid.CurrentRow?.DataBoundItem is not Booking booking) return; selectedId = booking.Id; status.SelectedItem = booking.Status; }
    private void Create() { try { if (stay.SelectedItem is not Stay selectedStay || guest.SelectedItem is not Guest selectedGuest) throw new ArgumentException("Select a guest and a stay."); controller.Create(new Booking { GuestId = selectedGuest.Id, StayId = selectedStay.Id, CheckIn = checkIn.Value.Date, CheckOut = checkOut.Value.Date }, selectedStay.PricePerNight); MessageBox.Show("Booking created.", "RwaStay", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadLookups(); LoadRows(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void ChangeStatus() { try { controller.ChangeStatus(selectedId, (BookingStatus)status.SelectedItem!); LoadRows(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void Cancel() { try { controller.Cancel(selectedId); LoadRows(); } catch (Exception ex) { UiHelpers.ShowError(ex); } }
    private void Delete() { try { if (selectedId > 0 && UiHelpers.ConfirmDelete()) { controller.Delete(selectedId); selectedId = 0; LoadRows(); } } catch (Exception ex) { UiHelpers.ShowError(ex); } }
}
