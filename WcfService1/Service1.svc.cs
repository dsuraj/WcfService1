﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MyJsonRestService.FaultException;
using MyJsonRestService.Entity;
using System.Web.Script.Serialization;
using System.Net;

namespace WcfService1
{
    public class Service : IService1
    {
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "data/{input}")]
        public string FilterData(string input)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            try
            {
                RootObject rootObject = javaScriptSerializer.Deserialize<RootObject>(input);
                //return the ones with DRM enabled (drm: true) and at least one episode (episodeCount > 0).

                if (rootObject == null)
                {
                    throw new WebFaultException<InvalidJson>(new InvalidJson() { Error = "Could not decode request: JSON parsing failed" }, HttpStatusCode.BadRequest);
                }
                else
                {
                    List<Show> shows = new List<Show>();
                    foreach (Payload payload in rootObject.payload)
                    {
                        if (payload.drm && payload.episodeCount > 0)
                        {
                            shows.Add(new Show()
                            {
                                showImage = payload.image.showImage,
                                slug = payload.slug,
                                title = payload.title
                            });
                        }
                    }
                    return javaScriptSerializer.Serialize(shows);
                }
            }
            catch (Exception ex)
            {
                throw new WebFaultException<InvalidJson>(new InvalidJson() { Error = "Could not decode request: JSON parsing failed" }, HttpStatusCode.BadRequest);
            }
            return javaScriptSerializer.Serialize(string.Empty);
        }
    }
}
