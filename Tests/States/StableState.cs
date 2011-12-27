namespace Tests
{
    public class StableState : StockState
    {
        readonly string _id;
        readonly int _version;

        public StableState(string id, int version)
        {
            _id = id;
            _version = version;
        }

        public StockState HandleTimeout(SellCommand command)
        {
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
                return new IncreasingState(_id, newPrice, _version + 1);
            }

            if(newPrice <= currentSellPrice)
            {
                return new SellingState(_id, _version + 1);
            }

            return new StableState(_id, _version);
        }
    }
}