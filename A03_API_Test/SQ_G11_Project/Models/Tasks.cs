/*
 * FILE          : Tasks.cs
 * PROJECT       : SENG2020 - Group Project #01
 * PROGRAMMER    : Burhan Shibli, Daeseong Yu, Nick Turco, Sungmin Leem
 * FIRST VERSION : 2025-10-31
 * DESCRIPTION   : Functions for validating variables
 */

namespace TaskManagement.Models
{
    /*
     * NAME     : 
     * PURPOSE  : 
     */
    public class Tasks
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Assignee { get; set; }
        public int Priority { get; set; }

        public Tasks() { }

        public Tasks(int id, string title, string assignee, int priority)
        {
            this.Id = id;
            this.Title = title;
            this.Assignee = assignee;
            this.Priority = priority;
        }

    }

    public class AssigneeInfo
    {
        public string? Name { get; set; }
    }
}
