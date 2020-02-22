using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BiesBlogBlazor.Server.Hubs
{
    public class BlogHub : Hub<IBlogFeed>
    {
    }
}
