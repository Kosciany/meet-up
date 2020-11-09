﻿using System;

namespace WebApplication.Exceptions
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException()
        {
            
        }
        public AuthorizationException(string message)
         : base(message)
        {
        }

    }
}
