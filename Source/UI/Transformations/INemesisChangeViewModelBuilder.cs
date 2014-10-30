using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Logic.Nemeses;
using UI.Models.Nemeses;

namespace UI.Transformations
{
    public interface INemesisChangeViewModelBuilder
    {
        List<NemesisChangeViewModel> Build(List<NemesisChange> nemesisChanges);
    }
}
