//-----------------------------------------------------------------------------
// <copyright file="MyODataRoutingApplicationModelProvider.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Nox.Api.OData.Constants;
using Nox.Api.OData.Routing.TemplateSegments;
using Nox.Api.OData.Serializers;
using Nox.Core.Interfaces.Database;

namespace Nox.Api.OData.Routing
{
    public class EntityODataRoutingApplicationModelProvider : IApplicationModelProvider
    {
        private readonly IDynamicModel _model;

        public EntityODataRoutingApplicationModelProvider(
            IOptions<ODataOptions> options,
            IDynamicModel model)
        {
            options.Value.AddRouteComponents(RoutingConstants.ODATA_ROUTE_PREFIX, EdmCoreModel.Instance, builder => builder.AddSingleton<IODataSerializerProvider, CustomODataSerializerProvider>());

            _model = model;
        }

        /// <summary>
        /// Gets the order value for determining the order of execution of providers.
        /// </summary>
        public int Order => 90;

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            var model = _model.GetEdmModel();
            foreach (var controllerModel in context.Result.Controllers)
            {
                if (controllerModel.ControllerName == "OData")
                {
                    ProcessHandleAll(RoutingConstants.ODATA_ROUTE_PREFIX, model, controllerModel);
                    continue;
                }

                if (controllerModel.ControllerName == "Meta")
                {
                    ProcessODataMetadata(RoutingConstants.ODATA_METADATA_ROUTE_PREFIX, model, controllerModel);
                    continue;
                }
            }
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
        }

        private static void ProcessHandleAll(string prefix, IEdmModel model, ControllerModel controllerModel)
        {
            foreach (var actionModel in controllerModel.Actions)
            {
                if (actionModel.ActionName == "GetNavigation")
                {
                    var path = new ODataPathTemplate(
                        new EntitySetTemplateSegment(),
                        new EntitySetWithKeyTemplateSegment(),
                        new NavigationTemplateSegment());
                    actionModel.AddSelector("get", prefix, model, path, new ODataRouteOptions());
                }
                else if (actionModel.ActionName == "GetProperty")
                {
                    var path = new ODataPathTemplate(
                        new EntitySetTemplateSegment(),
                        new EntitySetWithKeyTemplateSegment(),
                        new PropertyTemplateSegment());

                    actionModel.AddSelector("get", prefix, model, path);
                }
                else if (actionModel.ActionName == "Get")
                {
                    if (actionModel.Parameters.Count == 0)
                    {
                        var path = new ODataPathTemplate(new EntitySetTemplateSegment());
                        actionModel.AddSelector("get", prefix, model, path);
                    }
                    else
                    {
                        var path = new ODataPathTemplate(new EntitySetTemplateSegment(), new EntitySetWithKeyTemplateSegment());
                        actionModel.AddSelector("get", prefix, model, path);
                    }
                }
                else if (actionModel.ActionName == "Post")
                {
                    var path = new ODataPathTemplate(new EntitySetTemplateSegment());

                    actionModel.AddSelector("post", prefix, model, path);

                }
                else if (actionModel.ActionName == "Put")
                {
                    var path = new ODataPathTemplate(new EntitySetTemplateSegment());

                    actionModel.AddSelector("put", prefix, model, path);

                }
                else if (actionModel.ActionName == "Patch")
                {
                    var path = new ODataPathTemplate(new EntitySetTemplateSegment(), new EntitySetWithKeyTemplateSegment());

                    actionModel.AddSelector("patch", prefix, model, path);

                }
                else if (actionModel.ActionName == "Delete")
                {
                    var path = new ODataPathTemplate(new EntitySetTemplateSegment(), new EntitySetWithKeyTemplateSegment());

                    actionModel.AddSelector("delete", prefix, model, path);

                }
            }
        }

        private static void ProcessODataMetadata(string prefix, IEdmModel model, ControllerModel controllerModel)
        {
            foreach (var actionModel in controllerModel.Actions)
            {
                if (actionModel.ActionName == "GetMetadata")
                {
                    var path = new ODataPathTemplate(MetadataSegmentTemplate.Instance);
                    actionModel.AddSelector("get", prefix, model, path);
                }
                else if (actionModel.ActionName == "GetServiceDocument")
                {
                    var path = new ODataPathTemplate();
                    actionModel.AddSelector("get", prefix, model, path);
                }
            }
        }
    }
}
