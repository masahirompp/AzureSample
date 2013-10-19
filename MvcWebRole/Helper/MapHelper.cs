using System.Linq;
using System.Web.Mvc;

namespace jp.ne.ghopper.echo.azuredemo.Helper
{
    public static class MapHelper
    {
        /// <summary>
        /// FormCollectionをmodelにMapします。
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static object Map(FormCollection collection, object model)
        {
            foreach (var p in
                model.GetType().GetProperties().AsParallel()
                .Where(p => collection[p.Name] != null)
                .Select(p => new string[2] { p.Name, collection[p.Name] }))
            {
                var property = model.GetType().GetProperty(p[0]);
                if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                {
                    if (!string.IsNullOrEmpty(p[1])) property.SetValue(model, int.Parse(p[1]), null);
                }
                else
                {
                    property.SetValue(model, p[1], null);
                }
            }

            return model;
        }
    }
}