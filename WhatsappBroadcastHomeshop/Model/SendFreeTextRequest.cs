using System;
using System.Collections.Generic;
using System.Text;

namespace WhatsappBroadcastHomeshop.Model
{
    public class SendFreeTextRequest
    {
        /// <summary>
        /// To
        /// </summary>
        public string To { get; set; }
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
        //public string TemplateNamespace { get; set; }
        /// <summary>
        /// TemplateName
        /// </summary>
        public List<string> AdditionalInfo { get; set; }
    }
}
