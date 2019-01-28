/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using Python.Runtime;
using QuantConnect.Logging;
using System;
using System.Diagnostics;

namespace QuantConnect.Python
{
    /// <summary>
    /// Wraps Python debugger package for use with Visual Studio.
    /// https://github.com/Microsoft/ptvsd
    /// </summary>
    public static class PythonDebugger
    {
        private static dynamic _ptvsd;
        private static bool _isInitialized = false;

        /// <summary>
        /// Break into debugger.
        /// </summary>
        public static void Break()
        {
            Initialize();

            if (_ptvsd != null)
            {
                using (Py.GIL())
                {
                    _ptvsd.break_into_debugger();
                }
            }
        }

        private static void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            if (OS.IsLinux)
            {
                Log.Error("PythonDebugger.Break() not supported in Linux.");
                return;
            }

            // If .NET debugger is attached, cannot break into python code
            if (Debugger.IsAttached)
            {
                Log.Error(".NET debugger is attached. PythonDebugger.Break() calls will be ignored. To enable it, please run the Lean without debugging.");
                return;
            }

            using (Py.GIL())
            {
                try
                {
                    _ptvsd = Py.Import("ptvsd");
                    _ptvsd.enable_attach();

                    Log.Error($@"Python Tool for Visual Studio Debugger {_ptvsd.__version__}
Please attach the python debugger:
- In Visual Studio, select Debug > Attach to Process (or press Ctrl+Alt+P) to open the Attach to Process dialog box.
- For Connection type, select Python remote (ptvsd)
- In the Connection target box, select tcp://localhost:{5678}/
- Press Attach button");

                    _ptvsd.wait_for_attach();
                }
                catch (PythonException e)
                {
                    var message = e.Message + Environment.NewLine;

                    if (e.Message.Contains("ModuleNotFoundError"))
                    {
                        message += "Please install PTVSD package to enable debugging: pip install ptvsd." +
                            Environment.NewLine;
                    }

                    Log.Error(message + "DEBUGGING IS DISABLED.");
                }
            }
        }
    }
}