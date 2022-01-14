﻿global using Autofac.Extensions.DependencyInjection;
global using Autofac;
global using Serilog;
global using AutoMapper;
global using Microsoft.AspNetCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Design;
global using Microsoft.OpenApi.Models;
global using Microsoft.Extensions.Options;
global using HealthChecks.UI.Client;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.Data.SqlClient;
global using Polly;
global using Polly.Retry;
global using System.Net;
global using System.Reflection;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.ActionResults;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.Exceptions;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.Filters;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure.EntityConfigurations;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.DtoModel;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Extensions;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Interfaces;
global using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Services;