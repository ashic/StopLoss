using System;

namespace Tests
{
    public class IncreasingState : StockState
    {
        readonly string _id;
        readonly decimal _trailingPrice;
        readonly int _version;

        public IncreasingState(string id, decimal trailingPrice, int version)
        {
            _id = id;
            _trailingPrice = trailingPrice;
            _version = version;

            Bus.Send(new RequestTimeout<IncreaseSellingPointCommand>(id, new IncreaseSellingPointCommand(trailingPrice - 1, version), TimeSpan.FromSeconds(15)));
        }

        public StockState HandleTimeout(SellCommand command)
        {
            return this;
        }

        public StockState HandleTimeout(IncreaseSellingPointCommand command)
        {
            if (command.Version == _version)
                Bus.Send(new SellPriceIncreasedEvent(_id, command.SellingPoint));
            
            return this;
        }

        public StockState HandlePriceChanged(decimal currentSellPrice, decimal newPrice)
        {
            if (newPrice > _trailingPrice)
            {
                Bus.Send(new RequestTimeout<IncreaseSellingPointCommand>(_id, new IncreaseSellingPointCommand(newPrice - 1, _version), TimeSpan.FromSeconds(30)));
                return this;
            }

            return new StableState(_id, _version).HandlePriceChanged(currentSellPrice, newPrice);
        }
    }
}