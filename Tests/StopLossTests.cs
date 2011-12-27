using System;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Tests
{
    public abstract class with_bus
    {
        Establish context = () => Bus.Clear();
    }

    public class when_price_increases_beyond_trailing_point : with_bus
    {
        static Stock stock;
        Establish context = () => stock = new Stock("MSFT", 10M);

        Because of = () => stock.HandlePriceChange(11.0M);

        It should_request_increasing_timeout = () =>
        {
            var message = Bus.Messages.Single() as RequestTimeout<IncreaseSellingPointCommand>;
            message.Id.ShouldEqual("MSFT");
            message.Delay.ShouldEqual(TimeSpan.FromSeconds(15));
            message.Body.SellingPoint.ShouldEqual(10M);
        };
    }

    public class when_price_decreases_beyond_selling_point : with_bus
    {
        static Stock stock;
        Establish context = () => stock = new Stock("MSFT", 10M);

        Because of = () => stock.HandlePriceChange(9.0M);

        It should_request_selling_timeout = () =>
        {
            var message = Bus.Messages.Single() as RequestTimeout<SellCommand>;
            message.Id.ShouldEqual("MSFT");
            message.Delay.ShouldEqual(TimeSpan.FromSeconds(30));
        };
    }

    public class when_price_goes_up_for_15_seconds : with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(12);
            Bus.Clear();
        };

        Because of = () =>
            stock.HandleTimeout(new IncreaseSellingPointCommand(11, 1));

        It should_increase_selling_point = () =>
        {
            var message = Bus.Messages.Single() as SellPriceIncreasedEvent;
            message.Id.ShouldEqual("MSFT");
            message.SellingPoint.ShouldEqual(11);
        };
    }

    public class when_price_goes_up_then_down_before_15_seconds_is_over : with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(12);
            stock.HandlePriceChange(10);
            Bus.Clear();
        };

        Because of = () => stock.HandleTimeout(new IncreaseSellingPointCommand(11, 1));

        It should_not_increase_selling_point = () =>
            Bus.Messages.ShouldBeEmpty();
    }

    public class when_price_goes_below_and_timeout_occurs : with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(9);
            Bus.Clear();
        };

        Because of = () => 
            stock.HandleTimeout(new SellCommand("MSFT", 1));

        It should_sell = () =>
        {
            var message = Bus.Messages.Single() as SoldEvent;
            message.Id.ShouldEqual("MSFT");
        };
    }

    public class when_price_goes_below_but_increases_and_timeout_occurs : with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(9);
            stock.HandlePriceChange(10);
            Bus.Clear();
        };

        Because of = () => stock.HandleTimeout(new SellCommand("MSFT", 1));

        It should_not_sell = () =>
            Bus.Messages.ShouldBeEmpty();
    }


    public class when_price_goes_above_trailpoint_drops_but_stays_above_trailpoint_and_timeout_for_first_occurs:with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(14);
            stock.HandlePriceChange(12);
            Bus.Clear();
        };

        Because of = () => stock.HandleTimeout(new IncreaseSellingPointCommand(13, 1));

        It should_not_increase_sell_point = () =>
            Bus.Messages.ShouldBeEmpty();
    }
    
    public class when_price_goes_above_trailpoint_drops_but_stays_above_trailpoint_and_timeout_for_second_occurs:with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(14);
            stock.HandlePriceChange(12);
            Bus.Clear();
            stock.HandleTimeout(new IncreaseSellingPointCommand(13, 1));
        };

        Because of = () => 
            stock.HandleTimeout(new IncreaseSellingPointCommand(11, 2));

        It should_increase_sell_point = () =>
        {
            var message = Bus.Messages.Single() as SellPriceIncreasedEvent;
            message.Id.ShouldEqual("MSFT");
            message.SellingPoint.ShouldEqual(11);
        };
    }

    public class when_price_falls_below_rises_above_falls_below_and_timeout_for_first_occurs : with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(8);
            stock.HandlePriceChange(12);
            stock.HandlePriceChange(6);
            Bus.Clear();
        };

        Because of = () =>
            stock.HandleTimeout(new SellCommand("MSFT", 1));

        It should_not_sell = () =>
            Bus.Messages.ShouldBeEmpty();
    }

    public class when_price_falls_below_rises_above_falls_below_and_timeout_for_second_occurs : with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(8);
            stock.HandlePriceChange(12);
            stock.HandlePriceChange(6);
            Bus.Clear();
        };

        Because of = () =>
            stock.HandleTimeout(new SellCommand("MSFT", 3));

        It should_sell = () =>
        {
            var message = Bus.Messages.Single() as SoldEvent;
            message.ShouldNotBeNull();
        };
    }

    public class when_sell_price_increases_and_price_falls_till_timeout : with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(11);
            stock.HandleTimeout(new IncreaseSellingPointCommand(10, 1));
            stock.HandlePriceChange(10);
            Bus.Clear();
        };

        Because of = () =>
            stock.HandleTimeout(new SellCommand("MSFT", 2));

        It should_sell = () =>
        {
            var message = Bus.Messages.Single() as SoldEvent;
            message.ShouldNotBeNull();
        };
    }

    public class when_sell_price_increases_and_price_falls_but_rises_before_timeout : with_bus
    {
        static Stock stock;
        Establish context = () =>
        {
            stock = new Stock("MSFT", 10M);
            stock.HandlePriceChange(11);
            stock.HandleTimeout(new IncreaseSellingPointCommand(10, 1));
            stock.HandlePriceChange(10);
            stock.HandlePriceChange(10.5M);
            Bus.Clear();
        };

        Because of = () =>
            stock.HandleTimeout(new SellCommand("MSFT", 2));

        It should_not_sell = () =>
            Bus.Messages.ShouldBeEmpty();
    }
}


