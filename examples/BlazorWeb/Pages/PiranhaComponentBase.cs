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
    public class PiranhaComponentBase<T> : ComponentBase where T : Page<T>
    {
        [Inject]
        protected IApi Api { get; set; }
        [Inject]
        protected IModelLoader ModelLoader { get; set; }
        [Inject]
        protected AuthenticationStateProvider AuthenticationState { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets/sets the model data.
        /// </summary>
        [Parameter]
        public T Model { get; set; }

        [Parameter]
        public Guid Id { get; set; }

       /// <summary>
        /// Gets the model data.
        /// </summary>
        /// <param name="id">The requested model id</param>
        /// <param name="draft">If the draft should be fetched</param>
        public virtual async Task<T> GetModel(bool draft = false)
        {
            var defaultSite = await Api.Sites.GetDefaultAsync();
            var response = await PageRouter.InvokeAsync(Api, new Uri(NavigationManager.Uri).LocalPath, defaultSite.Id);

            Id = response.PageId;

            var state = await AuthenticationState.GetAuthenticationStateAsync();

            return await ModelLoader.GetPage<T>(response.PageId, state.User, draft);
        }
    }
}
