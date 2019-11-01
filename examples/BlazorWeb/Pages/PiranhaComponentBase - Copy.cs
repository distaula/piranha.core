using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorWeb.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.AspNetCore.Services;
using Piranha.Models;
using Piranha.Web;

namespace BlazorWeb.Pages
{
    //[Route("/{slug}")]
    public class PageWrapper : ComponentBase //<TType> : ComponentBase where TType : RoutedContent
    {
        [Inject] protected IApi Api { get; set; }
        [Inject] protected IModelLoader ModelLoader { get; set; }
        [Inject] protected IApplicationService AppService { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationState { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }

        private Site _site;

        protected async Task<Site> GetSite(Guid? guid = null)
        {
            if (_site == null || guid.HasValue && guid.Value == _site.Id)
            {
                _site = await Api.Sites.GetByIdAsync(guid.GetValueOrDefault());
            }

            return _site;
        }

        [Parameter] public string Slug { get; set; }
        
        private async Task<RenderFragment<TContent>> GetContent<TContent>() where TContent : RoutedContent
        {
            var response = await PageRouter.InvokeAsync(Api, new Uri(NavigationManager.Uri).LocalPath, _site.Id)
                           ?? await PostRouter.InvokeAsync(Api, new Uri(NavigationManager.Uri).LocalPath, _site.Id)
                           ?? await ArchiveRouter.InvokeAsync(Api, new Uri(NavigationManager.Uri).LocalPath, _site.Id)
                           ?? await AliasRouter.InvokeAsync(Api, new Uri(NavigationManager.Uri).LocalPath, _site.Id);

            //var render
            //Id = response.PageId;
            var state = await AuthenticationState.GetAuthenticationStateAsync();

            switch (response)
            {

            }
            await ModelLoader.GetPage<BlazorWeb.Models.TeaserPage>(response.PageId, state.User, false);
            RenderFragment<TContent> content = value => async builder =>
            {
                //builder.OpenComponent<TeaserPage>(0);
                //builder.AddAttribute();
                builder.AddAttribute(1, "PetDetailsQuote", "Someone's best friend!");
                builder.CloseComponent();
            };

            //RenderFragment frag = builder => builder.OpenComponent(0)

            return content;
            //var model = 
        }

        protected override async Task OnInitializedAsync()
        {
            //_fragment = await GetContent<BlazorWeb.Models.TeaserPage>()
            //MainLayout ly = new MainLayout();
            
        }
    }
}
