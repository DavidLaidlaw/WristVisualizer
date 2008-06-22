using System;
using System.Collections.Generic;
using System.Text;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class BatchMode
    {
        public BatchMode(CommandLineOptions options)
        {
            switch (options.getMode())
            {
                case CommandLineOptions.BatchModes.ContourData:
                    processContourData(options.subject, options.test_position, options.output);
                    break;
                default:
                    throw new WristVizualizerException("Unknown batch mode encountered: " + options.getMode().ToString());
            }
        }

        private void processContourData(string subject, string test_position, string output)
        {
            Wrist w = new Wrist();
            //TODO: Well...something!
        }
    }
}
