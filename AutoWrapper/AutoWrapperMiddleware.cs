﻿using AutoWrapper.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AutoWrapper
{
    public class AutoWrapperMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AutoWrapperOptions _options;
        private readonly ILogger<AutoWrapperMiddleware> _logger;
        public AutoWrapperMiddleware(RequestDelegate next, AutoWrapperOptions options, ILogger<AutoWrapperMiddleware> logger)
        {
            _next = next;
            _options = options;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
         {
            AutoWrapperMembers awm = new AutoWrapperMembers(_options, _logger);

            if (awm.IsSwagger(context) || !awm.IsApi(context))
                await this._next(context);
            else
            {

                var stopWatch = Stopwatch.StartNew();

                var request = await awm.FormatRequest(context.Request);

                var originalBodyStream = context.Response.Body;

                using (var newBodyStream = new MemoryStream())
                {
                    try
                    {
                        context.Response.Body = newBodyStream;
                        await _next.Invoke(context);
                        context.Response.Body = originalBodyStream;
     
                        var bodyAsText = await awm.FormatResponse(newBodyStream);

                        if (!_options.IsApiOnly && bodyAsText.IsHtml() && context.Response.StatusCode == 200)
                            context.Response.StatusCode = 404;


                        if (_options.IsApiOnly && !context.Request.Path.StartsWithSegments(new PathString(_options.WrapWhenApiPathStartsWith)))
                        {
                            await awm.HandleSpaSupportAsync(context);
                        }
                        else if (context.Response.StatusCode == 200)
                        {
                            await awm.HandleSuccessRequestAsync(context, bodyAsText, context.Response.StatusCode);
                        }
                        else
                        {
                            await awm.HandleNotSuccessRequestAsync(context, context.Response.StatusCode);
                        }

                    }
                    catch (Exception ex)
                    {
                        await awm.HandleExceptionAsync(context, ex);
                        newBodyStream.Seek(0, SeekOrigin.Begin);
                        await newBodyStream.CopyToAsync(originalBodyStream);
                    }
                    finally
                    {
                        stopWatch.Stop();
                        _logger.Log(LogLevel.Information, $@"Request: {request} Responded with [{context.Response.StatusCode}] in {stopWatch.ElapsedMilliseconds}ms");
                    }
                }

            }

        }

    }
}
