﻿using BusinessLogicLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PresentationLayer.UserControls
{
    /// <summary>
    /// Interaction logic for UcAddMatch.xaml
    /// </summary>
    public partial class UcUpdateMatch : UserControl
    {
        private EntitiesLayer.Entities.Match match;

        public UcUpdateMatch(EntitiesLayer.Entities.Match fetchedMatch)
        {
            InitializeComponent();
            match = fetchedMatch;
        }

        private void btnUpdateMatch_Click(object sender, RoutedEventArgs e)
        {
            var _matchService = new MatchManagementService();
            if (match == null)
            {
                MessageBox.Show("Match not found!");
                return;
            }

            string result = txtResult.Text;
            string summary = txtSummary.Text;

            string resultPattern = @"^([0-9]|[1-9][0-9])[-:]([0-9]|[1-9][0-9])$";

            if (string.IsNullOrEmpty(summary) || string.IsNullOrEmpty(result))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }
            else if (!Regex.IsMatch(result, resultPattern))
            {
                MessageBox.Show("Invalid result format. Please enter in format '0:0', '99:99', etc.");
                return;
            }
            else
            {
                match.Result = result;
                match.Summary = summary;
                match.Status = cmbStatus.SelectedValue.ToString();
                bool a = _matchService.UpdateMatch(match);
                if (a)
                {
                    MessageBox.Show("Match updated successfully!");
                }
                else
                {
                    MessageBox.Show("Match update failed!");
                }
            }
            GuiManager.CloseContent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            GuiManager.CloseContent();
        }
    }
}
