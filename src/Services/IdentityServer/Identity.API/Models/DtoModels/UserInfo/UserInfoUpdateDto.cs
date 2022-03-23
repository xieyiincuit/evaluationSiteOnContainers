namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.UserInfo;

public class UserInfoUpdateDto
{
    [Required(ErrorMessage = "required | 请输入昵称")]
    [MaxLength(30, ErrorMessage = "length | 昵称不要超过30个字符")]
    public string NickName { get; set; }

    //此处头像上传不应该结合在表单操作中
    //[Required(ErrorMessage = "required | 请上传头像")]
    //public string Avatar { get; set; }

    [Required(ErrorMessage = "required | 请选择性别")]
    [Range(0, 1, ErrorMessage = "invalid | 非法参数: sex")]
    public Gender? Sex { get; set; }

    public DateTime? BirthDate { get; set; }
    
    [MaxLength(50, ErrorMessage = "length | 自我介绍控制在50个字符以内")]
    public string Introduction { get; set; }
}