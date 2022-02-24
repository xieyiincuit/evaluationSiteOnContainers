namespace GrpcOrdering;

public class OrderingService : OrderingGrpc.OrderingGrpcBase
{
    public override Task<ShopItemResponse> SetShopItemStock(CreateOrderRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}