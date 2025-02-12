﻿using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using PresentationLayer.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PresentationLayer.UserControls
{
    /// <summary>
    /// Interaction logic for UcMatchManagement.xaml
    /// </summary>
    public partial class UcCoachMatchManagement : UserControl
    {
        private MatchManagementService _matchManagementService = new MatchManagementService();

        public DateTime TodayDate => DateTime.Today;

        public UcCoachMatchManagement()
        {
            InitializeComponent();
        }

        public async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.User.RoleID != 2)
            {
                var attendanceColumn = dgCoachGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Attendance");
                if (attendanceColumn != null)
                {
                    attendanceColumn.Visibility = Visibility.Collapsed;
                }
            }
            await LoadMatches();
        }

        public async Task LoadMatches()
        {
            if (!CurrentUser.User.TeamID.HasValue && CurrentUser.User.RoleID == 2)
            {
                ShowToast("You aren't assigned to a team.");
                return;
            }

            if (CurrentUser.User.RoleID == 1)
            {
                var fetchedMatches = await _matchManagementService.GetMatches();
                if (fetchedMatches == null || fetchedMatches.Count == 0)
                {
                    MessageBox.Show("There are no data to be shown.");
                    return;
                }
                dgCoachGrid.ItemsSource = fetchedMatches;
            }
            else
            {
                int teamId = (int)CurrentUser.User.TeamID;
                var fetchedMatches = await _matchManagementService.GetMatchesByTeamId(teamId);
                if (fetchedMatches == null || fetchedMatches.Count == 0)
                {
                    MessageBox.Show("There are no data to be shown.");
                    return;
                }
                dgCoachGrid.ItemsSource = fetchedMatches;
            }
            if (CurrentUser.User.RoleID == 2)
            {
                foreach (var item in dgCoachGrid.Items)
                {
                    var match = item as Match;
                    if (match != null && match.MatchDate < DateTime.Now && match.Status == "Scheduled")
                    {
                        ShowToast("You have matches that have already been played. Please update the results.");
                    }
                }
            }
        }

        public void btnAddMatch_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.User.TeamID != null || CurrentUser.User.RoleID == 1)
            {
                GuiManager.OpenContent(new UcAddMatch());
            }
            else
            {
                ShowToast("You cannot add matches if you're not party of a team");
            }
        }
        //Černjević
        public void btnUpdateMatch_Click(object sender, RoutedEventArgs e)
        {
            EntitiesLayer.Entities.Match match = GetMatch();
            if (match != null)
            {
                if (match.Status == "Cancelled")
                {
                    ShowToast("Cannot update cancelled matches! Only matches that have already been played can be updated.");
                    return;
                }
                else if (match.MatchDate > DateTime.Now)
                {
                    GuiManager.OpenContent(new UcEditMatch(match));
                }
                //ako je utakmica prošla, otvori formu za unos rezultata
                else
                {
                    GuiManager.OpenContent(new UcUpdateMatch(match));
                }
            }
            else
            {
                ShowToast("Please select a match.");
                return;
            }
        }
        //Černjević
        public void btnDeleteMatch_Click(object sender, RoutedEventArgs e)
        {
            EntitiesLayer.Entities.Match match = GetMatch();
            if (match != null)
            {
                //prikaz poruke s potvrdom brisanja
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this match",
                    "Confirm delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MatchManagementService _matchManagementService = new MatchManagementService();
                    _matchManagementService.RemoveMatch(match);
                    _ = LoadMatches();
                }
                else
                {
                    ShowToast("Delete cancelled.");
                }
            }
            else
            {
                ShowToast("Please select a match.");
            }
        }
        //Černjević
        public void btnCancelMatch_Click(object sender, RoutedEventArgs e)
        {
            EntitiesLayer.Entities.Match match = GetMatch();
            if (match != null)
            {
                GuiManager.OpenContent(new UcCancelMatch(match));
            }
            else
            {
                ShowToast("Please select a match.");
            }
        }

        public EntitiesLayer.Entities.Match GetMatch()
        {
            if (dgCoachGrid.SelectedItem == null)
            {
                return null;
            }
            else
            {
                return dgCoachGrid.SelectedItem as EntitiesLayer.Entities.Match;
            }
        }

        //Valec
        private void btnAttendance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var match = button.DataContext as EntitiesLayer.Entities.Match;
            if (match != null)
            {
                var attendanceControl = new UcMatchAttendanceCoach(match);
                GuiManager.OpenContent(attendanceControl);
            }
        }

        private void ShowToast(string message)
        {
            ToastWindow toast = new ToastWindow(message);
            toast.Show();
        }
    }
}
