using BiesBlogBlazor.Shared.Entities;
using System.Threading.Tasks;

namespace BiesBlogBlazor.Server.Hubs
{
    public interface IBlogFeed
    {
        Task BlogCreated(Blog blog);
    }
}