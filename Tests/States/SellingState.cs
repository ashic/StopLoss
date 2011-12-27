using System;

namespace Tests
{
    public class SellingState : StockState
    {
        readonly string _id;
        readonly int _version;

        public SellingState(string id, int version)
        {
            _id = id;
            _version = version;

            Bus.Send(new RequestTimeout<SellCommand>(_id, new SellCommand(_id, _version), TimeSpan.FromSeconds(30)));
        }

        public StockState HandleTimeout(SellCommand command)
        {
            if(_version == command.Version)
                return new SoldState(command.Id);

            return this;
        }

        public StockState HandleTimeout(IncreaseSellingPointCommand command)
        {
            return this;
        }

        public StockState HandlePriceChanged(decimal currentSellPrice, decimal newPrice)
        {
            if(newPrice > currentSellPrice)
            {
                return new StableState(_id, _version).HandlePriceChanged(currentSellPrice, newPrice);
            }

            return this;
        }
    }
}