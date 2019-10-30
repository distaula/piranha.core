using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Piranha.Extend;
using Piranha.Extend.Blocks;

namespace BlazorWeb.Helpers
{
    public static class RenderExtensions
    {
        public static string GetBlockContent(this Block block)
        {
            switch (block)
            {
                case HtmlBlock htmlBlock:
                    return htmlBlock.Body;
            }

            return string.Empty;
        }

    }
}
