using AutoMapper;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PropertySearchApp.Common.Exceptions.Abstract;
using PropertySearchApp.Domain;
using PropertySearchApp.Models;
using PropertySearchApp.Services.Abstract;

namespace PropertySearchApp.Controllers;

public class IdentityController : Controller
{
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;
    public IdentityController(IIdentityService identityService, IMapper mapper)
    {
        _identityService = identityService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [ValidateAntiForgeryToken, HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginModel)
    {
        if (ModelState.IsValid == false)
            return View(loginModel);

        var result = await _identityService.LoginAsync(loginModel.Username, loginModel.Password);

        return result.Match<IActionResult>(success =>
        {
            return RedirectToAction("Index", "Home");
        }, exception =>
        {
            if (AddErrorsToModelState(ModelState, exception))
                return View(loginModel);
            else
                throw exception;
        });
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [ValidateAntiForgeryToken, HttpPost]
    public async Task<IActionResult> Register(RegistrationFormViewModel registrationModel)
    {
        if (ModelState.IsValid == false)
            return View(registrationModel);

        Result<bool> result = await _identityService.RegisterAsync(_mapper.Map<UserDomain>(registrationModel));

        return result.Match<IActionResult>(success =>
        {
            return RedirectToAction("Index", "Home");
        }, exception =>
        {
            if (AddErrorsToModelState(ModelState, exception))
                return View(registrationModel);
            else
                throw exception;
        });
    }
    [HttpPost, Authorize]
    public async Task<IActionResult> Logout()
    {
        await _identityService.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private bool AddErrorsToModelState(ModelStateDictionary modelState, Exception exception)
    {
        if (exception is AuthorizationOperationException registrationException)
        {
            foreach (var error in registrationException.GetErrors())
            {
                modelState.AddModelError(string.Empty, error);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}