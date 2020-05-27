using System;
using System.Collections.Generic;
using System.Text;

namespace WhatsappBroadcastHomeshop.Model
{
    public class GetWhatsappMessageDetailsResponse
    {

        /// <summary>
        /// ProgramCode
        /// </summary>
        public string ProgramCode { get; set; }
        /// <summary>
        /// TemplateName
        /// </summary>
        public string TemplateName { get; set; }
        /// <summary>
        /// TemplateNamespace
        /// </summary>
        public string TemplateNamespace { get; set; }
        /// <summary>
        /// TemplateText
        /// </summary>
        public string TemplateText { get; set; }
        /// <summary>
        /// TemplateLanguage
        /// </summary>
        public string TemplateLanguage { get; set; }
        /// <summary>
        /// Remarks
        /// </summary>
        public string Remarks { get; set; }

    }
}
