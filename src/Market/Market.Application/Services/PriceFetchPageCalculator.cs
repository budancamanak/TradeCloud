using Ardalis.GuardClauses;
using Common.Core.Enums;
using Common.Core.Extensions;
using Market.Application.Exceptions;
using Market.Application.Models;

namespace Market.Application.Services;

public class PriceFetchPageCalculator
{
    /// <summary>
    /// Will calculate price fetch pages to be used to fetch data from external sources
    /// </summary> 
    /// <param name="timeframe">Timeframe of price data</param>
    /// <param name="start">Starting date of plugin</param>
    /// <param name="end">End date of plugin</param>
    /// <param name="pageLimit">Number of prices to be fetched each </param>
    /// <returns></returns>
    /// <exception cref="NoPageCalculatedException">Throws when there are no pages calculated for given parameters</exception>
    public static List<PriceFetchPages> ToPages(Timeframe timeframe, DateTime start,
        DateTime end, int pageLimit)
    {
        // todo remove price info from here
        // todo calculate pages by end-start only
        // todo recurring fetches might happen. they won't be saved to database anyway
        // todo priceService must not cache duplicate prices upon completion
        Guard.Against
            .NullDate(start)
            .NullDate(end);
        // Dictionary<long, long> map = new Dictionary<long, long>();


        var pages = new List<PriceFetchPages>();
        // var priceInfoLeftBound = priceInfo.First().Timestamp;
        // var priceInfoRightBound = priceInfo.Last().Timestamp;

        var startMillisecond = start.TotalMilliseconds() - timeframe.GetMilliseconds();
        var timeBuffer = timeframe.GetMilliseconds();
        var timespan = ((end - start).TotalMilliseconds + timeBuffer) / timeframe.GetMilliseconds();
        var pageCount = (int)Math.Ceiling(timespan / pageLimit);

        for (var i = 0; i < pageCount; i++)
        {
            pages.Add(new PriceFetchPages(startMillisecond, pageLimit));
            startMillisecond += (pageLimit * timeframe.GetMilliseconds());
        }

        // timespan = (end - priceInfoRightBound).TotalMilliseconds / timeframe.GetMilliseconds();
        // pageCount = (int)Math.Ceiling(timespan / pageLimit);
        // startMillisecond = priceInfo.Last().Timestamp.TotalMilliseconds();
        // for (var i = 0; i < pageCount; i++)
        // {
        //     pages.Add(new PriceFetchPages(startMillisecond, timeframe, pageLimit));
        //     startMillisecond += (pageLimit * timeframe.GetMilliseconds());
        // }

        if (pages.Count == 0)
            throw new NoPageCalculatedException();
        return pages;
    }
}