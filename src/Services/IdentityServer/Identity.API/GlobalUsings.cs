﻿global using Autofac;
global using Autofac.Extensions.DependencyInjection;
global using HealthChecks.UI.Client;
global using IdentityModel;
global using IdentityServer4;
global using IdentityServer4.Configuration;
global using IdentityServer4.EntityFramework.DbContexts;
global using IdentityServer4.EntityFramework.Options;
global using IdentityServer4.Events;
global using IdentityServer4.Extensions;
global using IdentityServer4.Models;
global using IdentityServer4.Services;
global using IdentityServer4.Stores;
global using IdentityServer4.Validation;
global using Microsoft.AspNetCore;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Design;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Minio;
global using Minio.AspNetCore;
global using RabbitMQ.Client;
global using Serilog;
global using Serilog.Events;
global using Serilog.Sinks.SystemConsole.Themes;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Data.Common;
global using System.IdentityModel.Tokens.Jwt;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text;
global using System.Threading.Tasks;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase.Abstractions;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase.Events;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusRabbitMQ;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF.Services;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF.Utilities;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.WebHost.Customization;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Configurations;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Data;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.Consent;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.Device;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.Login;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.DtoModels.UserInfo;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Extensions;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Extensions.Profiles;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Infrastructure;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.IntegrationEvents;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.IntegrationEvents.Events;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Models;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.Services;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.ViewModels;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.ViewModels.Consent;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.ViewModels.Device;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.ViewModels.Diagnostics;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.ViewModels.Grant;
global using Zhouxieyi.evaluationSiteOnContainers.Services.Identity.API.ViewModels.Login;