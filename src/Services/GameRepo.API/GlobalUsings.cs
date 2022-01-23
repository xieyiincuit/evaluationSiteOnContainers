﻿global using Autofac;
global using Autofac.Extensions.DependencyInjection;
global using AutoMapper;
global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Design;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Options;
global using Microsoft.OpenApi.Models;
global using Polly;
global using Polly.Retry;
global using RabbitMQ.Client;
global using Serilog;
global using Serilog.Events;
global using System;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Data.Common;
global using System.Net;
global using System.Reflection;
global using System.Text.Json.Serialization;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase.Abstractions;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase.Events;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusRabbitMQ;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF.Services;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF.Utilities;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Extensions;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.ActionResults;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Exceptions;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.IntegrationEvents;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.IntegrationEvents.Events;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;
