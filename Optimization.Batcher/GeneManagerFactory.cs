﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization.Batcher
{
    public class GeneManagerFactory : IGeneManagerFactory
    {
        public IOptimizerManager Create()
        {
            return new GeneManager();
        }
    }
}
