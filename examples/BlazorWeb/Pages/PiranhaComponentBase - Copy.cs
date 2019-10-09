using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.Models;
using Piranha.Web;

namespace BlazorWeb.Pages
{
    public class PiranhaComponentBase2<T> : ComponentBase where T : Page<T>
    {
        [Inject]
        protected IApi Api { get; set; }
        [Inject]
        protected IModelLoader ModelLoader { get; set; }
        [Inject] 
        protected IApplicationService AppService { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthenticationState { get; set; }
        [Inject] 
        protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets/sets the model data.
        /// </summary>
        //[CascadingParameter]
        protected T Model { get; set; }

        [Parameter]
        public Guid Id { get; set; }

       /// <summary>
        /// Gets the model data.
        /// </summary>
        /// <param name="id">The requested model id</param>
        /// <param name="draft">If the draft should be fetched</param>
        public virtual async Task<T> GetModel(bool draft = false)
        {
            var uri = new Uri(NavigationManager.Uri);

            var queryStrings = System.Web.HttpUtility.ParseQueryString(uri.Query);

            //if (!string.IsNullOrEmpty(queryStrings.Get("id")))
            //{
            //    Id = Guid.Parse(queryStrings["id"]);
            //}
            //else
            //{
                await AppService.InitAsync(uri);
                var response = await StartPageRouter.InvokeAsync(AppService.Api, uri.LocalPath, AppService.Site.Id);
            //    Id = response.PageId;
            //}

            var state = await AuthenticationState.GetAuthenticationStateAsync();
            return await ModelLoader.GetPage<T>(Id, state.User, draft);
        }
    }
}
