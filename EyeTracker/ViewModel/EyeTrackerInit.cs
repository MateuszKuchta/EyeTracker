using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EyeTracker.ViewModel
{
    //TODO
    public class EyeTrackerInit
    {
        private ICommand eyeTrackerStartCommand;

        public ICommand EyeTrackerStart
        {
            get
            {
                if (eyeTrackerStartCommand == null)
                    eyeTrackerStartCommand = new MvvmCommand(parameter => { EyeTrackerStartMethod(); });
                return eyeTrackerStartCommand;
            }
        }

        public void EyeTrackerStartMethod()
        {

        }
    }
}
