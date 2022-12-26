import React, { useEffect } from 'react';
import { SignInStatus, IModelState } from '../store/auth/types';
import { CallbackFunction } from '../types';
import Alert from '@material-ui/lab/Alert';

type AuthenticatorProps = {
    readonly delay?: number;
    readonly authStatus?: SignInStatus;
    readonly modelState?: IModelState;
    readonly handleOnFail: CallbackFunction;
    readonly handleOnSuccess: CallbackFunction;
};
const Authenticator = React.memo<AuthenticatorProps>(({ authStatus, modelState, handleOnFail, handleOnSuccess, delay = 2000 }) => {

    const errorMessage = modelState && modelState[""].errors[0].errorMessage;
    useEffect(() => {
        const authHandler = setTimeout(() => {
            switch (authStatus) {
                case SignInStatus.Invalid:
                case SignInStatus.IsLockedOut:
                case SignInStatus.RequiresTwoFactor:
                    handleOnFail();
                    return;
                case SignInStatus.Succeeded:
                    handleOnSuccess();
                    return;
                default:
                    return;
            }
        }, delay);

        return () => {
            clearTimeout(authHandler);
        };
    }, [authStatus, delay, handleOnFail, handleOnSuccess]);

    if (!authStatus || authStatus === SignInStatus.None) {
        return null;
    }

    if (errorMessage == null) {
        return null;
    }

    return (
        <Alert severity="error">{errorMessage}</Alert>
    );
});

Authenticator.displayName = 'Authenticator';

export default Authenticator;