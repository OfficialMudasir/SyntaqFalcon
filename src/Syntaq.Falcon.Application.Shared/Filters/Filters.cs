using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Syntaq.Falcon.Filters
{
    public class DateTimeZoneNormailser : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //try
            //{
            //    if (context.ActionArguments.ContainsKey("Input"))
            //    {
            //        var jInput = (JObject) context.ActionArguments["input"];
 
            //        foreach (JToken token in jInput.Descendants())
            //        {
            //            WalkNode(token, null, prop =>
            //            {
            //                if (  prop.Value.Type == JTokenType.Date)
            //                {
            //                    // Output Needed 2021-12-15T10:00:00
            //                    DateTime dt = (DateTime)prop.Value;
            //                    prop.Value = Convert.ToString( dt.Year + "-" + dt.Month + "-" + dt.Day + "T" + dt.Hour + ":" + dt.Minute + ":" + dt.Second) ;
            //                }
            //            });
            //        } 
            //    }
            //}
            //catch
            //{
            //    // Do nothing
            //}

        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // our code after action executes
        }

 
        private  void WalkNode(JToken node, Action<JObject> objectAction = null, Action<JProperty> propertyAction = null)
        {
            if (node.Type == JTokenType.Object)
            {
                if (objectAction != null) objectAction((JObject)node);

                foreach (JProperty child in node.Children<JProperty>())
                {
                    if (propertyAction != null) propertyAction(child);
                    WalkNode(child.Value, objectAction, propertyAction);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (JToken child in node.Children())
                {
                    WalkNode(child, objectAction, propertyAction);
                }
            }
        }

    }
}
