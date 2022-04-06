﻿global using Autofac;
global using Autofac.Extensions.DependencyInjection;
global using AutoMapper;
global using Grpc.Core;
global using GrpcGameRepository;
global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.HttpLogging;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Authorization;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Design;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Logging;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Minio;
global using Minio.AspNetCore;
global using Minio.AspNetCore.HealthChecks;
global using MySqlConnector;
global using Polly;
global using Polly.Retry;
global using RabbitMQ.Client;
global using RedLockNet;
global using RedLockNet.SERedis;
global using RedLockNet.SERedis.Configuration;
global using Serilog;
global using Serilog.Events;
global using StackExchange.Redis.Extensions.Core.Abstractions;
global using StackExchange.Redis.Extensions.Core.Configuration;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using System;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Data.Common;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text.Json.Serialization;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase.Abstractions;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase.Events;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusRabbitMQ;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF.Services;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF.Utilities;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.RedisRepository;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.WebHost.Customization;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Auth;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.DtoModels;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Grpc;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.HttpClients;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.ActionResults;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Exceptions;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure.Filters;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.IntegrationEvents;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.IntegrationEvents.Events;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Interfaces;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models;
global using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Services;