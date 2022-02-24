﻿namespace Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.Consent;

public class ConsentInputModel
{
    public string Button { get; set; }
    public IEnumerable<string> ScopesConsented { get; set; }
    public bool RememberConsent { get; set; }
    public string ReturnUrl { get; set; }
    public string Description { get; set; }
}