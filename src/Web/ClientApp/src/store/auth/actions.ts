import axios, { AxiosError } from 'axios';
import { AppThunk, RootState } from "..";
import { AuthApi } from '../../services';
import { CallbackFunction } from '../../types';
import { slice } from "./reducer";
import { ICredentials, ILoginResult, SignInStatus } from "./types";

const { setAuthStatus, loginSuccess, loginFail, resetState } = slice.actions;

export const setSignInStatus = (signInStatus: SignInStatus): AppThunk => dispatch => {
    dispatch(setAuthStatus(signInStatus));
};

export const loginUserRequest = (credentials: ICredentials, returnUrl: string): AppThunk => dispatch => {
    AuthApi.loginAsync(credentials, returnUrl)
        .then((loginResult: ILoginResult) => {
            const dispatchBody = (loginResult.status === SignInStatus.Succeeded)
                ? dispatch(loginSuccess(loginResult))
                : dispatch(loginFail(loginResult));
        }).catch((error: AxiosError) => {
            dispatch(loginFail(error.response.data));
        });
};

export const logoutUserRequest = (handleRouteCallback: CallbackFunction): AppThunk => dispatch => {
    AuthApi.logoutAsync()
        .then(() => {
            handleRouteCallback();
            dispatch(resetState());
        });
};

export const reset = (): AppThunk => dispatch => {
    dispatch(resetState());
};

export const getStatus = (state: RootState) => state.auth.status;
export const getRedirectPath = (state: RootState) => state.auth.redirectPath;
export const getModelState = (state: RootState) => state.auth.modelState;
export const getErrors = (state: RootState) => {
    const modelState = state.auth.modelState;
    return modelState ? modelState[""].errors : null;
}