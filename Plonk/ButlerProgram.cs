﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neato
{
    public class ButlerProgram : ExternalProgram
    {
        public ButlerProgram() : base("butler")
        {
        }

        public void Logout()
        {
            RunWithArgs("logout", "--assume-yes");
        }

        public void Login()
        {
            RunWithArgs("login");
        }
    }
}
