/*
 * FILE          : TaskServices.cs
 * PROJECT       : SENG2020 - Group Project #01
 * PROGRAMMER    : Burhan Shibli, Daeseong Yu, Nick Turco, Sungmin Leem
 * FIRST VERSION : 2025-10-31
 */

using TaskManagement.Models;
using TaskManagement.Repositories;

namespace TaskManagement.Services
{
/*
 * NAME     : TaskServices
 * PURPOSE  : 
 */
    public class TaskServices
    {
        private readonly TaskRepository _repository;
        public TaskServices(TaskRepository repository)
        {
            _repository = repository;
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public void AddTask(Tasks data)
        {
            if (string.IsNullOrWhiteSpace(data.Title))
            {
                throw new ArgumentException("Title is required.");
            }

            if (string.IsNullOrWhiteSpace(data.Assignee))
            {
                data.Assignee = "";
            }

            // Priority defaults to 0 if not set
            if (data.Priority < 0 || data.Priority > 4)
            {
                data.Priority = 0;
            }
            _repository.AddTask(data);
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public List<Tasks> DisplayAllTasks()
        {
            return _repository.DisplayAllTasks();
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public void UpdateTask(Tasks data)
        {
            //validate : Title required
            if (string.IsNullOrWhiteSpace(data.Title))
            {
                throw new ArgumentException("Title is required.");
            }
            _repository.UpdateTask(data);
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public void DeleteTask(int taskId)
        {
            _repository.DeleteTask(taskId);
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public List<Tasks> SearchTask(object filter)
        {
            List<Tasks> result = new List<Tasks>();

            if (filter is int filterTaskId)
            {
                result = _repository.SearchTaskById(filterTaskId);
            }
            else if (filter is string filterAssigneeName && !string.IsNullOrEmpty(filterAssigneeName))
            {
                result = _repository.SearchTaskByAssignee(filterAssigneeName);
            }

            return result;
            
        }
    }
}
