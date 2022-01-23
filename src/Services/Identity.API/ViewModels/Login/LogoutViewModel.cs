// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Identity.API.DtoModels.Login;

namespace Identity.API.ViewModels.Login;

public class LogoutViewModel : LogoutInputModel
{
    public bool ShowLogoutPrompt { get; set; } = true;
}
