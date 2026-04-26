using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IMusicSearchEngineService : IService<MusicSearchItemResponse, MusicSearchSearchObject>
    {
    }
}