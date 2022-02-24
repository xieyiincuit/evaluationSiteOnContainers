namespace Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.IntegrationEvents.EventHandling;

public class GameNameChangedIntegrationEventHandler :
    IIntegrationEventHandler<GameNameChangedIntegrationEvent>
{
    private readonly IEvaluationArticleService _articleService;
    private readonly ILogger<GameNameChangedIntegrationEventHandler> _logger;

    public GameNameChangedIntegrationEventHandler(
        ILogger<GameNameChangedIntegrationEventHandler> logger,
        IEvaluationArticleService articleService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _articleService = articleService ?? throw new ArgumentNullException(nameof(articleService));
    }

    public async Task Handle(GameNameChangedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
        {
            _logger.LogInformation(
                "----- Handling integration event Begin: {IntegrationEventId} at {AppName} - {@IntegrationEvent}",
                @event.Id, Program.AppName, @event);

            await UpdateArticlesGameNameAsync(@event.GameId, @event.NewName);

            _logger.LogInformation(
                "----- Handling integration event End: {IntegrationEventId} at {AppName} - {@IntegrationEvent}",
                @event.Id, Program.AppName, @event);
        }
    }

    private async Task UpdateArticlesGameNameAsync(int gameId, string newName)
    {
        var articlesToUpdate = await _articleService.GetArticlesByGameInfoAsync(gameId);

        if (articlesToUpdate != null && articlesToUpdate.Count != 0)
        {
            _logger.LogInformation("----- GameNameChangedIntegrationEventHandler Begin");

            foreach (var article in articlesToUpdate)
            {
                _logger.LogInformation(
                    "----- Updating article's game name from {oldName} to {newName} => articleId: {articleId}",
                    article.GameName, newName, article.ArticleId);

                if (article.GameName != newName)
                    article.GameName = newName;
            }

            await _articleService.BatchUpdateArticlesAsync();
        }

        _logger.LogInformation("----- GameNameChangedIntegrationEventHandler End");
    }
}