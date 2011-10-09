using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeedbackManager.Model
{
    [Serializable]
    public class FeedbackClipboardInfo
    {
        public Byte[] Screenshot { get; set; }
        public string ClientVersion { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
    }
}
