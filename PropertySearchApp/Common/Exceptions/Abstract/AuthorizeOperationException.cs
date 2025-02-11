﻿namespace PropertySearchApp.Common.Exceptions.Abstract;

public class AuthorizationOperationException : Exception
{
    protected string[] _errors;

    public AuthorizationOperationException(string[] errors) : base()
    {
        _errors = errors;
    }

    public string[] GetErrors()
    {
        return _errors;
    }
}
