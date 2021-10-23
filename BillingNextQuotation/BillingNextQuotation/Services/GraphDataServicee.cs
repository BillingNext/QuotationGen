using BillingNextQuotation.DataModels;
using BillingNextQuotation.Hubs;
using BillingNextQuotation.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Services
{
    public class GraphDataService : IGraphDataService
    {
        private readonly BillingNextQuotation.Data.ApplicationDbContext _context;

        private readonly IHubContext<GraphDataHub> _hubContext;
        public GraphDataService(BillingNextQuotation.Data.ApplicationDbContext context, IHubContext<GraphDataHub> hubContext)
        {
            _hubContext = hubContext;
            _context = context;
        }

        async Task IGraphDataService.UpdateQuoteGenGraph()
        {
            var QuotGenAmtData = await _context.Quotations.GroupBy(x => x.ActualQuotationCreationDate).Select(g => new TwoDGraphData
            {
                Label = g.Key.ToString(),
                Data = g.Sum(x => x.QuotationGrandTotalAmount).ToString()
            }).ToListAsync();
            //var OutFlowData = await _context.Qu.GroupBy(x => x.BillDate.Date).Select(g => new TwoDGraphData
            //{
            //    Label = g.Key.ToString(),
            //    Data = g.Sum(x => x.BillAmount).ToString()
            //}).ToListAsync();
            var graphDataList = new List<GraphDataModel>();
            graphDataList.Add(new GraphDataModel { Count = QuotGenAmtData.Count, Type = "Quotations Generated", GraphContent = QuotGenAmtData });
            await _hubContext.Clients.All.SendAsync("sendCashFlowsData", JsonConvert.SerializeObject(graphDataList));
        }
    }
}
