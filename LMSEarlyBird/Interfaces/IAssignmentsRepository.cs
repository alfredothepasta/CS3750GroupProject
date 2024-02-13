﻿using LMSEarlyBird.Models;

namespace LMSEarlyBird.Interfaces
{
    public interface IAssignmentsRepository
    {
        Task<List<Assignment>> GetCourseAssignments(int courseId);
        Task<List<Assignment>> GetStudentAssignments(string studentId);
        Task<bool> AddStudentAssignments(string userId, int courseId);

		Task<bool> AddAssignment(Assignment assignment, int courseId);
        bool RemoveAssignment(Assignment assignment);
        bool UpdateAssignment(Assignment assignment);
        bool Save();

    }
}