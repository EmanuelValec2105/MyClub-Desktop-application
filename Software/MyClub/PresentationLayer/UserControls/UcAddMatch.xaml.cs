﻿using System;
using BusinessLogicLayer;
using EntitiesLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PresentationLayer.Helper;
using System.Text.RegularExpressions;
using BusinessLogicLayer.Services;

namespace PresentationLayer.UserControls
{
    /// <summary>
    /// Interaction logic for UcAddatch.xaml
    /// </summary>
    public partial class UcAddMatch : UserControl
    {
        MatchManagementService _matchManagementService = new MatchManagementService();

        public UcAddMatch()
        {
            InitializeComponent();
        }

        private void btnAddMatch_Click(object sender, RoutedEventArgs e)
        {
            string opponentTeam = txtOpponent.Text;
            string location = txtLocation.Text;
            string startTime = txtStartTime.Text;
            DateTime matchDate = dtMatchDate.SelectedDate.Value;

            string timePattern = @"^([01]\d|2[0-3]):[0-5]\d$"; // HH:mm format (24h)
            if (string.IsNullOrEmpty(opponentTeam) || string.IsNullOrEmpty(location) || string.IsNullOrEmpty(startTime) || dtMatchDate.SelectedDate == null)
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }
            else if (matchDate < DateTime.Now)
            {
                MessageBox.Show("Match date cannot be in the past.");
                return;
            }
            else if (!Regex.IsMatch(startTime, timePattern))
            {
                MessageBox.Show("Invalid time format. Please enter time in HH:mm format.");
                return;
            }
            else
            {
                var match = new EntitiesLayer.Entities.Match
                {
                    TeamID = (int)CurrentUser.User.TeamID,
                    MatchDate = matchDate,
                    OpponentTeam = opponentTeam,
                    Location = location,
                    StartTime = TimeSpan.Parse(startTime),
                    Status = "Scheduled"
                };
                _matchManagementService.AddMatch(match);
            }

            GuiManager.CloseContent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            GuiManager.CloseContent();
        }
    }
}
