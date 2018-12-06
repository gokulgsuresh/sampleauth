using System.Web;
using System.Web.Mvc;
using WebApiFilterPOC.Filters;

namespace WebApiFilterPOC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
