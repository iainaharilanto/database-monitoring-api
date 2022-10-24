namespace Monitor.WebApi.Entities
{
    using System;

    public class Configuration
    {
        public Guid ProjectId { get; set; }

        public double AllocatedMemory { get; set; }

        public double SlowQueryThreshold { get; set; }

        public double Frequency { get; set; }

        public double BulkInsertSize { get; set; }
    }
}
