﻿global using Autofac.Extensions.DependencyInjection;
global using Autofac;
global using Polly.Retry;
global using Polly;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http.Features;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.Authorization;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.AspNetCore;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Security.Claims;
global using System.Text.Json;
global using System.Threading.Tasks;
global using System;
global using System.Reflection;
global using Serilog.Context;
global using Serilog;
global using Serilog.Events;
global using Microsoft.OpenApi.Models;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Design;
global using Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API;
global using Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Models;
global using Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Infrastructure;

global using Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Infrastructure.Exceptions;
global using Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Infrastructure.ActionResults;
global using Zhouxieyi.evalutionSiteOnContainers.Services.GameRepo.API.Extensions;