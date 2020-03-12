using System;
using System.Collections.Generic;
using System.Text;

namespace DomainInfoCore.DataObject
{
    public class TaskItem
    {
        public TaskType TaskType { get; set; }
        public TaskState TaskState { get; set; }
    }
}
