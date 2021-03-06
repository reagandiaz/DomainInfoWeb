﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DomainInfoCore.DataObject
{
    public class TaskQueueItem
    {
        public Int64 ID => id;

        public string IP => ip;

        public TaskType TaskType => type;

        public DateTime QueueTS => ts;

        public int ReportCount => rpcnt;

        readonly Int64 id;

        readonly string ip;

        readonly TaskType type;

        readonly DateTime ts;

        readonly int rpcnt;

        public TaskQueueItem(IPRequest request, TaskType type)
        {
            id = request.ID;
            ip = request.IP;
            rpcnt = request.ReportCount;
            ts = request.TS;
            this.type = type;
        }

        public override string ToString()
        {
            return $"{Enum.GetName(TaskType.GetType(), type)}: ID:{id} IP:{ip} TS:{ts.ToShortTimeString()}";
        }
    }
}
