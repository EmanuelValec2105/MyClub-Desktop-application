﻿using DataAccessLayer.EntityRepository;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    //Černjević kompletno
    public class MatchManagementService
    {
        private List<Match> _cachedMatches = new List<Match>();

        public async Task<List<Match>> GetMatches()
        {
            using (var repo = new MatchManagementRepository())
            {
                _cachedMatches = await repo.GetAllMatches().ToListAsync();

                return _cachedMatches;
            }
        }

        public Match GetMatchById(int matchId)
        {
            using (var repo = new MatchManagementRepository())
            {
                return repo.GetMatchById(matchId).FirstOrDefault();
            }
        }

        public async Task<List<Match>> GetMatchesByTeamId(int teamId)
        {
            using (var repo = new MatchManagementRepository())
            {
                _cachedMatches = await repo.GetMatchesByTeamId(teamId)?.ToListAsync() ?? new List<Match>();

                return _cachedMatches;
            }
        }

        public List<Match> FilterMatches(DateTime? startDate, DateTime? endDate, string status)
        {
            var filteredMatches = _cachedMatches.AsQueryable();
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            if (startDate.HasValue && endDate.HasValue)
            {
                filteredMatches = filteredMatches
                    .Where(m => m.MatchDate >= startDate.Value && m.MatchDate <= endDate.Value)
                    .OrderBy(m => m.MatchDate);
            }

            if (!string.IsNullOrEmpty(status) && status != "- Select a status -")
            {
                filteredMatches = filteredMatches
                    .Where(m => m.Status.Trim().ToLower() == status.Trim().ToLower())
                    .OrderByDescending(m => m.MatchDate.Year == currentYear && m.MatchDate.Month == currentMonth)
                    .ThenBy(m => m.MatchDate);
            }

            var result = filteredMatches.ToList();

            return result;
        }


        public List<Match> FilterMatchesForMonth(int year, int month)
        {
            return _cachedMatches
                .Where(m => m.MatchDate.Year == year && m.MatchDate.Month == month)
                .OrderBy(m => m.MatchDate)
                .ToList();
        }

        public async Task<List<Match>> GetMatchesByStatus(int? teamId, string status)
        {
            using (var repo = new MatchManagementRepository())
            {
                return await repo.GetMatchesByStatus(teamId, status)?.ToListAsync() ?? new List<Match>();
            }
        }

        public async Task<List<Match>> GetMatchesByDate(int? teamId, DateTime startDate, DateTime endDate)
        {
            using (var repo = new MatchManagementRepository())
            {
                return await repo.GetMatchesByDate(teamId, startDate, endDate)?.ToListAsync() ?? new List<Match>();
            }
        }

        public bool AddMatch(Match match)
        {
            using (var repo = new MatchManagementRepository())
            {
                int affectedRows = repo.AddNewMatch(match);
                if (affectedRows > 0)
                {
                    _cachedMatches.Add(match);
                    return true;
                }
                return false;
            }
        }

        public bool RemoveMatch(Match match)
        {
            using (var repo = new MatchManagementRepository())
            {
                int affectedRows = repo.DeleteMatch(match);
                if (affectedRows > 0)
                {
                    var matchToRemove = _cachedMatches.FirstOrDefault(m => m.MatchID == match.MatchID);
                    if (matchToRemove != null)
                    {
                        _cachedMatches.Remove(matchToRemove);
                    }
                    return true;
                }
                return false;
            }
        }

        public bool UpdateMatch(Match match)
        {
            using (var repo = new MatchManagementRepository())
            {
                int affectedRows = repo.Update(match);
                if (affectedRows > 0)
                {
                    _cachedMatches.Add(match);
                    return true;
                }
                return false;
            }
        }




        public bool DoesMatchExist(int teamId, DateTime matchDate, TimeSpan startTime)
        {
            using (var repo = new MatchManagementRepository())
            {
                return repo.GetAllMatches().Any(m =>
                    m.TeamID == teamId &&
                    m.MatchDate == matchDate &&
                    m.StartTime == startTime);
            }
        }
    }
}
