namespace Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase;

public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    /// <summary>
    /// 事件订阅信息对象
    /// </summary>
    public class SubscriptionInfo
    {
        /// <summary>
        /// 是否为简单动态订阅
        /// </summary>
        public bool IsDynamic { get; }

        /// <summary>
        /// 应被哪种TypeHandler处理
        /// </summary>
        public Type HandlerType { get; }

        private SubscriptionInfo(bool isDynamic, Type handlerType)
        {
            IsDynamic = isDynamic;
            HandlerType = handlerType;
        }

        public static SubscriptionInfo Dynamic(Type handlerType) => new(true, handlerType);

        public static SubscriptionInfo Typed(Type handlerType) => new(false, handlerType);
    }
}
