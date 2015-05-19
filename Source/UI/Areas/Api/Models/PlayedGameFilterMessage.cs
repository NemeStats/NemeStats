using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class PlayedGameFilterMessage
    {
        [RegularExpression(@"^(20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])$)]")]
        public string StartDateGameLastUpdated { get; set; }

        public int? MaximumNumberOfResults { get; set; }
    }
}
