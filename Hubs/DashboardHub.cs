using Microsoft.AspNetCore.SignalR;

namespace SSISAnalyticsDashboard.Hubs
{
    public class DashboardHub : Hub
    {
        public async Task SendMetricsUpdate(object metrics)
        {
            await Clients.All.SendAsync("ReceiveMetricsUpdate", metrics);
        }

        public async Task SendTrendsUpdate(object trends)
        {
            await Clients.All.SendAsync("ReceiveTrendsUpdate", trends);
        }

        public async Task NotifyDataRefresh()
        {
            await Clients.All.SendAsync("DataRefreshed");
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
