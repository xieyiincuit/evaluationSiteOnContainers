namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Extensions.Profiles;

public static class UserInfoMapping
{
    public static UserInfoDto MapToDtoModel(ApplicationUser src)
    {
        var mapToUser = new UserInfoDto
        {
            UserId = src.Id,
            Avatar = src.Avatar,
            NickName = src.NickName,
            Sex = src.Sex,
            Introduction = src.Introduction,
            BirthOfYear = src.BirthOfYear,
            BirthOfMonth = src.BirthOfMonth,
            BirthOfDay = src.BirthOfDay
        };
        return mapToUser;
    }

    public static ApplicationUser UpdateDtoMapToModel(UserInfoUpdateDto src, ApplicationUser dest)
    {
        dest.NickName = src.NickName;
        dest.Sex = src.Sex;
        dest.Introduction = src.Introduction;
        dest.BirthOfYear = src.BirthOfYear;
        dest.BirthOfMonth = src.BirthOfMonth;
        dest.BirthOfDay = src.BirthOfDay;
        return dest;
    }
}