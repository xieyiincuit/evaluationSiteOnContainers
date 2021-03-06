global using Autofac;
global using Microsoft.Extensions.Logging;
global using Polly;
global using Polly.Retry;
global using RabbitMQ.Client;
global using RabbitMQ.Client.Events;
global using RabbitMQ.Client.Exceptions;
global using System.Net.Sockets;
global using System.Text;
global using System.Text.Json;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase.Abstractions;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase.Events;
global using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.EventBusBase.Extensions;