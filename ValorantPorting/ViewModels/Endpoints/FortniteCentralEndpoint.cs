using System.Threading.Tasks;
using ValorantPorting.Framework.ViewModels.Endpoints;
using ValorantPorting.ViewModels.Endpoints.Models;
using RestSharp;

namespace ValorantPorting.ViewModels.Endpoints;

public class ValorantCentralEndpoint : EndpointBase
{
    private const string AES_URL = "https://valorantcentral.genxgames.gg/api/v1/aes";
    private const string MAPPINGS_URL = "https://valorantcentral.genxgames.gg/api/v1/mappings";

    public ValorantCentralEndpoint(RestClient client) : base(client)
    {
    }

    public async Task<AesResponse?> GetKeysAsync()
    {
        return await ExecuteAsync<AesResponse>(AES_URL);
    }

    public AesResponse? GetKeys()
    {
        return GetKeysAsync().GetAwaiter().GetResult();
    }

    public async Task<MappingsResponse[]?> GetMappingsAsync()
    {
        return await ExecuteAsync<MappingsResponse[]>(MAPPINGS_URL);
    }

    public MappingsResponse[]? GetMappings()
    {
        return GetMappingsAsync().GetAwaiter().GetResult();
    }
}