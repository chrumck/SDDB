using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Web;
using System.Web.Mvc;

namespace SDDB.WebUI.Infrastructure
{
    //DBJsonDateTime-----------------------------------------------------------------------------------------------------------//

    public class DBJsonDateISO : JsonResult
    {
        private const string _dateFormat = "yyyy-MM-dd";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                // Using Json.NET serializer
                var isoConvert = new IsoDateTimeConverter();
                isoConvert.DateTimeFormat = _dateFormat;
                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            }
        }
    }

    //DBJsonDateTimeISO--------------------------------------------------------------------------------------------------------//

    public class DBJsonDateTimeISO : JsonResult
    {
        private const string _dateFormat = "yyyy-MM-dd HH:mm";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                // Using Json.NET serializer
                var isoConvert = new IsoDateTimeConverter();
                isoConvert.DateTimeFormat = _dateFormat;
                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            }
        }
    }
}