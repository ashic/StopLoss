using System;

namespace Tests
{
    public class RequestTimeout<T>
    {
        public readonly string Id;
        public readonly TimeSpan Delay;
        public readonly T Body;

        public RequestTimeout(string id, T body, TimeSpan delay)
        {
            Id = id;
            Body = body;
            Delay = delay;
        }
    }

    public class IncreaseSellingPointCommand
    {
        public readonly decimal SellingPoint;
        public readonly int Version;

        public IncreaseSellingPointCommand(decimal sellingPoint, int version)
        {
            SellingPoint = sellingPoint;
            Version = version;
        }
    }

    public class SellPriceIncreasedEvent
    {
        public readonly string Id;
        public readonly decimal SellingPoint;

        public SellPriceIncreasedEvent(string id, decimal sellingPoint)
        {
            Id = id;
            SellingPoint = sellingPoint;
        }
    }


    public class SellCommand
    {
        public readonly string Id;
        public readonly int Version;

        public SellCommand(string id, int version)
        {
            Id = id;
            Version = version;
        }
    }

    public class SoldEvent
    {
        public readonly string Id;

        public SoldEvent(string id)
        {
            Id = id;
        }
    }
}