using System;
using System.Text;

namespace BusinessLogic.Jobs
{
    public abstract class BaseJobResult
    {
        protected BaseJobResult()
        {
            Success = true;
        }

        public TimeSpan TimeEllapsed { get; set; }
        public bool Success { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Job ended after {TimeEllapsed.TotalSeconds} seconds with a {Success} result");

            return sb.ToString();
        }
    }
}