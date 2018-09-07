using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.ViewModels
{
    class SensorWindowViewModel
    {
        #region Fields
        private PeripheralSummaryViewModel _summaryVM = new PeripheralSummaryViewModel();
        #endregion

        #region Properties
        public PeripheralSummaryViewModel Summary
        {
            get { return _summaryVM; }
            private set { _summaryVM = value; }
        }
        #endregion
    }
}
