/*
 * FILE          : TaskController.cs
 * PROJECT       : SENG2020 - Group Project #01
 * PROGRAMMER    : Burhan Shibli, Daeseong Yu, Nick Turco, Sungmin Leem
 * FIRST VERSION : 2025-10-31
 * DESCRIPTION   : Functions for validating variables
 */

using Microsoft.AspNetCore.Mvc;
using TaskManagement.Models;
using TaskManagement.Services;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api")]
    /*
     * NAME     : 
     * PURPOSE  : 
     */
    public class TaskController : ControllerBase
    {
        private readonly AssigneeServices _assignee;
        private readonly PriorityServices _priorityServices;
        private readonly TaskServices _services;
        private readonly ILogger<TaskController> _logger;

        public TaskController(TaskServices services, AssigneeServices assignee, PriorityServices priority, ILogger<TaskController> logger)
        {
            _logger = logger;
            _services = services;
            _assignee = assignee;
            _priorityServices = priority;
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        [HttpPost(("tasks"), Name = "AddTask")]
        public ActionResult AddTask([FromBody] Tasks newTask)
        {
            try
            {
                _services.AddTask(newTask);
                return StatusCode(201, "Added!"); //201 created
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        [HttpPatch("tasks/{id}", Name = "UpdateTask")]
        public ActionResult UpdateTask(int id, [FromBody] UpdateTaskRequest request)
        {
            try
            {
                // Create a minimal task object with only Title
                var updatedTask = new Tasks
                {
                    Id = id,
                    Title = request.Title,
                    Assignee = null,  // Ignore assignee from this endpoint
                    Priority = 0      // Ignore priority from this endpoint
                };

                _services.UpdateTask(updatedTask);
                return Ok(new { message = "Task updated successfully!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Helper DTO class (add at the end of Program.cs or separate file)
        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        public class UpdateTaskRequest
        {
            public string Title { get; set; } = string.Empty;
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        [HttpDelete(("tasks/{id}"), Name = "DeleteTask")]
        public ActionResult DeleteTask(int id)
        {
            try
            {
                _services.DeleteTask(id);
                return NoContent(); //204 no content
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        // FUNCTION :AddAssignee
        // DESCRIPTION : handles put request to assign assignee to task
        // PARAMETERS : id, info
        // RETURNS :IActionResult if add works, else bad request or not found
        [HttpPut("tasks/{id:int}/assignee")]
        public IActionResult AddAssignee(int id, [FromBody] AssigneeInfo info)
        {
            var result = _assignee.AddAssignee(id, info?.Name);
            return result switch
            {
                AddAssigneeResult.Ok => Ok(),
                AddAssigneeResult.NotFound => NotFound("Task not found."),
                AddAssigneeResult.BadInput => BadRequest("Assignee name is required."),
                AddAssigneeResult.AlreadyAssigned => BadRequest("This task already has an assignee."),
                _ => BadRequest("Bad request.")
            };
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        // FUNCTION :RemoveAssignee
        // DESCRIPTION : handles delete request for removing assignee from a task
        // PARAMETERS : id
        // RETURNS : IActionResult if removed, else not found or bad request
        [HttpDelete("tasks/{id:int}/assignee")]
        public IActionResult RemoveAssignee(int id)
        {
            var result = _assignee.RemoveAssignee(id);
            return result switch
            {
                RemoveAssigneeResult.Ok => NoContent(),
                RemoveAssigneeResult.NotFound => NotFound("Task not found."),
                RemoveAssigneeResult.NoAssignee => BadRequest("This task has no assignee to remove."),
                _ => BadRequest("Bad request.")
            };
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        //METHOD NAME: AddPriorityToTask
        //DESCRIPTION: Adds a value to the priority attrubute of the given task
        //             Uses a try catch to check watch for exceptions
        //             If a match is found, then the new value replaces the old value.
        //PARAMETERS: int id - the id value of the task being modified
        //            [FromBody] int priority - the value being entered into the priority attribute, comes in a JSON format from client side.
        //RETURNS: returns Ok(code:200) if successful.
        //                 BadRequest(code:400) if the server could not understand the request.
        //                 StatusCode 500: if there is an internal server error.
        [HttpPut("tasks/{id}/priority")]
        public ActionResult AddPriorityToTask(int id, [FromBody] int priority)
        {
            try
            {
                //check if the method returns -1 for error or 0 for successful.
                bool success = _priorityServices.AddPriority(id, priority);

                if (!success)
                {
                    return BadRequest(new { message = "Invalid task ID or priority range." });
                }
                    
                return Ok(new { message = "Priority added successfully!"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding priority to task.");
                return StatusCode(500, new { message = "Internal server error.", details = ex.Message });
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        //METHOD NAME: ChangeTaskPriority
        //DESCRIPTION: Changes the value of the priority attrubute of the given task
        //             Uses a try catch to check watch for exceptions
        //             If a match is found, then the new value replaces the old value.
        //PARAMETERS: int id - the id value of the task being modified
        //            [FromBody] int priority - the value being entered into the priority attribute, comes in a JSON format from client side.
        //RETURNS: returns Ok(code:200) if successful.
        //                 BadRequest(code:400) if the server could not understand the request.
        //                 StatusCode 500: if there is an internal server error.
        [HttpPatch("tasks/{id}/priority")]
        public ActionResult ChangeTaskPriority(int id, [FromBody] int priority)
        {
            try
            {
                //check if the method returns -1 for error or 0 for successful.
                bool success = _priorityServices.ChangePriority(id, priority);

                if (!success)
                {
                    return BadRequest(new { message = "Invalid task ID or priority range." });
                }

                return Ok(new { message = "Priority updated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while modifying priority to task.");
                return StatusCode(500, new { message = "Internal server error.", details = ex.Message });
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        //METHOD NAME: RemovePriorityFromTask
        //DESCRIPTION: Removes the value of the priority attrubute of the given task
        //             Uses a try catch to check watch for exceptions
        //             If a match is found, then the new value replaces the old value.
        //PARAMETERS: int id - the id value of the task being modified
        //RETURNS: returns Ok(code:200) if successful.
        //                 BadRequest(code:400) if the server could not understand the request.
        //                 StatusCode 500: if there is an internal server error.
        [HttpDelete("tasks/{id}/priority")]
        public ActionResult RemovePriorityFromTask(int id) 
        {
            try
            {
                bool success = _priorityServices.DeletePriority(id);

                if(!success)
                {
                    return BadRequest(new { message = "Invalid task ID."});
                }
                return Ok(new { message = "Priority removed successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error while removing priority from task.");
                return StatusCode(500, new { message = "Internal server error.", details = ex.Message });
            }
        }

        //
        // FUNCTION     : 
        // DESCRIPTION  : 
        // PARAMETERS   : 
        // RETURNS      : 
        //
        [HttpGet("tasks",Name = "SearchTask")]
        public ActionResult SearchTask([FromQuery] string? id, [FromQuery] string? assignee) 
        {
            int taskId = 0;
            try
            {
                List<Tasks> result = null;

                if (!String.IsNullOrWhiteSpace(id))
                {
                    if (int.TryParse(id, out taskId))
                    {
                        result = _services.SearchTask(taskId);
                    }
                    else
                    {
                        return BadRequest("Task Id must be numeric digits");
                    }
                } 
                else if (!String.IsNullOrWhiteSpace(assignee))
                {
                    result = _services.SearchTask(assignee);
                }
                else
                {
                    result = _services.DisplayAllTasks();
                }

                if (result == null || result.Count == 0)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
