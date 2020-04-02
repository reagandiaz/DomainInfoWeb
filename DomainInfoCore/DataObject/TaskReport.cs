using System;

namespace DomainInfoCore.DataObject
{
    public class TaskReport
    {
        TaskType type;
        DateTime start;
        DateTime end;
        string data;
        TaskState state;

        public TaskType TaskType => type;
        public DateTime Start => start;
        public DateTime End => end;
        public string Data => data;
        public TaskState State => state;

        public TaskReport(TaskResultItem tr)
        {
            type = tr.TaskType;
            start = tr.QTS;
            end = tr.TS;
            data = tr.Data;
            state = tr.State;
        }

        public override string ToString()
        {
            return $"TYPE:{Enum.GetName(type.GetType(), type)} ST:{Enum.GetName(state.GetType(), state)} DUR:{end.Subtract(start).TotalSeconds}";
        }
    }
}
