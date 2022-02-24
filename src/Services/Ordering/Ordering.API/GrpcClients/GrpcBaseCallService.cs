using Grpc.Net.Client;

namespace Zhouxieyi.evaluationSiteOnContainers.Services.Ordering.API.GrpcClients;

public class GrpcBaseCallService
{
    public async Task<bool> StopShopSellAsync(int shopItemId)
    {
        var channel = GrpcChannel.ForAddress("http://127.0.0.1:55001");
        var client = new GameRepository.GameRepositoryClient(channel);
        var request = new shopStatusChangeRequest
        {
            ShopItemId = shopItemId
        };
        var response = await client.ChangeShopSellStatusAsync(request);
        return response != null && response.StopSell;
    }
}