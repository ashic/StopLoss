namespace Tests
{
    public class SoldState : StockState
    {
        public SoldState(string id)
        {
            Bus.Send(new SoldEvent(id));
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
            return this;
        }
    }
}