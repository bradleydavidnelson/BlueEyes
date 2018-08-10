using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.ViewModels
{
    class CalibrationWindowViewModel
    {
        #region Fields
        private CalibrateRheostatViewModel _calibrateRheostatVM = new CalibrateRheostatViewModel();
        #endregion

        #region Properties
        public CalibrateRheostatViewModel RheostatCalibration
        {
            get { return _calibrateRheostatVM; }
            private set { _calibrateRheostatVM = value; }
        }
        #endregion
    }
}
