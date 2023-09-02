namespace Membership.Shared.MessageLocalizer;
internal class MembershipMessageLocalizer : IMembershipMessageLocalizer
{
    readonly Dictionary<string, string> MessagesEspañol = new()
    {
        {MessageKeys.RequiredFirstNameErrorMessage, "El nombre es requerido." },
        {MessageKeys.RequiredLastNameErrorMessage, "El apellido es requerido." },
        {MessageKeys.RequiredEmailErrorMessage, "El correo es requerido." },
        {MessageKeys.RequiredPasswordErrorMessage, "La contraseña es requerida." },
        {MessageKeys.CompareConfirmPasswordErrorMessage, "La contraseña y la confirmación no coinciden." },
        {MessageKeys.PasswordRequiresDigitErrorMessage, "Se requiere al menos un dígito en la contraseña." },
        {MessageKeys.PasswordRequiresLowerErrorMessage,
            "Se requiere al menos un caracter minúscula en la contraseña." },
        {MessageKeys.PasswordRequiresNonAlphanumericErrorMessage,
            "Se requiere al menos un caracter no alfanumérico en la contraseña." },
        {MessageKeys.PasswordRequiresUpperErrorMessage,
            "Se requiere al menos un caracter mayúscula en la contraseña." },
        {MessageKeys.PasswordTooShortErrorMessage,
            "Se requiere al menos 6 caracteres en la contraseña." },

        {MessageKeys.DuplicateEmailErrorMessage,
            "El correo proporcionado ya se encuentra registrado." },
        {MessageKeys.LoginAlreadyAssociatedErrorMessage,
            "El correo proporcionado ya se encuentra asociado a una cuenta de usuario." },

        {MessageKeys.DisplayFirstNameMessage, "Nombre:" },
        {MessageKeys.DisplayLastNameMessage, "Apellidos:" },
        {MessageKeys.DisplayEmailMessage, "Correo:" },
        {MessageKeys.DisplayPasswordMessage, "Contraseña:" },
        {MessageKeys.DisplayConfirmPasswordMessage, "Confirmar contraseña:" },
        {MessageKeys.DisplayRegisterButtonMessage, "Registrar" },
        {MessageKeys.DisplayLoginButtonMessage, "Iniciar sesión" },
        {MessageKeys.DisplayLogoutButtonMessage, "Cerrar sesión" },

        {MessageKeys.RegisterUserExceptionMessage, 
            "Error al registrar al usuario." },
        {MessageKeys.LoginUserExceptionMessage,
            "Las credenciales proporcionadas son incorrectas." },
        {MessageKeys.RefreshTokenCompromisedExceptionMessage,
            "El token de actualización fue comprometido."},
        {MessageKeys.RefreshTokenExpiredExceptionMessage,
            "El token de actualización ha expirado."},
        {MessageKeys.RefreshTokenNotFoundExceptionMessage,
            "El token de actualización no fue encontrado."},

        {MessageKeys.MissingCallbackStateParameterExceptionMessage, 
            "No se recibio el parámetro State requerido."  },
        {MessageKeys.UnableToGetIdpTokensExceptionMessage, 
            "No fue posible obtener la respuesta del servidor de identidad externo."  },
        {MessageKeys.InvalidAuthorizationCodeExceptionMessage ,
            "El código de autorización del cliente no es valido."  },
        {MessageKeys.InvalidRedirectUriExceptionMessage , 
            "El valor redirect_uri no es válido."  },
        {MessageKeys.InvalidClientIdExceptionMessage , 
            "El valor client_id no es válido."  },
        {MessageKeys.InvalidScopeExceptionMessage ,
            "El valor scope no es válido."  },
        {MessageKeys.InvalidCodeVerifierExceptionMessage , 
            "El valor code_verifier no es válido."  },
        {MessageKeys.InvalidScopeActionExceptionMessage , 
            "La acción solicitada no es válida."  },

        {MessageKeys.UnableToGetExternalUserTokens ,
            "No se pudo realizar la operación solicitada."  },
                {MessageKeys.InvalidStateValue ,
            "Se recibió un valor 'state' incorrecto."  },
                {MessageKeys.MissingAuthorizeCallbackParameters,
            "No se recibieron los parámetros esperados."  },
                {MessageKeys.InvalidNonceValue,
            "El valor 'nonce' no es válido."  }
    };

    public string this[string key]
    {
        get
        {            
            MessagesEspañol.TryGetValue(key, out string message);
            return string.IsNullOrWhiteSpace(message) ? key : message;
        }
    }
}
