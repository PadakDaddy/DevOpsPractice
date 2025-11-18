/*
 * FILE          : TaskRepository.cs
 * PROJECT       : SENG2020 - Group Project #01
 * PROGRAMMER    : Burhan Shibli, Daeseong Yu, Nick Turco, Sungmin Leem
 * FIRST VERSION : 2025-10-31
 */

using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class TaskRepository
    {
        /*
         * NAME     : 
         * PURPOSE  : 
         */
        private readonly List<Tasks> _tasks = new List<Tasks> { };
        private readonly object _locker = new object();

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public void AddTask(Tasks data)
        {
            lock (_locker)
            {
                Tasks existingTask = _tasks.FirstOrDefault(t => t.Id == data.Id);

                if (existingTask != null)
                {
                    throw new ArgumentException("Task ID is already existed.");
                }
                _tasks.Add(data); //save task
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public void UpdateTask(Tasks data)
        {
            lock (_locker)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == data.Id);
                if (task == null)
                {
                    throw new InvalidOperationException($"Task with ID {data.Id} not found.");
                }

                // Update only the Title
                task.Title = data.Title;

                // These should be updated through dedicated endpoints in AssigneeServices and PriorityServices
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public void DeleteTask(int taskId)
        {
            lock (_locker)
            {
                var task = _tasks.FirstOrDefault(task => task.Id == taskId);
                if (task == null)
                    throw new InvalidOperationException($"Task with ID {taskId} not found.");

                _tasks.Remove(task);
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public Tasks? GetTaskById(int id)
        {
            lock (_locker)
            {
                return _tasks.FirstOrDefault(t => t.Id == id);
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        //METHOD NAME: UpdatePriority
        //DESCRIPTION: Searches for the task to be modified
        //             Checks if there was no task found that matches the id.
        //             If a match is found, then the new value replaces the old value.
        //PARAMETERS: int id - the id value of the task being modified
        //            int priority - the value being entered into the priority attribute
        //RETURNS: returns a bool, true if update was successful or false if it fails.
        public bool UpdatePriority(int id, int priority)
        {
            lock (_locker)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task == null)
                {
                    return false;
                }

                task.Priority = priority;
                return true; 
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        //METHOD NAME: RemovePriority
        //DESCRIPTION: Searches for the task to be modified
        //             Checks if there was no task found that matches the id.
        //             If it finds a match, it sets the priority attribute to zero.
        //PARAMETERS: int id - the id value of the task being modified
        //RETURNS: returns a bool, true if update was successful or false if it fails.
        public bool RemovePriority (int id)
        {
            lock ( _locker)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if(task == null)
                {
                    return false;
                }
                task.Priority = 0;
                return true;
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public List<Tasks> SearchTaskById(int query)
        {
            List<Tasks> result = _tasks.FindAll(x => x.Id == query);

            return result;
            
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public List<Tasks> SearchTaskByAssignee(string? query)
        {
            List<Tasks> result = _tasks.FindAll(x =>
                !String.IsNullOrWhiteSpace(x.Assignee) &&
                x.Assignee.Equals(query)
            );

            return result;
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public List<Tasks> DisplayAllTasks()
        {
            return _tasks;
        }
    }
}
