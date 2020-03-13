using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DomainInfoCore.DataObject
{
    public class TaskReport
    {
        TaskType type;
        DateTime start;
        DateTime end;
        string message;
        TaskState state;

        public TaskType TaskType => type;
        public DateTime Start => start;
        public DateTime End => end;
        public string Message => message;
        public TaskState State => state;

        public TaskReport(TaskResultItem tr)
        {
            type = tr.TaskType;
            start = tr.QTS;
            end = tr.TS;
            message = tr.Message;
            state = tr.State;
        }

        public override string ToString()
        {
            return String.Format("TYPE:{0} ST:{1} MSG:{2} DUR:{1} ",
                Enum.GetName(type.GetType(), type), Enum.GetName(state.GetType(), state), message, end.Subtract(start).TotalSeconds);
        }
    }
}
