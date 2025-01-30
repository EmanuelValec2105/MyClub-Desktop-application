﻿using DataAccessLayer.EntityRepositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class TrainingService
    {
        public List<Training> GetTrainings()
        {
            using (var repo = new TrainingRepository())
            {
                List<Training> trainings = repo.GetAll().ToList();
                return trainings;
            }
        }

        public bool AddTraining(Training training)
        {
            bool isSuccessful = false;

            using (var repo = new TrainingRepository())
            {
                int affectedRows = repo.Add(training);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool UpdateTraining(Training training)
        {
            bool isSuccessful = false;

            using (var repo = new TrainingRepository())
            {
                int affectedRows = repo.Update(training);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool RemoveTraining(Training training)
        {
            bool isSuccessful = false;

            using (var repo = new TrainingRepository())
            {
                int affectedRows = repo.Remove(training);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool 
    }
}
