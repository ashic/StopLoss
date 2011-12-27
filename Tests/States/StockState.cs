namespace Tests
{
    public interface StockState
    {
        StockState HandleTimeout(SellCommand command);
        StockState HandleTimeout(IncreaseSellingPointCommand command);
        StockState HandlePriceChanged(decimal currentSellPrice, decimal newPrice);
    }
}