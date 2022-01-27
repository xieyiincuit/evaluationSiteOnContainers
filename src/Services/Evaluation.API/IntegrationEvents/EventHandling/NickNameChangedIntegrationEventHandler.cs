namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.IntegrationEvents.EventHandling;

public class NickNameChangedIntegrationEventHandler :
    IIntegrationEventHandler<NickNameChangedIntegrationEvent>
{
    private readonly ILogger<NickNameChangedIntegrationEventHandler> _logger;
    private readonly IEvaluationArticleService _articleService;

    public NickNameChangedIntegrationEventHandler(
        ILogger<NickNameChangedIntegrationEventHandler> logger,
        IEvaluationArticleService articleService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
    }

    public async Task Handle(NickNameChangedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            _logger.LogInformation("----- Handling integration event Begin: {IntegrationEventId} at {AppName} - {@IntegrationEvent}",
                @event.Id, Program.AppName, @event);

            
            await UpdateArticlesUserNameAsync(@event.UserId, @event.NewName);

            _logger.LogInformation("----- Handling integration event End: {IntegrationEventId} at {AppName} - {@IntegrationEvent}",
                @event.Id, Program.AppName, @event);
        }
    }

    private async Task UpdateArticlesUserNameAsync(string userId, string newName)
    {
        var articlesToUpdate = await _articleService.GetArticlesByAuthorInfoAsync(userId);

        if (articlesToUpdate != null && articlesToUpdate.Count != 0)
        {
            _logger.LogInformation("----- NickNameChangedIntegrationEventHandler Begin");

            foreach (var article in articlesToUpdate)
            {
                _logger.LogInformation("----- Updating article's author name from {oldName} to {newName} => articleId: {articleId}",
                    article.NickName, newName, article.ArticleId);

                if (article.NickName != newName)
                    article.NickName = newName;
            }

            await _articleService.BatchUpdateArticlesAsync();
        }
        _logger.LogInformation("----- NickNameChangedIntegrationEventHandler End");

    }
}