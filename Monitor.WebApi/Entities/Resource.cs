namespace Monitor.WebApi.Entities
{
    using System;

    public class Resource
    {
        public Guid Id { get; set; }

        public Guid ProjectId { get; set; }

        public DateTime Date { get; set; }

        public ResourceType Type { get; set; }

        public double Value { get; set; }
    }

    public enum ResourceType
    {
        Ram,
        Cpu
    }
}
