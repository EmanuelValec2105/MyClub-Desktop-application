﻿using BusinessLogicLayer.Services;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using PresentationLayer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Paragraph = iTextSharp.text.Paragraph;

namespace PresentationLayer.UserControls
{
    /// <summary>
    /// Interaction logic for UcAttendanceReport.xaml
    /// </summary>
    
    //Valec kompletno
    public partial class UcAttendanceReport : UserControl
    {
        private readonly AttendanceService attendanceService = new AttendanceService();
        private readonly UserService userService = new UserService();
        public UcAttendanceReport()
        {
            InitializeComponent();
            InitializeDates();
            LoadData();
        }

        private void InitializeDates()
        {
            dpEndDate.SelectedDate = DateTime.Today;
            dpStartDate.SelectedDate = DateTime.Today.AddDays(-30);
            cbEventType.SelectedIndex = 0;
        }

        private void LoadData()
        {
            var startDate = dpStartDate.SelectedDate ?? DateTime.Today.AddDays(-30);
            var endDate = dpEndDate.SelectedDate ?? DateTime.Today;
            var eventType = cbEventType.SelectedIndex;

            UpdateReport(startDate, endDate, eventType);
        }

        private void UpdateReport(DateTime startDate, DateTime endDate, int eventType)
        {
            var teamId = (int)CurrentUser.User.TeamID;
            var attendances = attendanceService.GetTeamAttendancesForPeriod(teamId, startDate, endDate);
            attendances = attendanceService.FilterAttendancesByEventType(attendances, eventType);

            var athletes = userService.GetUsersFromTeam(teamId);

            var reportData = athletes.Select(athlete =>
            {
                var athleteAttendances = attendanceService.GetAttendancesForUser(attendances, athlete.UserID);
                var athleteTotalEvents = athleteAttendances.Count;
                var presentCount = attendanceService.CountAttendancesByStatus(athleteAttendances, 4);
                var absentCount = athleteTotalEvents - presentCount;

                return new AthleteReportModel
                {
                    FullName = $"{athlete.FirstName} {athlete.LastName}",
                    TotalEvents = athleteTotalEvents,
                    Attendances = presentCount,
                    Absences = absentCount,
                    AttendancePercentage = athleteTotalEvents > 0
                        ? Math.Round((double)presentCount / athleteTotalEvents * 100, 1)
                        : 0
                };
            }).ToList();

            var totalEvents = reportData.Sum(r => r.TotalEvents);
            var totalAttendances = reportData.Sum(r => r.Attendances);
            var totalAbsences = reportData.Sum(r => r.Absences);

            txtTotalEvents.Text = totalEvents.ToString();
            txtAverageAttendance.Text = totalEvents > 0
                ? $"{Math.Round((double)totalAttendances / totalEvents * 100, 1)}%"
                : "0%";
            txtTotalAbsences.Text = totalAbsences.ToString();

            dgAthletes.ItemsSource = reportData;
        }

        private void ShowMessage(string message, bool isError = true)
        {
            lblMessage.Content = message;
            lblMessage.Foreground = isError ? Brushes.Red : Brushes.Green;
            lblMessage.Visibility = Visibility.Visible;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (!dpStartDate.SelectedDate.HasValue || !dpEndDate.SelectedDate.HasValue)
            {
                ShowMessage("Please select both dates!");
                return;
            }

            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                ShowMessage("Start date cannot be after end date!");
                return;
            }

            if (dpEndDate.SelectedDate > DateTime.Today)
            {
                ShowMessage("End date cannot be in the future!");
                return;
            }

            LoadData();
            lblMessage.Visibility = Visibility.Collapsed;
        }

        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = dgAthletes.ItemsSource as List<AthleteReportModel>;
                if (data == null || !data.Any())
                {
                    ShowMessage("No data to export!");
                    return;
                }
                var dialog = new SaveFileDialog
                {
                    Filter = "PDF Files|*.pdf",
                    DefaultExt = ".pdf",
                    FileName = $"TeamReport_{DateTime.Now:yyyyMMdd}"
                };
                if (dialog.ShowDialog() == true)
                {
                    using (FileStream fs = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        var document = new Document(PageSize.A4, 25, 25, 30, 30);
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);
                        document.Open();
                        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                        var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                        document.Add(new Paragraph("Team Report", titleFont));
                        document.Add(new Paragraph($"Period: {dpStartDate.SelectedDate:dd.MM.yyyy} - {dpEndDate.SelectedDate:dd.MM.yyyy}", normalFont));
                        document.Add(new Paragraph($"Event Type: {cbEventType.Text}", normalFont));
                        document.Add(new Paragraph("\n"));
                        document.Add(new Paragraph("Summary Statistics", titleFont));
                        document.Add(new Paragraph($"Total Events: {txtTotalEvents.Text}", normalFont));
                        document.Add(new Paragraph($"Average Attendance: {txtAverageAttendance.Text}", normalFont));
                        document.Add(new Paragraph($"Total Absences: {txtTotalAbsences.Text}", normalFont));
                        document.Add(new Paragraph("\n"));
                        PdfPTable table = new PdfPTable(5);
                        table.WidthPercentage = 100;
                        table.SetWidths(new float[] { 3f, 2f, 2f, 2f, 2f });
                        var headers = new[] { "Name", "Total Events", "Attendances", "Absences", "Attendance %" };
                        foreach (var header in headers)
                        {
                            var cell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = BaseColor.LIGHT_GRAY
                            };
                            table.AddCell(cell);
                        }
                        foreach (var athlete in data)
                        {
                            table.AddCell(new Phrase(athlete.FullName, normalFont));
                            table.AddCell(new Phrase(athlete.TotalEvents.ToString(), normalFont));
                            table.AddCell(new Phrase(athlete.Attendances.ToString(), normalFont));
                            table.AddCell(new Phrase(athlete.Absences.ToString(), normalFont));
                            table.AddCell(new Phrase($"{athlete.AttendancePercentage}%", normalFont));
                        }
                        document.Add(table);
                        document.Close();
                    }
                    ShowMessage("Report exported successfully!", false);
                    lblMessage.Visibility = Visibility.Visible;
                }
            } catch (Exception ex)
            {
                ShowMessage($"Error exporting report: {ex.Message}");
            }
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = dgAthletes.ItemsSource as List<AthleteReportModel>;
                if (data == null || !data.Any())
                {
                    ShowMessage("No data to export!");
                    return;
                }

                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Attendance Report");

                worksheet.Cell("A1").Value = "Attendance Report";
                worksheet.Cell("A2").Value = $"Period: {dpStartDate.SelectedDate:dd.MM.yyyy} - {dpEndDate.SelectedDate:dd.MM.yyyy}";
                worksheet.Cell("A3").Value = $"Event Type: {cbEventType.Text}";

                worksheet.Cell("A5").Value = "Summary Statistics";
                worksheet.Cell("A6").Value = "Total Events";
                worksheet.Cell("B6").Value = txtTotalEvents.Text;
                worksheet.Cell("A7").Value = "Average Attendance";
                worksheet.Cell("B7").Value = txtAverageAttendance.Text;
                worksheet.Cell("A8").Value = "Total Absences";
                worksheet.Cell("B8").Value = txtTotalAbsences.Text;

                int rowIndex = 10;
                worksheet.Cell(rowIndex, 1).Value = "Name";
                worksheet.Cell(rowIndex, 2).Value = "Total Events";
                worksheet.Cell(rowIndex, 3).Value = "Attendances";
                worksheet.Cell(rowIndex, 4).Value = "Absences";
                worksheet.Cell(rowIndex, 5).Value = "Attendance %";

                foreach (var athlete in data)
                {
                    rowIndex++;
                    worksheet.Cell(rowIndex, 1).Value = athlete.FullName;
                    worksheet.Cell(rowIndex, 2).Value = athlete.TotalEvents;
                    worksheet.Cell(rowIndex, 3).Value = athlete.Attendances;
                    worksheet.Cell(rowIndex, 4).Value = athlete.Absences;
                    worksheet.Cell(rowIndex, 5).Value = athlete.AttendancePercentage;
                }

                var headerRow = worksheet.Row(10);
                headerRow.Style.Font.Bold = true;
                worksheet.Columns().AdjustToContents();

                var dialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    DefaultExt = ".xlsx",
                    FileName = $"AttendanceReport_{DateTime.Now:yyyyMMdd}"
                };

                if (dialog.ShowDialog() == true)
                {
                    workbook.SaveAs(dialog.FileName);
                    ShowMessage("Report exported successfully!", false);
                    lblMessage.Visibility = Visibility.Visible;
                }
            } catch (Exception ex)
            {
                ShowMessage($"Error exporting report: {ex.Message}");
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(dgAthletes, "Attendance Report");
                    ShowMessage("Report printed successfully!", false);
                }
            } catch (Exception ex)
            {
                ShowMessage($"Error printing report: {ex.Message}");
            }
        }
    }
}
