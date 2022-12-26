export type ICredentials = {
    email?: string;
    password?: string;
    rememberMe?: boolean;
};
export enum AuthStatusEnum {
    FAIL = 'fail',
    NONE = 'none',
    PROCESS = 'process',
    SUCCESS = 'success'
};
export type IAuthUser = {
    token?: string;
    userName?: string;
    status?: AuthStatusEnum;
};

export enum SignInStatus {
    Succeeded,
    RequiresTwoFactor,
    IsLockedOut,
    Invalid,
    Process,
    None
}

export type IModelState = {
    [key: string]: any
}

export type ILoginResult = {
    modelState: IModelState | null;
    status: SignInStatus;
    redirectPath: string;
}

export type IAuthState = ILoginResult & { isAuthenticated: boolean; };