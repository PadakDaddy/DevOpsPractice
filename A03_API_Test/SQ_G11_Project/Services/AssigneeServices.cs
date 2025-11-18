/*
 * FILE          : AssigneeServices.cs
 * PROJECT       : SENG2020 - Group Project #01
 * PROGRAMMER    : Burhan Shibli, Daeseong Yu, Nick Turco, Sungmin Leem
 * FIRST VERSION : 2025-10-31
 */

using TaskManagement.Repositories;

namespace TaskManagement.Services
{
    //tells user whats happenign when trying to add assignee to a task
    /*
     * NAME     : 
     * PURPOSE  : 
     */
    public enum AddAssigneeResult
    {
        Ok,
        NotFound,
        BadInput,
        AlreadyAssigned
    }
    public enum RemoveAssigneeResult { Ok, NotFound, NoAssignee }

    public class AssigneeServices
    {
        private readonly TaskRepository _repo;
        public AssigneeServices(TaskRepository repo)
        {
            _repo = repo;
        }

        //-=-------------------ADD ASSIGNEE--------------------------

        // FUNCTION     :AddAssignee
        // DESCRIPTION  :adds assignee name to a task if it exists and does not have a name an assignee already assigned
        // PARAMETERS :id, name
        // RETURNS :AddAssigneeResult value, else error
        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public AddAssigneeResult AddAssignee(int id, string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return AddAssigneeResult.BadInput;

            var task = _repo.GetTaskById(id);
            if (task == null)
                return AddAssigneeResult.NotFound;

            if (!string.IsNullOrWhiteSpace(task.Assignee))
                return AddAssigneeResult.AlreadyAssigned;

            task.Assignee = name.Trim();
            _repo.UpdateTask(task);
            return AddAssigneeResult.Ok;
        }

        //------------------REMOVE ASSIGNEE------------------------------------

        // FUNCTION :RemoveAssignee
        // DESCRIPTION :removes assignee from a task if task has an assignee
        // PARAMETERS :id
        // RETURNS :RemoveAssigneeResult value , else error
        public RemoveAssigneeResult RemoveAssignee(int id)
        {
            var task = _repo.GetTaskById(id);
            if (task == null) return RemoveAssigneeResult.NotFound;
            if (string.IsNullOrWhiteSpace(task.Assignee)) return RemoveAssigneeResult.NoAssignee;

            task.Assignee = null;   
            _repo.UpdateTask(task);
            return RemoveAssigneeResult.Ok;
        }
    }
}