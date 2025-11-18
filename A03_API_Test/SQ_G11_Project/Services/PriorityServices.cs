/*
 * FILE          : PriorityServices.cs
 * PROJECT       : SENG2020 - Group Project #01
 * PROGRAMMER    : Burhan Shibli, Daeseong Yu, Nick Turco, Sungmin Leem
 * FIRST VERSION : 2025-10-31
 */

using TaskManagement.Repositories;

namespace TaskManagement.Services
{
    /*
     * NAME     : 
     * PURPOSE  : 
     */
    public class PriorityServices
    {
        private readonly TaskRepository _repository;
        public PriorityServices(TaskRepository repository)
        {
            _repository = repository;
        }

        //
        // FUNCTION     : AddPriority
        // DESCRIPTION  : Validates the priority range.
        //                Calls the Updatepriority method if valid.
        // PARAMETERS   : int id - the id value of the task being modified
        //                int priority - the value being entered into the priority attribute
        // RETURNS      : returns a bool, true if update was successful or false if it fails.
        //
        public bool AddPriority(int id, int priority)
        {

            // Validate range
            if (priority < 0 || priority > 4)
            {
                return false;
            }
            return _repository.UpdatePriority(id, priority);
        }
        //METHOD NAME: ChangePriority
        //DESCRIPTION: validates the priority range.
        //             Calls the Updatepriority method if valid.
        //PARAMETERS: int id - the id value of the task being modified
        //            int priority - the value being entered into the priority attribute
        //RETURNS: returns a bool, true if update was successful or false if it fails.
        public bool ChangePriority(int id, int priority)
        {

            // Validate range
            if (priority < 0 || priority > 4)
            {
                return false;
            }
            return _repository.UpdatePriority(id, priority);
        }
        //METHOD NAME: DeletePriority
        //DESCRIPTION: Checks if the entered value us less than or equal to zero.
        //             Calls the Updatepriority method if valid.
        //PARAMETERS: int id - the id value of the task being modified
        //RETURNS: returns a bool, true if update was successful or false if it fails.
        public bool DeletePriority(int id)
        {
            if (id <= 0)
            {
                return false;
            }
            _repository.RemovePriority(id);
            return true;
        }
    }      
}
