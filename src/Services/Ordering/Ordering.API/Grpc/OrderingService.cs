namespace GrpcOrdering;

public class OrderingService : OrderingGrpc.OrderingGrpcBase
{
    public OrderingService()
    {

    }

    public override Task<ShopItemResponse> SetShopItemStock(CreateOrderRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}
