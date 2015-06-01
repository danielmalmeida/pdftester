using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using EBPP.Domain.Entities.Documents;
using EBPP.Domain.Entities.Documents.Base.DocumentStructure;
using EBPP.Infrastructure.DocumentConverter.XML;

namespace PDFreact.Controllers
{
    public class HomeController : Controller
    {
        private const string LicenseKey = @"<license>
  < licensee >
    < name > Liliana.Vieira@saphety.com </ name >
     </ licensee >
     < product > PDFreactor </ product >
     < majorversion > 7 </ majorversion >
     < minorversion > 0 </ minorversion >
     < licensetype > Evaluation </ licensetype >
     < expirationdate > 2015 - 06 - 25 </ expirationdate >
     < signatureinformation >
       < signdate > 2015 - 05 - 25 16:52 </ signdate >
              < signature > 302c021439369cd333d5d49dcc6868abf4f51fe7f7f3a50f02140f42f0cde3b3154cb8afb706301c7409081ad79e </ signature >
                 < checksum > 1492 </ checksum >
               </ signatureinformation >
             </ license > ";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Pdf(string id)
        {
            var pdfReactor = new PDFreactor();
            pdfReactor.SetLicenseKey(LicenseKey);


            id = string.IsNullOrEmpty(id) ? "default" : id;
            var html = RenderViewToString(ControllerContext, id);

            //if (id == "textbook")
            //{
            //    var cssFilePath = Server.MapPath(string.Format("~/Views/PdfTemplates/{0}.css", id));
            //    pdfReactor.AddUserStyleSheet("", "", "", cssFilePath);
            //}

            ////var result = pdfReactor.RenderDocumentFromContent(html);

            //var htmlFilePath = Server.MapPath(string.Format("~/Views/PdfTemplates/{0}.html", id));
            //var html = System.IO.File.ReadAllText(htmlFilePath);

            //var result = pdfReactor.RenderDocumentFromContent(html);

            id = "invoice";
            var cssFilePath = Server.MapPath(string.Format("~/Views/PdfTemplates/{0}.css", id));
            var css = System.IO.File.ReadAllText(cssFilePath);
            pdfReactor.AddUserStyleSheet(css, "", "", "");

            var xslFilePath = Server.MapPath(string.Format("~/Views/PdfTemplates/{0}.xsl", id));
            var xsl = System.IO.File.ReadAllText(xslFilePath);
            pdfReactor.AddXSLTStyleSheet(xsl, "");

            var xmlFilePath = Server.MapPath(string.Format("~/Views/PdfTemplates/{0}.xml", id));
            var xml = System.IO.File.ReadAllText(xmlFilePath);
            //var htmlFilePath = Server.MapPath(string.Format("~/Views/PdfTemplates/{0}.html", id));
            //var html = System.IO.File.ReadAllText(htmlFilePath);


            var result = pdfReactor.RenderDocumentFromContent(xml);

            return ReturnFile(result);
        }

        public ActionResult PdfDoc()
        {
            var doc = CreateTargetDocument(200);

            var docTemplate = RenderViewToString(ControllerContext, "docTemplate", doc);

            var pdfReactor = new PDFreactor();

            var cssFilePath = Server.MapPath(string.Format("~/Views/PdfTemplates/{0}.css", "docTemplate"));
            var css = System.IO.File.ReadAllText(cssFilePath);
            pdfReactor.AddUserStyleSheet(css, "", "", "");

            var pdf = pdfReactor.RenderDocumentFromContent(docTemplate);

            return ReturnFile(pdf);
        }

        private TargetDocument CreateTargetDocument(int nLines)
        {
            var sampleLines = new List<Line>
            {
                new Line
                {
                    Elements = new List<Element>
                    {
                        new Element {Attribute = "PhoneNumber", Value = "123123"},
                        new Element {Attribute = "Minutes", Value = "60"},
                        new Element {Attribute = "Cost", Value = "1.25"}
                    }
                },
                new Line
                {
                    Elements = new List<Element>
                    {
                        new Element {Attribute = "PhoneNumber", Value = "123123"},
                        new Element {Attribute = "Minutes", Value = "90"},
                        new Element {Attribute = "Cost", Value = "2.25"}
                    }
                },
                new Line
                {
                    Elements = new List<Element>
                    {
                        new Element {Attribute = "PhoneNumber", Value = "99999"},
                        new Element {Attribute = "Minutes", Value = "60"},
                        new Element {Attribute = "Cost", Value = "1.25"}
                    }
                },
                new Line
                {
                    Elements = new List<Element>
                    {
                        new Element {Attribute = "PhoneNumber", Value = "99999"},
                        new Element {Attribute = "Minutes", Value = "5"},
                        new Element {Attribute = "Cost", Value = "0.25"}
                    }
                }
            };

            var doc = new TargetDocument
            {
                Content = new Content
                {
                    Header = new Header
                    {
                        Elements = new List<Element>
                        {
                            new Element {Attribute = "Company", Value = "Saph"},
                            new Element {Attribute = "Address", Value = "Picoas"}
                        }
                    },
                    Body = new Body
                    {
                        Lines = new List<Line>()
                    }
                }
            };

            var rnd = new Random((int) DateTime.Now.Ticks);
            for (var i = 0; i < nLines; i++)
            {
                var randomIndex = rnd.Next(sampleLines.Count);
                var line = sampleLines[randomIndex];

                doc.Content.Body.Lines.Add(line);
            }
            return doc;
        }


        private string RenderViewToString(ControllerContext context, string template, object model = null)
        {
            var templatePath = string.Format("~/Views/PdfTemplates/{0}.cshtml", template);

            // first find the ViewEngine for this view
            var viewEngineResult = ViewEngines.Engines.FindView(context, templatePath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result;
            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public ActionResult PdfDocXml()
        {
            var pdfReactor = new PDFreactor();

            var doc = CreateTargetDocument(200);

            var xml = new XmlDocumentSerializer().SerializeDocument(doc);

            var xslFilePath = Server.MapPath(string.Format("~/Views/PdfTemplates/{0}.xsl", "docTemplate"));
            var xsl = System.IO.File.ReadAllText(xslFilePath);
            pdfReactor.AddXSLTStyleSheet(xsl, "");

            //var cssFilePath = Server.MapPath(string.Format("~/Views/PdfTemplates/{0}.css", "docTemplate"));
            //var css = System.IO.File.ReadAllText(cssFilePath);
            //pdfReactor.AddUserStyleSheet(css, "", "", "");

            var pdf = pdfReactor.RenderDocumentFromContent(xml);

            return ReturnFile(pdf);
        }

        private ActionResult ReturnFile(byte[] pdf)
        {
            Response.AppendHeader("Content-Disposition", "application/pdf");
            return File(pdf, "application/pdf");
        }
    }
}