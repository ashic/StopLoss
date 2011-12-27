namespace Tests
{
    public class Stock
    {
        StockState _state;
        decimal _currentSellPrice;

        public Stock(string id, decimal price)
        {
            _currentSellPrice = price - 1;
            _state = new StableState(id, 0);

            Bus.RegisterHandler<SellPriceIncreasedEvent>(x=>_currentSellPrice = x.SellingPoint);
        }

        public void HandlePriceChange(decimal newPrice)
        {
            _state = _state.HandlePriceChanged(_currentSellPrice, newPrice);
        }

        public void HandleTimeout(IncreaseSellingPointCommand command)
        {
            _state = _state.HandleTimeout(command);
        }

        public void HandleTimeout(SellCommand command)
        {
            _state = _state.HandleTimeout(command);
        }
    }
}