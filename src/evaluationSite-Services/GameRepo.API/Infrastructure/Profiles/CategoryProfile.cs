﻿namespace Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<GameCategoryAddDto, GameCategory>();
        CreateMap<GameCategoryUpdateDto, GameCategory>();
    }
}
